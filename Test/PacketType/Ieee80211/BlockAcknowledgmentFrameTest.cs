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
        public class BlockAcknowledgmentFrameTest
        {
            /// <summary>
            /// Test that parsing a block acknowledgment frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_block_acknowledgment_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                BlockAcknowledgmentFrame frame = (BlockAcknowledgmentFrame)p.PayloadPacket;

                Assert.AreEqual (0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual (FrameControlField.FrameSubTypes.ControlBlockAcknowledgment, frame.FrameControl.SubType);
                Assert.IsFalse (frame.FrameControl.ToDS);
                Assert.IsFalse (frame.FrameControl.FromDS);
                Assert.IsFalse (frame.FrameControl.MoreFragments);
                Assert.IsFalse (frame.FrameControl.Retry);
                Assert.IsFalse (frame.FrameControl.PowerManagement);
                Assert.IsFalse (frame.FrameControl.MoreData);
                Assert.IsFalse (frame.FrameControl.Wep);
                Assert.IsFalse (frame.FrameControl.Order);
                Assert.AreEqual (0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual ("0024B2F8D706", frame.ReceiverAddress.ToString ().ToUpper ());
                Assert.AreEqual ("7CC5376D16E7", frame.TransmitterAddress.ToString ().ToUpper ());

                Assert.AreEqual (BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, frame.BlockAcknowledgmentControl.Policy);
                Assert.IsFalse (frame.BlockAcknowledgmentControl.MultiTid);
                Assert.IsTrue (frame.BlockAcknowledgmentControl.CompressedBitmap);
                Assert.AreEqual (0, frame.BlockAcknowledgmentControl.Tid);
                Assert.AreEqual (0, frame.BlockAckStartingSequenceControl);
                Assert.AreEqual (8, frame.BlockAckBitmap.Length);

                Assert.AreEqual (0x9BB909C2, frame.FrameCheckSequence);
                Assert.AreEqual (28, frame.FrameSize);
            }
            
            [Test]
            public void Test_Constructor_ConstructWithValues ()
            {
                Byte[] BlockAckBitmap = new Byte[]{0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8};
                
                BlockAcknowledgmentFrame frame = new BlockAcknowledgmentFrame (PhysicalAddress.Parse ("111111111111"),
                                                                               PhysicalAddress.Parse ("222222222222"),
                                                                               BlockAckBitmap);
                
                frame.FrameControl.ToDS = false;
                frame.FrameControl.FromDS = true;
                frame.FrameControl.MoreFragments = true;
                
                frame.Duration.Field = 0x1234;
                
                frame.BlockAcknowledgmentControl.Policy = BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed;
                frame.BlockAcknowledgmentControl.Tid = 0xF;
                
                frame.BlockAckStartingSequenceControl = 0x5678;
                
                frame.UpdateFrameCheckSequence ();
                UInt32 fcs = frame.FrameCheckSequence;
                
                //serialize the frame into a byte buffer
                var bytes = frame.Bytes;
                var bas = new ByteArraySegment (bytes);

                //create a new frame that should be identical to the original
                BlockAcknowledgmentFrame recreatedFrame = MacFrame.ParsePacket (bas) as BlockAcknowledgmentFrame;
                recreatedFrame.UpdateFrameCheckSequence();
                
                Assert.AreEqual (FrameControlField.FrameSubTypes.ControlBlockAcknowledgment, recreatedFrame.FrameControl.SubType);
                Assert.IsFalse (recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue (recreatedFrame.FrameControl.FromDS);
                Assert.IsTrue (recreatedFrame.FrameControl.MoreFragments);
                Assert.AreEqual (0x1234, recreatedFrame.Duration.Field);
                Assert.AreEqual (BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed,
                                 recreatedFrame.BlockAcknowledgmentControl.Policy);
                
                Assert.AreEqual (0xF, recreatedFrame.BlockAcknowledgmentControl.Tid);
                Assert.IsTrue (recreatedFrame.BlockAcknowledgmentControl.CompressedBitmap);
                Assert.AreEqual (0x5678, recreatedFrame.BlockAckStartingSequenceControl);
                
                Assert.AreEqual ("111111111111", recreatedFrame.TransmitterAddress.ToString ().ToUpper ());
                Assert.AreEqual ("222222222222", recreatedFrame.ReceiverAddress.ToString ().ToUpper ());
                
                CollectionAssert.AreEqual (BlockAckBitmap, recreatedFrame.BlockAckBitmap);
                
                Assert.AreEqual (fcs, recreatedFrame.FrameCheckSequence);
            }
                
			[Test]
			public void Test_ConstructorWithCorruptBuffer ()
			{
				//buffer is way too short for frame. We are just checking it doesn't throw
				byte[] corruptBuffer = new byte[]{0x01};
				BlockAcknowledgmentFrame frame = new BlockAcknowledgmentFrame(new ByteArraySegment(corruptBuffer));
				Assert.IsFalse(frame.FCSValid);
			}
        } 
    }
}
