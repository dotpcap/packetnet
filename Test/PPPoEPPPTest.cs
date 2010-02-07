using System;
using NUnit.Framework;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class PPPoEPPPTest
    {
        [Test]
        public void TestParsingPPPoePPPPacket()
        {
            var dev = new SharpPcap.OfflinePcapDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            Packet packet;

            // first packet is a udp packet
            rawPacket = dev.GetNextRawPacket();
            packet = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);
            var udpPacket = UdpPacket.GetType(packet);
            Assert.IsNotNull(udpPacket, "Expected a valid udp packet for the first packet");

            // second packet is the PPPoe Ptp packet
            rawPacket = dev.GetNextRawPacket();
            packet = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);
            var anotherUdpPacket = UdpPacket.GetType(packet);
            Assert.IsNotNull(anotherUdpPacket, "Expected a valid udp packet for the second packet as well");

            dev.Close();
        }
    }
}

