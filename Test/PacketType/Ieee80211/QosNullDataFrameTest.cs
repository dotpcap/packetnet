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
    public class QosNullDataFrameTest
    {
        /// <summary>
        /// Test that parsing a QOS null data frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_qos_null_data_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (QosNullDataFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.QosNullData, frame.FrameControl.SubType);
            ClassicAssert.IsFalse(frame.FrameControl.ToDS);
            ClassicAssert.IsTrue(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsFalse(frame.FrameControl.Protected);
            ClassicAssert.IsFalse(frame.FrameControl.Order);
            ClassicAssert.AreEqual(314, frame.Duration.Field);
            ClassicAssert.AreEqual("78ACC00A5E0E", frame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("00223FC1F378", frame.BssId.ToString().ToUpper());
            ClassicAssert.AreEqual("00223FC1F378", frame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            ClassicAssert.AreEqual(2292, frame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(0x00, frame.QosControl);
            ClassicAssert.AreEqual(0xDBF2B119, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(26, frame.FrameSize);
        }

        [Test]
        public void Test_Constructor_ConstructWithValues()
        {
            var frame = new QosNullDataFrame
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                SequenceControl = { SequenceNumber = 0x89, FragmentNumber = 0x1 },
                Duration = { Field = 0x1234 },
                QosControl = 0x9876,
                DestinationAddress = PhysicalAddress.Parse("111111111111"),
                SourceAddress = PhysicalAddress.Parse("222222222222"),
                BssId = PhysicalAddress.Parse("333333333333"),
                PayloadData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
                AppendFcs = true
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacketWithFcs(byteArraySegment) as QosNullDataFrame;

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.QosNullData, recreatedFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.FromDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.MoreFragments);

            ClassicAssert.AreEqual(0x89, recreatedFrame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(0x1, recreatedFrame.SequenceControl.FragmentNumber);

            ClassicAssert.AreEqual(0x9876, recreatedFrame.QosControl);

            ClassicAssert.AreEqual("111111111111", recreatedFrame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("222222222222", recreatedFrame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("333333333333", recreatedFrame.BssId.ToString().ToUpper());

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new QosNullDataFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }
    }