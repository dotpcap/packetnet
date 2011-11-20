using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    class Ieee80211BlockAcknowledgmentFrameTest
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
            Ieee80211BlockAcknowledgmentFrame frame = (Ieee80211BlockAcknowledgmentFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ControlBlockAcknowledgment, frame.FrameControl.Type);
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

            Assert.AreEqual(Ieee80211BlockAcknowledgmentControlField.AcknowledgementPolicy.Delayed, frame.BlockAcknowledgmentControl.Policy);
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
