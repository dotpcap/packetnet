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
        public class ProbeResponseTest
        {
            /// <summary>
            /// Test that parsing a probe response frame yields the proper field values
            /// </summary>
            [Test]
            public void Test_Constructor()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_probe_response_frame.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                ProbeResponseFrame frame = (ProbeResponseFrame)p.PayloadPacket;

                Assert.AreEqual(0, frame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ManagementProbeResponse, frame.FrameControl.Type);
                Assert.IsFalse(frame.FrameControl.ToDS);
                Assert.IsFalse(frame.FrameControl.FromDS);
                Assert.IsFalse(frame.FrameControl.MoreFragments);
                Assert.IsTrue(frame.FrameControl.Retry);
                Assert.IsFalse(frame.FrameControl.PowerManagement);
                Assert.IsFalse(frame.FrameControl.MoreData);
                Assert.IsFalse(frame.FrameControl.Wep);
                Assert.IsFalse(frame.FrameControl.Order);
                Assert.AreEqual(314, frame.Duration.Field); //this need expanding on in the future
                Assert.AreEqual("0020008AB749", frame.DestinationAddress.ToString().ToUpper());
                Assert.AreEqual("00223FCD9C26", frame.SourceAddress.ToString().ToUpper());
                Assert.AreEqual("00223FCD9C26", frame.BssId.ToString().ToUpper());
                Assert.AreEqual(0, frame.SequenceControl.FragmentNumber);
                Assert.AreEqual(1468, frame.SequenceControl.SequenceNumber);

                Assert.AreEqual(0x0000047A44EF1DE0, frame.Timestamp);
                Assert.AreEqual(100, frame.BeaconInterval);

                Assert.IsTrue(frame.CapabilityInformation.IsEss);
                Assert.IsFalse(frame.CapabilityInformation.IsIbss);
                Assert.IsFalse(frame.CapabilityInformation.CfPollable);
                Assert.IsFalse(frame.CapabilityInformation.CfPollRequest);
                Assert.IsTrue(frame.CapabilityInformation.Privacy);
                Assert.IsFalse(frame.CapabilityInformation.ShortPreamble);
                Assert.IsFalse(frame.CapabilityInformation.Pbcc);
                Assert.IsFalse(frame.CapabilityInformation.ChannelAgility);
                Assert.IsTrue(frame.CapabilityInformation.ShortTimeSlot);
                Assert.IsFalse(frame.CapabilityInformation.DssOfdm);

                Assert.AreEqual(0x257202BE, frame.FrameCheckSequence);
                Assert.AreEqual(164, frame.FrameSize);
            }
        } 
    }
}
