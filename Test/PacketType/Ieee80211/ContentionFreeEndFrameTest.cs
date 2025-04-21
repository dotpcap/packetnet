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
    public class ContentionFreeEndFrameTest
    {
        /// <summary>
        /// Test that parsing a contention free end frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_contention_free_end_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (ContentionFreeEndFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlCFEnd, frame.FrameControl.SubType);
            ClassicAssert.IsFalse(frame.FrameControl.ToDS);
            ClassicAssert.IsFalse(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsFalse(frame.FrameControl.Protected);
            ClassicAssert.IsTrue(frame.FrameControl.Order);
            ClassicAssert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("FFFFFFFFFFFF", frame.ReceiverAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("001B2FDCFC12", frame.BssId.ToString().ToUpper());

            ClassicAssert.AreEqual(0x0AE8A403, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(16, frame.FrameSize);
        }

        [Test]
        public void Test_Constructor_ConstructWithValues()
        {
            var frame = new ContentionFreeEndFrame(PhysicalAddress.Parse("111111111111"),
                                                   PhysicalAddress.Parse("222222222222"))
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                Duration = { Field = 0x1234 }
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as ContentionFreeEndFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlCFEnd, recreatedFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.FromDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.MoreFragments);
            ClassicAssert.AreEqual(0x1234, recreatedFrame.Duration.Field);

            ClassicAssert.AreEqual("111111111111", recreatedFrame.ReceiverAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("222222222222", recreatedFrame.BssId.ToString().ToUpper());

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new ContentionFreeEndFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }
    }