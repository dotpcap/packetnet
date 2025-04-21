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
    public class AssociationRequestFrameTest
    {
        /// <summary>
        /// Test that parsing a association request frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_association_request_frame.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var frame = (AssociationRequestFrame) p.PayloadPacket;

            ClassicAssert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementAssociationRequest, frame.FrameControl.SubType);
            ClassicAssert.IsFalse(frame.FrameControl.ToDS);
            ClassicAssert.IsFalse(frame.FrameControl.FromDS);
            ClassicAssert.IsFalse(frame.FrameControl.MoreFragments);
            ClassicAssert.IsFalse(frame.FrameControl.Retry);
            ClassicAssert.IsFalse(frame.FrameControl.PowerManagement);
            ClassicAssert.IsFalse(frame.FrameControl.MoreData);
            ClassicAssert.IsFalse(frame.FrameControl.Protected);
            ClassicAssert.IsFalse(frame.FrameControl.Order);
            ClassicAssert.AreEqual(248, frame.Duration.Field); //this need expanding on in the future
            ClassicAssert.AreEqual("0024B2F8D706", frame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("00173FB72C29", frame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("0024B2F8D706", frame.BssId.ToString().ToUpper());
            ClassicAssert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            ClassicAssert.AreEqual(1328, frame.SequenceControl.SequenceNumber);
            ClassicAssert.IsTrue(frame.CapabilityInformation.IsEss);
            ClassicAssert.IsFalse(frame.CapabilityInformation.IsIbss);
            ClassicAssert.IsFalse(frame.CapabilityInformation.CfPollable);
            ClassicAssert.IsFalse(frame.CapabilityInformation.CfPollRequest);
            ClassicAssert.IsTrue(frame.CapabilityInformation.Privacy);
            ClassicAssert.IsFalse(frame.CapabilityInformation.ShortPreamble);
            ClassicAssert.IsFalse(frame.CapabilityInformation.Pbcc);
            ClassicAssert.IsFalse(frame.CapabilityInformation.ChannelAgility);
            ClassicAssert.IsTrue(frame.CapabilityInformation.ShortTimeSlot);
            ClassicAssert.IsFalse(frame.CapabilityInformation.DssOfdm);
            ClassicAssert.AreEqual(0x05, frame.ListenInterval);
            ClassicAssert.AreEqual(0xDE82C216, frame.FrameCheckSequence);
            ClassicAssert.AreEqual(4, frame.InformationElements.Count);
            ClassicAssert.AreEqual(74, frame.FrameSize);
        }

        [Test]
        public void Test_Constructor_ConstructWithValues()
        {
            var ssidInfoElement = new InformationElement(InformationElement.ElementId.ServiceSetIdentity,
                                                         new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });

            var vendorElement = new InformationElement(InformationElement.ElementId.VendorSpecific,
                                                       new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 });

            var frame = new AssociationRequestFrame(PhysicalAddress.Parse("111111111111"),
                                                    PhysicalAddress.Parse("222222222222"),
                                                    PhysicalAddress.Parse("333333333333"),
                                                    new InformationElementList { ssidInfoElement, vendorElement })
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                Duration = { Field = 0x1234 },
                SequenceControl = { SequenceNumber = 0x77, FragmentNumber = 0x1 },
                CapabilityInformation = { Privacy = true, ChannelAgility = true }
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as AssociationRequestFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ManagementAssociationRequest, recreatedFrame.FrameControl.SubType);
            ClassicAssert.IsFalse(recreatedFrame.FrameControl.ToDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.FromDS);
            ClassicAssert.IsTrue(recreatedFrame.FrameControl.MoreFragments);

            ClassicAssert.AreEqual(0x77, recreatedFrame.SequenceControl.SequenceNumber);
            ClassicAssert.AreEqual(0x1, recreatedFrame.SequenceControl.FragmentNumber);

            ClassicAssert.IsTrue(recreatedFrame.CapabilityInformation.Privacy);
            ClassicAssert.IsTrue(recreatedFrame.CapabilityInformation.ChannelAgility);

            ClassicAssert.AreEqual("111111111111", recreatedFrame.SourceAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("222222222222", recreatedFrame.DestinationAddress.ToString().ToUpper());
            ClassicAssert.AreEqual("333333333333", recreatedFrame.BssId.ToString().ToUpper());

            ClassicAssert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new AssociationRequestFrame(new ByteArraySegment(corruptBuffer));
            ClassicAssert.IsFalse(frame.FcsValid);
        }
    }