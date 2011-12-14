using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;
using PacketDotNet.Ieee80211;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        

        [TestFixture]
        public class BeaconFrameTest
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
                BeaconFrame beaconFrame = (BeaconFrame)p.PayloadPacket;

                Assert.AreEqual(0, beaconFrame.FrameControl.ProtocolVersion);
                Assert.AreEqual(FrameControlField.FrameTypes.ManagementBeacon, beaconFrame.FrameControl.Type);
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
                FrameControlField frameControl = new FrameControlField();
                frameControl.Type = FrameControlField.FrameTypes.ManagementBeacon;
                frameControl.ToDS = true;
                frameControl.Wep = true;

                DurationField duration = new DurationField(12345);

                SequenceControlField sequenceControl = new SequenceControlField();
                sequenceControl.SequenceNumber = 3;

                CapabilityInformationField capabilityInfo = new CapabilityInformationField();
                //TODO: Cant set these properties yet
                //capabilityInfo.IsIbss = true;
                //capabilityInfo.Privacy = true;

                List<InformationElement> infoElements = new List<InformationElement>();
                infoElements.Add(new InformationElement(InformationElement.ElementId.ServiceSetIdentity, new Byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f }));

                BeaconFrame frame = new BeaconFrame(frameControl,
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
                BeaconFrame recreatedFrame = new BeaconFrame(bas);

                Assert.AreEqual(FrameControlField.FrameTypes.ManagementBeacon, recreatedFrame.FrameControl.Type);
                Assert.AreEqual(PhysicalAddress.Parse("11-11-11-11-11-11"), recreatedFrame.SourceAddress);
                Assert.AreEqual(PhysicalAddress.Parse("22-22-22-22-22-22"), recreatedFrame.DestinationAddress);
                Assert.AreEqual(PhysicalAddress.Parse("33-33-33-33-33-33"), recreatedFrame.BssId);
                Assert.IsTrue(recreatedFrame.FrameControl.ToDS);
                Assert.IsTrue(recreatedFrame.FrameControl.Wep);
                Assert.AreEqual(12345, recreatedFrame.Duration.Field);
                Assert.AreEqual(3, recreatedFrame.SequenceControl.SequenceNumber);
                Assert.AreEqual(123456789, recreatedFrame.Timestamp);
                Assert.AreEqual(4444, recreatedFrame.BeaconInterval);
                Assert.AreEqual(InformationElement.ElementId.ServiceSetIdentity, recreatedFrame.InformationElements.InformationElements[0].Id);
                Assert.AreEqual(5, recreatedFrame.InformationElements.InformationElements[0].Value.Length);
            }

        } 
    }
}
