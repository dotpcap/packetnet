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
        public class AckFrameTest
        {
			
			[Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_ack_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                AckFrame frame = (AckFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ControlACK, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Protected);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("F8DB7F491342", frame.ReceiverAddress.ToString ().ToUpper ());
                Assert.AreEqual (0xD2F5BE07, frame.FrameCheckSequence);
                Assert.AreEqual (10, frame.FrameSize);
            }	
				
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                AckFrame frame = new AckFrame (PhysicalAddress.Parse ("111111111111"));
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                AckFrame recreatedFrame = MacFrame.ParsePacket (bas) as AckFrame;
                recreatedFrame.UpdateFrameCheckSequence ();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ControlACK, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual ("111111111111", recreatedFrame.ReceiverAddress.ToString ().ToUpper ());
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
			
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				Byte[] corruptBuffer = new Byte[]{0x01};
				AckFrame frame = new AckFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
