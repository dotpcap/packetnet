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
    class Ieee80211QosNullDataFrameTest
    {
        /// <summary>
        /// Test that parsing a QOS null data frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_qos_null_data_frame.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Ieee80211QosNullDataFrame frame = (Ieee80211QosNullDataFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.QosNullData, frame.FrameControl.Type);
            Assert.IsFalse(frame.FrameControl.ToDS);
            Assert.IsTrue(frame.FrameControl.FromDS);
            Assert.IsFalse(frame.FrameControl.MoreFragments);
            Assert.IsFalse(frame.FrameControl.Retry);
            Assert.IsFalse(frame.FrameControl.PowerManagement);
            Assert.IsFalse(frame.FrameControl.MoreData);
            Assert.IsFalse(frame.FrameControl.Wep);
            Assert.IsFalse(frame.FrameControl.Order);
            Assert.AreEqual(314, frame.Duration.Field);
            Assert.AreEqual("78ACC00A5E0E", frame.DestinationAddress.ToString().ToUpper());
            Assert.AreEqual("00223FC1F378", frame.BssId.ToString().ToUpper());
            Assert.AreEqual("00223FC1F378", frame.SourceAddress.ToString().ToUpper());
            Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            Assert.AreEqual(2292, frame.SequenceControl.SequenceNumber);
            Assert.AreEqual(0x00, frame.QosControl);
            Assert.AreEqual(0xDBF2B119, frame.FrameCheckSequence);
            Assert.AreEqual(26, frame.FrameSize);
        }
    }
}
