/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System.Net.NetworkInformation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class BlockAcknowledgmentFrameTest
    {
        /// <summary>
        /// Test that parsing a block acknowledgment frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_block_acknowledgment_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (BlockAcknowledgmentFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlBlockAcknowledgment, frame.FrameControl.SubType);
            ClassicAssert.IsFalse(frame.FrameControl.ToDS);
            ClassicAssert.IsFalse(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsFalse(frame.FrameControl.Protected);
            ClassicAssert.IsFalse(frame.FrameControl.Order);
            ClassicAssert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("0024B2F8D706", frame.ReceiverAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("7CC5376D16E7", frame.TransmitterAddress.ToString().ToUpper());

            ClassicAssert.AreEqual(BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, frame.BlockAcknowledgmentControl.Policy);
            ClassicAssert.IsFalse(frame.BlockAcknowledgmentControl.MultiTid);
            ClassicAssert.IsTrue(frame.BlockAcknowledgmentControl.CompressedBitmap);
            ClassicAssert.AreEqual(0, frame.BlockAcknowledgmentControl.Tid);
            ClassicAssert.AreEqual(0, frame.BlockAckStartingSequenceControl);
            ClassicAssert.AreEqual(8, frame.BlockAckBitmap.Length);

            ClassicAssert.AreEqual(0x9BB909C2, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(28, frame.FrameSize);
        }

        [Test]
        public void Test_Constructor_ConstructWithValues()
        {
            byte[] blockAckBitmap = { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8 };

            var frame = new BlockAcknowledgmentFrame(PhysicalAddress.Parse("111111111111"),
                                                     PhysicalAddress.Parse("222222222222"),
                                                     blockAckBitmap)
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                Duration = { Field = 0x1234 },
                BlockAcknowledgmentControl = { Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, Tid = 0xF },
                BlockAckStartingSequenceControl = 0x5678
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as BlockAcknowledgmentFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlBlockAcknowledgment, recreatedFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.FromDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.MoreFragments);
            ClassicAssert.AreEqual(0x1234, recreatedFrame.Duration.Field);
            ClassicAssert.AreEqual(BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed,
                            recreatedFrame.BlockAcknowledgmentControl.Policy);

            ClassicAssert.AreEqual(0xF, recreatedFrame.BlockAcknowledgmentControl.Tid);
            ClassicAssert.IsTrue(recreatedFrame.BlockAcknowledgmentControl.CompressedBitmap);
            ClassicAssert.AreEqual(0x5678, recreatedFrame.BlockAckStartingSequenceControl);

            ClassicAssert.AreEqual("111111111111", recreatedFrame.TransmitterAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("222222222222", recreatedFrame.ReceiverAddress.ToString().ToUpper());

            CollectionAssert.AreEqual(blockAckBitmap, recreatedFrame.BlockAckBitmap);

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new BlockAcknowledgmentFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }
    }