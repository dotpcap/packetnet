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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;

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
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_block_acknowledgment_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                BlockAcknowledgmentFrame frame = (BlockAcknowledgmentFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ControlBlockAcknowledgment, frame.FrameControl.Type);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsFalse(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsFalse(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("0024B2F8D706", frame.ReceiverAddress.ToString().ToUpper());
                Assert.AreEqual("7CC5376D16E7", frame.TransmitterAddress.ToString().ToUpper());

                Assert.AreEqual(BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, frame.BlockAcknowledgmentControl.Policy);
                Assert.IsFalse(frame.BlockAcknowledgmentControl.MultiTid);
                Assert.IsTrue(frame.BlockAcknowledgmentControl.CompressedBitmap);
                Assert.AreEqual(0, frame.BlockAcknowledgmentControl.Tid);
                Assert.AreEqual(0, frame.BlockAckStartingSequenceControl);
                Assert.AreEqual(8, frame.BlockAckBitmap.Length);

                Assert.AreEqual(0x9BB909C2, frame.FrameCheckSequence);
                Assert.AreEqual(28, frame.FrameSize);
            }
        } 
    }
}
