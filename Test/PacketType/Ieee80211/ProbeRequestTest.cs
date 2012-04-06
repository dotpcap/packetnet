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
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class ProbeRequestTest
        {
            /// <summary>
            /// Test that parsing a probe request frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_probe_request_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                ProbeRequestFrame frame = (ProbeRequestFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementProbeRequest, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("FFFFFFFFFFFF", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0020008AB749", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("FFFFFFFFFFFF", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (234, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual (0xD83CB03D, frame.FrameCheckSequence);
                Assert.AreEqual (45, frame.FrameSize);
            }
            
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                InformationElement ssidInfoElement = new InformationElement (InformationElement.ElementId.ServiceSetIdentity, 
                                                                           new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f });
                InformationElement vendorElement = new InformationElement (InformationElement.ElementId.VendorSpecific,
                                                                           new Byte[] {0x01, 0x02, 0x03, 0x04, 0x05});
                
                
                ProbeRequestFrame frame = new ProbeRequestFrame (PhysicalAddress.Parse ("111111111111"),
                                                                 PhysicalAddress.Parse ("222222222222"),
                                                                 PhysicalAddress.Parse ("333333333333"),
                                                                new InformationElementList (){ssidInfoElement, vendorElement});
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                
                frame.SequenceControl.SequenceNumber = 0x77;
                frame.SequenceControl.FragmentNumber = 0x1;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);
    
                //create a new frame that should be identical to the original
                ProbeRequestFrame recreatedFrame = MacFrame.ParsePacket (bas) as ProbeRequestFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementProbeRequest, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual (0x77, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                
                Assert.AreEqual ("111111111111", recreatedFrame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("222222222222", recreatedFrame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("333333333333", recreatedFrame.BssId.ToString ().ToUpper ());
                
                Assert.AreEqual (ssidInfoElement, recreatedFrame.InformationElements [0]);
                Assert.AreEqual (vendorElement, recreatedFrame.InformationElements [1]);
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
			
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				ProbeRequestFrame frame = new ProbeRequestFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}

        } 
    }
}
