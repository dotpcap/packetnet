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
        public class ReassociationRequestFrameTest
        {
			
			[Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_reassociation_request_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                ReassociationRequestFrame frame = (ReassociationRequestFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementReassociationRequest, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (314, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("0026F23E46C1", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("7CC5376C970A", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0026F23E46C1", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (2729, frame.SequenceControl.SequenceNumber);
                Assert.IsTrue (frame.CapabilityInformation.IsEss);
                Assert.IsFalse (frame.CapabilityInformation.IsIbss);
                Assert.IsFalse (frame.CapabilityInformation.CfPollable);
                Assert.IsFalse (frame.CapabilityInformation.CfPollRequest);
                Assert.IsTrue (frame.CapabilityInformation.Privacy);
                Assert.IsTrue (frame.CapabilityInformation.ShortPreamble);
                Assert.IsFalse (frame.CapabilityInformation.Pbcc);
                Assert.IsFalse (frame.CapabilityInformation.ChannelAgility);
                Assert.IsTrue (frame.CapabilityInformation.ShortTimeSlot);
                Assert.IsFalse (frame.CapabilityInformation.DssOfdm);
                Assert.AreEqual (0x0a, frame.ListenInterval);
                Assert.AreEqual (0xFCBB7306, frame.FrameCheckSequence);
                Assert.AreEqual (4, frame.InformationElements.Count);
                Assert.AreEqual (74, frame.FrameSize);
            }	
				
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                InformationElement ssidInfoElement = new InformationElement (InformationElement.ElementId.ServiceSetIdentity, 
                                                                           new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });
                InformationElement vendorElement = new InformationElement (InformationElement.ElementId.VendorSpecific,
                                                                           new Byte[] {0x01, 0x02, 0x03, 0x04, 0x05});
                
                
                ReassociationRequestFrame frame = new ReassociationRequestFrame (PhysicalAddress.Parse ("111111111111"),
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
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                ReassociationRequestFrame recreatedFrame = MacFrame.ParsePacket (bas) as ReassociationRequestFrame;
                recreatedFrame.UpdateFrameCheckSequence ();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementReassociationRequest, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual (0x77, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                
                Assert.IsTrue (recreatedFrame.CapabilityInformation.Privacy);
                Assert.IsTrue (recreatedFrame.CapabilityInformation.ChannelAgility);
                
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
				ReassociationRequestFrame frame = new ReassociationRequestFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
