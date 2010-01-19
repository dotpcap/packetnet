using System;
using NUnit.Framework;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class ConstructingPackets
    {
        /// <summary>
        /// Build a complete ethernet packet
        /// </summary>
        [Test]
        public void BuildEthernetPacket()
        {
            var tcpPacket = TcpPacket.RandomPacket();
            var ipPacket = IpPacket.RandomPacket(IpVersion.IPv4);
            var ethernetPacket = EthernetPacket.RandomPacket();

            // put these all together into a single packet
            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            Console.WriteLine("random packet: {0}", ethernetPacket.ToString());

            // and get a byte array that represents the single packet
            var bytes = ethernetPacket.Bytes;

            // and re-parse that packet
            var newPacket = Packet.ParsePacket(LinkLayers.Ethernet,
                                               new PosixTimeval(),
                                               bytes);

            Console.WriteLine("re-parsed random packet: {0}", newPacket.ToString());
        }
    }
}
