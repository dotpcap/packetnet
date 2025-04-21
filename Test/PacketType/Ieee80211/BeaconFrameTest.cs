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
    public class BeaconFrameTest
    {
        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_beacon_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var beaconFrame = (BeaconFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, beaconFrame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementBeacon, beaconFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.ToDS);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.FromDS);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.Retry);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.MoreData);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.Protected);
            ClassicAssert.IsFalse(beaconFrame.FrameControl.Order);
            ClassicAssert.AreEqual(0, beaconFrame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("FFFFFFFFFFFF", beaconFrame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("0024B2F8D706", beaconFrame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("0024B2F8D706", beaconFrame.BssId.ToString().ToUpper());
            ClassicAssert.AreEqual(0, beaconFrame.SequenceControl.FragmentNumber);
            ClassicAssert.AreEqual(2892, beaconFrame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(0x000000A07A7BA566, beaconFrame.Timestamp);
            ClassicAssert.AreEqual(100, beaconFrame.BeaconInterval);
            ClassicAssert.IsTrue(beaconFrame.CapabilityInformation.IsEss);
            ClassicAssert.IsFalse(beaconFrame.CapabilityInformation.IsIbss);

            ClassicAssert.AreEqual(15, beaconFrame.InformationElements.Count);

            ClassicAssert.AreEqual(0x2BADAF43, beaconFrame.FrameCheckSequence);
            ClassicAssert.AreEqual(262, beaconFrame.FrameSize);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new BeaconFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }

        [Test]
        public void TestConstructWithValues()
        {
            var ssidInfoElement = new InformationElement(InformationElement.ElementId.ServiceSetIdentity, new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });

            var frame = new BeaconFrame(
                                        PhysicalAddress.Parse("11-11-11-11-11-11"),
                                        PhysicalAddress.Parse("22-22-22-22-22-22"),
                                        new InformationElementList { ssidInfoElement })
            {
                FrameControl = { ToDS = true, Protected = true },
                Duration = { Field = 12345 },
                SequenceControl = { SequenceNumber = 3 },
                Timestamp = 123456789,
                BeaconInterval = 4444,
                CapabilityInformation = { IsIbss = true, Privacy = true }
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as BeaconFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementBeacon, recreatedFrame.FrameControl.SubType);
            ClassicAssert.AreEqual(PhysicalAddress.Parse("11-11-11-11-11-11"), recreatedFrame.SourceAddress);
            ClassicAssert.AreEqual(PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), recreatedFrame.DestinationAddress);
            ClassicAssert.AreEqual(PhysicalAddress.Parse("22-22-22-22-22-22"), recreatedFrame.BssId);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.Protected);
            ClassicAssert.AreEqual(12345, recreatedFrame.Duration.Field);
            ClassicAssert.AreEqual(3, recreatedFrame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(123456789, recreatedFrame.Timestamp);
            ClassicAssert.AreEqual(4444, recreatedFrame.BeaconInterval);
            ClassicAssert.AreEqual(ssidInfoElement, recreatedFrame.InformationElements[0]);

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }
    }