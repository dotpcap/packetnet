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
using System.Net.NetworkInformation;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        

        [TestFixture]
        public class BeaconFrameTest
        {
            /// <summary>
            /// Test that parsing an ip packet yields the proper field values
            /// </summary>
            [Test]
            public void ReadingPacketsFromFile ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_beacon_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                BeaconFrame beaconFrame = (BeaconFrame)p.PayloadPacket;

                Assert.AreEqual (0, beaconFrame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementBeacon, beaconFrame.FrameControl.SubType);
                Assert.IsFalse (beaconFrame.FrameControl.ToDS);
                Assert.IsFalse (beaconFrame.FrameControl.FromDS);
                Assert.IsFalse (beaconFrame.FrameControl.MoreFragments);
                Assert.IsFalse (beaconFrame.FrameControl.Retry);
                Assert.IsFalse (beaconFrame.FrameControl.PowerManagement);
                Assert.IsFalse (beaconFrame.FrameControl.MoreData);
                Assert.IsFalse (beaconFrame.FrameControl.Wep);
                Assert.IsFalse (beaconFrame.FrameControl.Order);
                Assert.AreEqual (0, beaconFrame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("FFFFFFFFFFFF", beaconFrame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", beaconFrame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", beaconFrame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, beaconFrame.SequenceControl.FragmentNumber);
                Assert.AreEqual (2892, beaconFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x000000A07A7BA566, beaconFrame.Timestamp);
                Assert.AreEqual (100, beaconFrame.BeaconInterval);
                Assert.IsTrue (beaconFrame.CapabilityInformation.IsEss);
                Assert.IsFalse (beaconFrame.CapabilityInformation.IsIbss);
                
                Assert.AreEqual (15, beaconFrame.InformationElements.Count);

                Assert.AreEqual (0x2BADAF43, beaconFrame.FrameCheckSequence);
                Assert.AreEqual (262, beaconFrame.FrameSize);
            }

            [Test]
            public void TestConstructWithValues ()
            {
            
                InformationElement ssidInfoElement = new InformationElement (InformationElement.ElementId.ServiceSetIdentity, new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f }); 
                
                BeaconFrame frame = new BeaconFrame (
                     PhysicalAddress.Parse ("11-11-11-11-11-11"),
                     PhysicalAddress.Parse ("22-22-22-22-22-22"),
                    new InformationElementList (){ssidInfoElement});

                frame.FrameControl.ToDS = true;
                frame.FrameControl.Wep = true;
                frame.Duration.Field = 12345;
                frame.SequenceControl.SequenceNumber = 3;
                frame.Timestamp = 123456789;
                frame.BeaconInterval = 4444;
                frame.CapabilityInformation.IsIbss = true;
                frame.CapabilityInformation.Privacy = true;
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                BeaconFrame recreatedFrame = MacFrame.ParsePacket (bas) as BeaconFrame;
                recreatedFrame.UpdateFrameCheckSequence();

                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementBeacon, recreatedFrame.FrameControl.SubType);
                Assert.AreEqual (PhysicalAddress.Parse ("11-11-11-11-11-11"), recreatedFrame.SourceAddress);
                Assert.AreEqual (PhysicalAddress.Parse ("FF-FF-FF-FF-FF-FF"), recreatedFrame.DestinationAddress);
                Assert.AreEqual (PhysicalAddress.Parse ("22-22-22-22-22-22"), recreatedFrame.BssId);
                Assert.IsTrue (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.Wep);
                Assert.AreEqual (12345, recreatedFrame.Duration.Field);
                Assert.AreEqual (3, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (123456789, recreatedFrame.Timestamp);
                Assert.AreEqual (4444, recreatedFrame.BeaconInterval);
                Assert.AreEqual (ssidInfoElement, recreatedFrame.InformationElements [0]);
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
   
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				BeaconFrame frame = new BeaconFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
