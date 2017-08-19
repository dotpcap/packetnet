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
using SharpPcap.LibPcap;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class QosDataFrameTest
        {
            /// <summary>
            /// Test that parsing a QOS data frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_qos_data_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                QosDataFrame frame = (QosDataFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.QosData, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsTrue (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsTrue (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (44, frame.Duration.Field);
                Assert.AreEqual ("7CC5376D16E7", frame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.BssId.ToString ().ToUpper ());
                Assert.AreEqual ("0024B2F8D706", frame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual (0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual (2, frame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x00, frame.QosControl);
                Assert.AreEqual (0x87311A87, frame.FrameCheckSequence);
                Assert.AreEqual (26, frame.FrameSize);

                Assert.AreEqual (0x34, frame.PayloadData.Length);
            }

            /// <summary>
            /// Test that a QosData frame containing a tcp packet can be properly parsed
            /// </summary>
            [Test]
            public void Test_QosDataFrameParsingWithIpV4Tcp()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_qos_data_frame_ipv4_tcp.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;

                // test that we can access the lowest level tcp packet
                var t = p.Extract(typeof(TcpPacket));
                Assert.IsNotNull(t, "Expected t not to be null");

                Console.WriteLine(p.ToString(StringOutputType.Verbose));
            }

            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                QosDataFrame frame = new QosDataFrame ();
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.SequenceControl.SequenceNumber = 0x89;
                frame.SequenceControl.FragmentNumber = 0x1;
                
                frame.Duration.Field = 0x1234;
                
                frame.QosControl = 0x9876;
                
                frame.DestinationAddress = PhysicalAddress.Parse ("111111111111");
                frame.SourceAddress = PhysicalAddress.Parse ("222222222222");
                frame.BssId = PhysicalAddress.Parse ("333333333333");
                
                frame.PayloadData = new byte[]{0x01, 0x02, 0x03, 0x04, 0x05};
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                QosDataFrame recreatedFrame = MacFrame.ParsePacket (bas) as QosDataFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.QosData, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                
                Assert.AreEqual (0x89, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual (0x1, recreatedFrame.SequenceControl.FragmentNumber);
                Assert.AreEqual (0x9876, recreatedFrame.QosControl);
                
                Assert.AreEqual ("111111111111", recreatedFrame.DestinationAddress.ToString ().ToUpper ());
                Assert.AreEqual ("222222222222", recreatedFrame.SourceAddress.ToString ().ToUpper ());
                Assert.AreEqual ("333333333333", recreatedFrame.BssId.ToString ().ToUpper ());
                
                CollectionAssert.AreEqual (new byte[]{0x01, 0x02, 0x03, 0x04, 0x05}, recreatedFrame.PayloadData);
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
			
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				QosDataFrame frame = new QosDataFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
