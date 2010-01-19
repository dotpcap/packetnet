using System;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test
{
    [TestFixture]
    public class IPv4PacketTest
    {
        [Test]
        public void ConstructingFromValues()
        {
            var sourceAddress = RandomUtils.GetIPAddress(IpVersion.IPv4);
            var destinationAddress = RandomUtils.GetIPAddress(IpVersion.IPv4);
            var ip = new IPv4Packet(sourceAddress, destinationAddress);

            Assert.AreEqual(sourceAddress, ip.SourceAddress);
            Assert.AreEqual(destinationAddress, ip.DestinationAddress);
        }

        [Test]
        public void RandomPacket()
        {
            IPv4Packet.RandomPacket();
        }
    }
}
