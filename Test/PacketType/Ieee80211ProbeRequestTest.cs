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
    class Ieee80211ProbeRequestTest
    {
        /// <summary>
        /// Test that parsing a probe request frame yields the proper field values
        /// </summary>
        [Test]
        public void Test_Constructor()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_probe_request_frame.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Ieee80211ProbeRequestFrame frame = (Ieee80211ProbeRequestFrame)p.PayloadPacket;

            Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementProbeRequest, frame.FrameControl.Type);
            Assert.IsFalse(frame.FrameControl.ToDS);
            Assert.IsFalse(frame.FrameControl.FromDS);
            Assert.IsFalse(frame.FrameControl.MoreFragments);
            Assert.IsFalse(frame.FrameControl.Retry);
            Assert.IsFalse(frame.FrameControl.PowerManagement);
            Assert.IsFalse(frame.FrameControl.MoreData);
            Assert.IsFalse(frame.FrameControl.Wep);
            Assert.IsFalse(frame.FrameControl.Order);
            Assert.AreEqual(0, frame.Duration.Field); //this need expanding on in the future
            Assert.AreEqual("FFFFFFFFFFFF", frame.DestinationAddress.ToString().ToUpper());
            Assert.AreEqual("0020008AB749", frame.SourceAddress.ToString().ToUpper());
            Assert.AreEqual("FFFFFFFFFFFF", frame.BssId.ToString().ToUpper());
            Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
            Assert.AreEqual(234, frame.SequenceControl.SequenceNumber);

            Assert.AreEqual(0xD83CB03D, frame.FrameCheckSequence);
            Assert.AreEqual(45, frame.FrameSize);
        }

    }
}
