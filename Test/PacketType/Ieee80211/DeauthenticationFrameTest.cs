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
using PacketDotNet;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class DeauthenticationFrameTest
        {
            /// <summary>
            /// Test that parsing a disassociation frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_deauthentication_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                DeauthenticationFrame frame = (DeauthenticationFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementDeauthentication, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (248, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("0024B2F8D706", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("00173FB72C29", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (1312, frame.SequenceControl.SequenceNumber);
                Assert.AreEqual (ReasonCode.LeavingToRoam, frame.Reason);

                Assert.AreEqual (0xDD0A5D6B, frame.FrameCheckSequence);
                Assert.AreEqual (26, frame.FrameSize);
            }
            
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                DeauthenticationFrame frame = new DeauthenticationFrame (PhysicalAddress.Parse ("111111111111"),
                                                                         PhysicalAddress.Parse ("222222222222"),
                                                                         PhysicalAddress.Parse ("333333333333"));
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                 
                frame.SequenceControl.SequenceNumber = 0x77;
                frame.SequenceControl.FragmentNumber = 0x1;
               
                frame.Reason = ReasonCode.LeavingToRoam;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                DeauthenticationFrame recreatedFrame = MacFrame.ParsePacket (bas) as DeauthenticationFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ManagementDeauthentication, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                Assert.AreEqual (0x1234, recreatedFrame.Duration.Field);
                
                Assert.AreEqual (0x77, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                
                Assert.AreEqual (ReasonCode.LeavingToRoam, recreatedFrame.Reason);
                
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
				DeauthenticationFrame frame = new DeauthenticationFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
