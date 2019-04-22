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

using System.Net.NetworkInformation;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class CtsFrameTest
    {
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_cts_frame.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            var frame = (CtsFrame) p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, frame.FrameControl.SubType);
            Assert.IsFalse(frame.FrameControl.ToDS);
            Assert.IsFalse(frame.FrameControl.FromDS);
            Assert.IsFalse(frame.FrameControl.MoreFragments);
            Assert.IsFalse(frame.FrameControl.Retry);
            Assert.IsFalse(frame.FrameControl.PowerManagement);
            Assert.IsFalse(frame.FrameControl.MoreData);
            Assert.IsFalse(frame.FrameControl.Protected);
            Assert.IsFalse(frame.FrameControl.Order);
            Assert.AreEqual(5653, frame.Duration.Field); //this need expanding on in the future
            Assert.AreEqual("001B2FDCFC12", frame.ReceiverAddress.ToString().ToUpper());
            Assert.AreEqual(0x405AC982, frame.FrameCheckSequence);
            Assert.AreEqual(10, frame.FrameSize);
        }

        [Test]
        public void Test_Constructor_ConstructWithValues()
        {
            var frame = new CtsFrame(PhysicalAddress.Parse("111111111111"))
            {
                FrameControl = { ToDS = false, FromDS = true, MoreFragments = true },
                Duration = { Field = 0x1234 }
            };

            frame.UpdateFrameCheckSequence();
            var fcs = frame.FrameCheckSequence;

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var byteArraySegment = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            var recreatedFrame = MacFrame.ParsePacket(byteArraySegment) as CtsFrame;
            recreatedFrame.UpdateFrameCheckSequence();

            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, recreatedFrame.FrameControl.SubType);
            Assert.IsFalse(recreatedFrame.FrameControl.ToDS);
            Assert.IsTrue(recreatedFrame.FrameControl.FromDS);
            Assert.IsTrue(recreatedFrame.FrameControl.MoreFragments);

            Assert.AreEqual("111111111111", recreatedFrame.ReceiverAddress.ToString().ToUpper());

            Assert.AreEqual(fcs, recreatedFrame.FrameCheckSequence);
        }

        [Test]
        public void Test_ConstructorWithCorruptBuffer()
        {
            //buffer is way too short for frame. We are just checking it doesn't throw
            byte[] corruptBuffer = { 0x01 };
            var frame = new CtsFrame(new ByteArraySegment(corruptBuffer));
            Assert.IsFalse(frame.FcsValid);
        }
    }
}