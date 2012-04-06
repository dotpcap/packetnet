/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap.LibPcap;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class AssociationResponseFrameTest
        {
            /// <summary>
            /// Test that parsing a association response frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_association_response_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                AssociationResponseFrame frame = (AssociationResponseFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementAssociationResponse, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (314, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("00173FB72C29", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (958, frame.SequenceControl.SequenceNumber);
                Assert.IsTrue (frame.CapabilityInformation.IsEss);
                Assert.IsFalse (frame.CapabilityInformation.IsIbss);
                Assert.IsFalse (frame.CapabilityInformation.CfPollable);
                Assert.IsFalse (frame.CapabilityInformation.CfPollRequest);
                Assert.IsTrue (frame.CapabilityInformation.Privacy);
                Assert.IsFalse (frame.CapabilityInformation.ShortPreamble);
                Assert.IsFalse (frame.CapabilityInformation.Pbcc);
                Assert.IsFalse (frame.CapabilityInformation.ChannelAgility);
                Assert.IsTrue (frame.CapabilityInformation.ShortTimeSlot);
                Assert.IsFalse (frame.CapabilityInformation.DssOfdm);
                Assert.AreEqual (AuthenticationStatusCode.Success, frame.StatusCode);
                Assert.AreEqual (2, frame.AssociationId);
                Assert.AreEqual (0xC61ACCD6, frame.FrameCheckSequence);
                Assert.AreEqual (3, frame.InformationElements.Count);
                Assert.AreEqual (57, frame.FrameSize);
            }
            
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                InformationElement ssidInfoElement = new InformationElement (InformationElement.ElementId.ServiceSetIdentity, 
                                                                           new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });
                InformationElement vendorElement = new InformationElement (InformationElement.ElementId.VendorSpecific,
                                                                           new Byte[] {0x01, 0x02, 0x03, 0x04, 0x05});
                
                
                AssociationResponseFrame frame = new AssociationResponseFrame (PhysicalAddress.Parse ("111111111111"),
                                                                             PhysicalAddress.Parse ("222222222222"),
                                                                             PhysicalAddress.Parse ("333333333333"),
                                                                             new InformationElementList (){ssidInfoElement, vendorElement});
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                
                frame.SequenceControl.SequenceNumber = 0x77;
                frame.SequenceControl.FragmentNumber = 0x1;
                
                frame.CapabilityInformation.Privacy = true;
                frame.CapabilityInformation.ChannelAgility = true;
                
                frame.StatusCode = AuthenticationStatusCode.Success;
                frame.AssociationId = 0x2;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                AssociationResponseFrame recreatedFrame = MacFrame.ParsePacket (bas) as AssociationResponseFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementAssociationResponse, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual (0x77, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                
                Assert.IsTrue (recreatedFrame.CapabilityInformation.Privacy);
                Assert.IsTrue (recreatedFrame.CapabilityInformation.ChannelAgility);
                
                Assert.AreEqual (AuthenticationStatusCode.Success, recreatedFrame.StatusCode);
                Assert.AreEqual (0x2, recreatedFrame.AssociationId);
                
                Assert.AreEqual ("111111111111", recreatedFrame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("222222222222", recreatedFrame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("333333333333", recreatedFrame.BssId.ToString ().ToUpper ());
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
			
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				AssociationResponseFrame frame = new AssociationResponseFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
