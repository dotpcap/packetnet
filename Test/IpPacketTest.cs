using System;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class IpPacketTest
    {
        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void IpPacketFields()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            var rawPacket = dev.GetNextRawPacket();
            dev.Close();

            Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                          new PosixTimeval(rawPacket.Timeval.Seconds,
                                                           rawPacket.Timeval.MicroSeconds),
                                          rawPacket.Data);

            Assert.IsNotNull(p);

            var ip = IpPacket.GetType(p);
            Console.WriteLine(ip.GetType());

            Assert.AreEqual(20, ip.Header.Length, "Header.Length doesn't match expected length");
            Console.WriteLine(ip.ToString());
        }
    }
}
