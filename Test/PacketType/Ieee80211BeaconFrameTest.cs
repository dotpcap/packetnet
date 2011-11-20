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
    class Ieee80211BeaconFrameTest
    {
        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_beacon_frame.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            Ieee80211BeaconFrame beaconFrame = (Ieee80211BeaconFrame)p.PayloadPacket;

            Assert.AreEqual(0, beaconFrame.FrameControl.ProtocolVersion);
            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementBeacon, beaconFrame.FrameControl.Type);
            Assert.IsFalse(beaconFrame.FrameControl.ToDS);
            Assert.IsFalse(beaconFrame.FrameControl.FromDS);
            Assert.IsFalse(beaconFrame.FrameControl.MoreFragments);
            Assert.IsFalse(beaconFrame.FrameControl.Retry);
            Assert.IsFalse(beaconFrame.FrameControl.PowerManagement);
            Assert.IsFalse(beaconFrame.FrameControl.MoreData);
            Assert.IsFalse(beaconFrame.FrameControl.Wep);
            Assert.IsFalse(beaconFrame.FrameControl.Order);
            Assert.AreEqual(0, beaconFrame.Duration.Field); //this need expanding on in the future
            Assert.AreEqual("FFFFFFFFFFFF", beaconFrame.DestinationAddress.ToString().ToUpper());
            Assert.AreEqual("0024B2F8D706", beaconFrame.SourceAddress.ToString().ToUpper());
            Assert.AreEqual("0024B2F8D706", beaconFrame.BssId.ToString().ToUpper());
            Assert.AreEqual(0, beaconFrame.SequenceControl.FragmentNumber);
            Assert.AreEqual(2892, beaconFrame.SequenceControl.SequenceNumber);
            Assert.AreEqual(0x000000A07A7BA566, beaconFrame.Timestamp);
            Assert.AreEqual(100, beaconFrame.BeaconInterval);
            Assert.IsTrue(beaconFrame.CapabilityInformation.IsEss);
            Assert.IsFalse(beaconFrame.CapabilityInformation.IsIbss);

            //Ignoring FCS for now as I haven't worked out how best to do that yet!
            Assert.AreEqual(0x2BADAF43, beaconFrame.FrameCheckSequence);
            Assert.AreEqual(262, beaconFrame.FrameSize);

            Console.WriteLine(p.ToString());
        }
    }
}
