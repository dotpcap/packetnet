using System;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test
{
    [TestFixture]
    public class TcpPacketTest
    {
        [Test]
        public void CreationAndReparsing()
        {
            var tcpPacket = TcpPacket.RandomPacket();

            var tcpPacket2 = new TcpPacket(tcpPacket.Bytes, 0);

            Console.WriteLine("tcpPacket {0}", tcpPacket);
            Console.WriteLine("tcpPacket2 {0}", tcpPacket2);
        }

        [Test]
        public void ByteLengthInternalLengthMismatch()
        {
            var dev = new PcapOfflineDevice("../../CaptureFiles/tcp_with_extra_bytes.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            rawPacket = dev.GetNextRawPacket();
            var p = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            Assert.IsTrue(TcpPacket.IsType(p));
            var t = TcpPacket.GetType(p);

            // even though the packet has 6 bytes of extra data, the ip packet shows a size of
            // 40 and the ip header has a length of 20. The TCP header is also 20 bytes so
            // there should be zero bytes in the TCPData value
            int expectedTcpDataLength = 0;
            Assert.AreEqual(expectedTcpDataLength, t.PayloadData.Length);

            dev.Close();
        }

        [Test]
        public virtual void Checksum()
        {
            var dev = new PcapOfflineDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            rawPacket = dev.GetNextRawPacket();
            var p = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            Assert.IsTrue(TcpPacket.IsType(p), "p is not a TcpPacket");

            var t = TcpPacket.GetType(p);
            Console.WriteLine("Checksum: " + t.Checksum.ToString("X"));
            Assert.IsTrue(t.ValidChecksum, "ValidChecksum indicates invalid checksum");

            dev.Close();
        }

        [Test]
        public void PayloadModification()
        {
            String s = "-++++=== HELLLLOOO ===++++-";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random pkt
            var p = TcpPacket.RandomPacket();

            //replace pkt's data with our string
            p.PayloadData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.PayloadData));
        }

        [Test]
        public void TCPOptions()
        {
            var dev = new PcapOfflineDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            rawPacket = dev.GetNextRawPacket();
            var p = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);

            Assert.IsNotNull(p);

            Assert.IsTrue(TcpPacket.IsType(p));
            var t = TcpPacket.GetType(p);

            // verify that the options byte match what we expect
            byte[] expectedOptions = new byte[] { 0x1, 0x1, 0x8, 0xa, 0x0, 0x14,
                                                  0x3d, 0xe5, 0x1d, 0xf5, 0xf8, 0x84 };
            Assert.AreEqual(expectedOptions, t.Options);

            dev.Close();
        }

        [Test]
        public void TCPConstructorFromValues()
        {
            ushort sourcePort = 100;
            ushort destinationPort = 101;
            var tcpPacket = new TcpPacket(sourcePort, destinationPort);

            Assert.AreEqual(sourcePort, tcpPacket.SourcePort);
            Assert.AreEqual(destinationPort, tcpPacket.DestinationPort);
        }

        [Test]
        public void RandomPacket()
        {
            TcpPacket.RandomPacket();
        }
    }
}