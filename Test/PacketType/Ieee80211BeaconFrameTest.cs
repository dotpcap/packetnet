using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

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

        [Test]
        public void TestConstructWithValues()
        {
            //create a frame with some of the fields set to arbitrary values
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField();
            frameControl.Type = Ieee80211FrameControlField.FrameTypes.ManagementBeacon;
            frameControl.ToDS = true;
            frameControl.Wep = true;

            Ieee80211DurationField duration = new Ieee80211DurationField(12345);

            Ieee80211SequenceControlField sequenceControl = new Ieee80211SequenceControlField();
            sequenceControl.SequenceNumber = 3;

            Ieee80211CapabilityInformationField capabilityInfo = new Ieee80211CapabilityInformationField();
            //TODO: Cant set these properties yet
            //capabilityInfo.IsIbss = true;
            //capabilityInfo.Privacy = true;

            List<Ieee80211InformationElement> infoElements = new List<Ieee80211InformationElement>();
            infoElements.Add(new Ieee80211InformationElement(Ieee80211InformationElement.ElementId.ServiceSetIdentity, new Byte[]{0x68, 0x65, 0x6c, 0x6c, 0x6f}));

            Ieee80211BeaconFrame frame = new Ieee80211BeaconFrame(frameControl,
                 duration,
                 PhysicalAddress.Parse("11-11-11-11-11-11"),
                 PhysicalAddress.Parse("22-22-22-22-22-22"),
                 PhysicalAddress.Parse("33-33-33-33-33-33"),
                 sequenceControl,
                 123456789,
                 4444,
                 capabilityInfo,
                 infoElements);

            //serialize the frame into a byte buffer
            var bytes = frame.Bytes;
            var bas = new ByteArraySegment(bytes);

            //create a new frame that should be identical to the original
            Ieee80211BeaconFrame recreatedFrame = new Ieee80211BeaconFrame(bas);

            Assert.AreEqual(Ieee80211FrameControlField.FrameTypes.ManagementBeacon, recreatedFrame.FrameControl.Type);
            Assert.AreEqual(PhysicalAddress.Parse("11-11-11-11-11-11"), recreatedFrame.SourceAddress);
            Assert.AreEqual(PhysicalAddress.Parse("22-22-22-22-22-22"), recreatedFrame.DestinationAddress);
            Assert.AreEqual(PhysicalAddress.Parse("33-33-33-33-33-33"), recreatedFrame.BssId);
            Assert.IsTrue(recreatedFrame.FrameControl.ToDS);
            Assert.IsTrue(recreatedFrame.FrameControl.Wep);
            Assert.AreEqual(12345, recreatedFrame.Duration.Field);
            Assert.AreEqual(3, recreatedFrame.SequenceControl.SequenceNumber);
            Assert.AreEqual(123456789, recreatedFrame.Timestamp);
            Assert.AreEqual(4444, recreatedFrame.BeaconInterval);
            Assert.AreEqual(Ieee80211InformationElement.ElementId.ServiceSetIdentity, recreatedFrame.InformationElements.InformationElements[0].Id);
            Assert.AreEqual(5, recreatedFrame.InformationElements.InformationElements[0].Value.Length);
        }

    }
}
