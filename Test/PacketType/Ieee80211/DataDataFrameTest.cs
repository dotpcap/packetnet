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
    public class DataDataFrameTest
    {
        /// <summary>
        /// Test that parsing an ecrypted data frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor_EncryptedDataFrame()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_encrypted_data_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (DataDataFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, frame.FrameControl.SubType);
            ClassicAssert.IsFalse(frame.FrameControl.ToDS);
            ClassicAssert.IsTrue(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsTrue(frame.FrameControl.Protected);
            ClassicAssert.IsFalse(frame.FrameControl.Order);
            ClassicAssert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("33330000000C", frame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
            ClassicAssert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            ClassicAssert.AreEqual(1561, frame.SequenceControl.SequenceNumber);

            ClassicAssert.AreEqual(0xC77B6323, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(24, frame.FrameSize);
            ClassicAssert.AreEqual(218, frame.PayloadData.Length);
        }

        [Test]
        public void Test_Constructor_UnecryptedDataFrameFromValues()
        {
            var frame = new DataDataFrame
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                SequenceControl = { SequenceNumber = 0x89, FragmentNumber = 0x1 },
                Duration = { Field = 0x1234 },
                DestinationAddress = PhysicalAddress.Parse("111111111111"),
                SourceAddress = PhysicalAddress.Parse("222222222222"),
                BssId = PhysicalAddress.Parse("333333333333"),
                PayloadData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as DataDataFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, recreatedFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.FromDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.MoreFragments);

            ClassicAssert.AreEqual(0x89, recreatedFrame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(0x1, recreatedFrame.SequenceControl.FragmentNumber);

            ClassicAssert.AreEqual("111111111111", recreatedFrame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("222222222222", recreatedFrame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("333333333333", recreatedFrame.BssId.ToString().ToUpper());

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, recreatedFrame.PayloadData);

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        /// <summary>
        /// Test that parsing an unecrypted data frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor_UnencryptedDataFrame()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_unencrypted_data_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (DataDataFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.Data, frame.FrameControl.SubType);
            ClassicAssert.IsTrue(frame.FrameControl.ToDS);
            ClassicAssert.IsFalse(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsFalse(frame.FrameControl.Protected);
            ClassicAssert.IsFalse(frame.FrameControl.Order);
            ClassicAssert.AreEqual(44, frame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("0024B2F8D706", frame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
            ClassicAssert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            ClassicAssert.AreEqual(2712, frame.SequenceControl.SequenceNumber);

            ClassicAssert.AreEqual(0x8F1A5F60, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(24, frame.FrameSize);
            ClassicAssert.AreEqual(681, frame.PayloadData.Length);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new DataDataFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }
    }