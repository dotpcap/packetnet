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
    class Ieee80211NullDataFrameTest
    {
        /// <summary>
        /// Test that parsing a null data frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_null_data_frame.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Ieee80211NullDataFrame frame = (Ieee80211NullDataFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.DataNullFunctionNoData, frame.FrameControl.Type);
            Assert.IsTrue(frame.FrameControl.ToDS);
            Assert.IsFalse(frame.FrameControl.FromDS);
            Assert.IsFalse(frame.FrameControl.MoreFragments);
            Assert.IsFalse(frame.FrameControl.Retry);
            Assert.IsFalse(frame.FrameControl.PowerManagement);
            Assert.IsFalse(frame.FrameControl.MoreData);
            Assert.IsFalse(frame.FrameControl.Wep);
            Assert.IsFalse(frame.FrameControl.Order);
            Assert.AreEqual(314, frame.Duration.Field); //this need expanding on in the future
            Assert.AreEqual("001B2F02DC6C", frame.DestinationAddress.ToString().ToUpper());
            Assert.AreEqual("2477030C721C", frame.SourceAddress.ToString().ToUpper());
            Assert.AreEqual("001B2F02DC6C", frame.BssId.ToString().ToUpper());
            Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            Assert.AreEqual(1245, frame.SequenceControl.SequenceNumber);

            Assert.AreEqual(0x8DAD458C, frame.FrameCheckSequence);
            Assert.AreEqual(24, frame.FrameSize);
        }
    }
}
