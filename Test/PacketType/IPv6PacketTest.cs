/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class IPv6PacketTest
    {
        // icmpv6
        public void VerifyPacket0(Packet p, RawCapture rawCapture)
        {
            Assert.IsNotNull(p);
            Console.WriteLine(p.ToString());

            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(PhysicalAddress.Parse("00-A0-CC-D9-41-75"), e.SourceHwAddress);
            Assert.AreEqual(PhysicalAddress.Parse("33-33-00-00-00-02"), e.DestinationHwAddress);

            var ip = (IpPacket)p.Extract (typeof(IpPacket));
            Console.WriteLine("ip {0}", ip.ToString());
            Assert.AreEqual(System.Net.IPAddress.Parse("fe80::2a0:ccff:fed9:4175"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("ff02::2"), ip.DestinationAddress);
            Assert.AreEqual(IpVersion.IPv6, ip.Version);
            Assert.AreEqual(IPProtocolType.ICMPV6, ip.Protocol);
            Assert.AreEqual(16,  ip.PayloadPacket.Bytes.Length, "ip.PayloadPacket.Bytes.Length mismatch");
            Assert.AreEqual(255, ip.HopLimit);
            Assert.AreEqual(255, ip.TimeToLive);
            Assert.AreEqual(0x3a, (byte)ip.NextHeader);
            Console.WriteLine("Failed: ip.ComputeIPChecksum() not implemented.");
            Assert.AreEqual(1221145299, rawCapture.Timeval.Seconds);
            Assert.AreEqual(453568.000, rawCapture.Timeval.MicroSeconds);
        }

        // Test that we can load and parse an IPv6 packet
        [Test]
        public void IPv6PacketTestParsing()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();

            RawCapture rawCapture;
            int packetIndex = 0;
            while((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                Console.WriteLine("got packet");
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p, rawCapture);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }

        /// <summary>
        /// Test that we can load and parse an IPv6 TCP packet and that
        /// the computed tcp checksum matches the expected checksum
        /// </summary>
        [Test]
        public void TCPChecksumIPv6()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_http.pcap");
            dev.Open();

            // checksums from wireshark of the capture file
            int[] expectedChecksum = {0x41a2,
                                      0x4201,
                                      0x5728,
                                      0xf448,
                                      0xee07,
                                      0x939c,
                                      0x63e4,
                                      0x4590,
                                      0x3725,
                                      0x3723};

            int packetIndex = 0;
            RawCapture rawCapture;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                var t = (TcpPacket)p.Extract(typeof(TcpPacket));
                Assert.IsNotNull(t, "Expected t to not be null");
                Assert.IsTrue(t.ValidChecksum, "t.ValidChecksum isn't true");

                // compare the computed checksum to the expected one
                Assert.AreEqual(expectedChecksum[packetIndex],
                                t.CalculateTCPChecksum(),
                                "Checksum mismatch");

                packetIndex++;
            }

            dev.Close();
        }

        // Test that we can correctly set the data section of a IPv6 packet
        [Test]
        public void TCPDataIPv6()
        {
            String s = "-++++=== HELLLLOOO ===++++-";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random packet
            TcpPacket p = TcpPacket.RandomPacket();

            //replace pkt's data with our string
            p.PayloadData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.PayloadData));
        }

        [Test]
        public void ConstructFromValues()
        {
            var sourceAddress = RandomUtils.GetIPAddress(IpVersion.IPv6);
            var destinationAddress = RandomUtils.GetIPAddress(IpVersion.IPv6);
            var ipPacket = new IPv6Packet(sourceAddress, destinationAddress);

            Assert.AreEqual(sourceAddress, ipPacket.SourceAddress);
            Assert.AreEqual(destinationAddress, ipPacket.DestinationAddress);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_http.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = (IPv6Packet)p.Extract(typeof(IPv6Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_http.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = (IPv6Packet)p.Extract(typeof(IPv6Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            IPv6Packet.RandomPacket();
        }
    }
}
