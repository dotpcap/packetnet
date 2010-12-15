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
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class EthernetPacketTest
    {
        // tcp
        public void VerifyPacket0(Packet p)
        {
            Console.WriteLine(p.ToString());

            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual(PhysicalAddress.Parse("00-13-10-03-71-47"), e.SourceHwAddress);
            Assert.AreEqual(PhysicalAddress.Parse("00-E0-4C-E5-73-AD"), e.DestinationHwAddress);

            IpPacket ip = (IpPacket)e.PayloadPacket;
            Assert.AreEqual(System.Net.IPAddress.Parse("82.165.240.134"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.221"), ip.DestinationAddress);
            Assert.AreEqual(IpVersion.IPv4, ip.Version);
            Assert.AreEqual(IPProtocolType.TCP, ip.Protocol);
            Assert.AreEqual(254, ip.TimeToLive);
            //FIXME: Should probably implement this
//            Assert.AreEqual(0x0df8, ip.ComputeIPChecksum());
            Assert.AreEqual(1176685346, ip.Timeval.Seconds);
            Assert.AreEqual(885259.000, ip.Timeval.MicroSeconds);

            TcpPacket tcp = (TcpPacket)ip.PayloadPacket;
            Assert.AreEqual(80, tcp.SourcePort);
            Assert.AreEqual(4324, tcp.DestinationPort);
            Assert.IsTrue(tcp.Ack);
            Assert.AreEqual(3536, tcp.WindowSize);
            //FIXME: should reenable these when we've fixed the algorithm
//            Assert.AreEqual(0x0df8, tcp.ComputeIPChecksum());
//            Assert.AreEqual(0xc835, tcp.ComputeTCPChecksum());
            Assert.AreEqual(0xc835, tcp.Checksum);
//            Assert.IsTrue(tcp.ValidTCPChecksum);
        }

        // tcp
        public void VerifyPacket1(Packet p)
        {
            Console.WriteLine(p.ToString());

            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual("0016CFC91E29", e.SourceHwAddress.ToString());
            Assert.AreEqual("0014BFF2EF0A", e.DestinationHwAddress.ToString());

            IpPacket ip = (IpPacket)p.PayloadPacket;
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("86.42.196.13"), ip.DestinationAddress);
            Assert.AreEqual(64, ip.TimeToLive);
//            Assert.AreEqual(0x2ff4, ip.ComputeIPChecksum());
            Assert.AreEqual(1171483600, ip.Timeval.Seconds);
            Assert.AreEqual(125234.000, ip.Timeval.MicroSeconds);

            TcpPacket tcp = (TcpPacket)ip.PayloadPacket;
            Assert.AreEqual(56925, tcp.SourcePort);
            Assert.AreEqual(50199, tcp.DestinationPort);
            Assert.IsTrue(tcp.Ack);
            Assert.IsTrue(tcp.Psh);
            Assert.AreEqual(16666, tcp.WindowSize);
//            Assert.AreEqual(0x2ff4, tcp.ComputeIPChecksum());
//            Assert.AreEqual(0x9b02, tcp.ComputeTCPChecksum());
            Assert.AreEqual(0x9b02, tcp.Checksum);
//            Assert.IsTrue(tcp.ValidTCPChecksum);
        }

        // udp
        public void VerifyPacket2(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual("0014BFF2EF0A", e.SourceHwAddress.ToString());
            Assert.AreEqual("0016CFC91E29", e.DestinationHwAddress.ToString());

            var ip = IpPacket.GetEncapsulated(p);
            Assert.AreEqual(System.Net.IPAddress.Parse("172.210.164.56"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.DestinationAddress);
            Assert.AreEqual(IpVersion.IPv4, ip.Version);
            Assert.AreEqual(IPProtocolType.UDP, ip.Protocol);
            Assert.AreEqual(112, ip.TimeToLive);
//            Assert.AreEqual(0xe0a2, ip.ComputeIPChecksum());
            Assert.AreEqual(1171483602, ip.Timeval.Seconds);
            Assert.AreEqual(578641.000, ip.Timeval.MicroSeconds);

            var udp = UdpPacket.GetEncapsulated(p);
            Assert.AreEqual(52886, udp.SourcePort);
            Assert.AreEqual(56924, udp.DestinationPort);
//            Assert.AreEqual(71, udp.UDPLength);
//            Assert.AreEqual(0xe0a2, udp.ComputeIPChecksum());
            Assert.AreEqual(0xc8b8, udp.Checksum);
        }

        // dns
        public void VerifyPacket3(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual("0016CFC91E29", e.SourceHwAddress.ToString());
            Assert.AreEqual("0014BFF2EF0A", e.DestinationHwAddress.ToString());

            var ip = IpPacket.GetEncapsulated(p);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.172"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("66.189.0.29"), ip.DestinationAddress);
            Assert.AreEqual(IPProtocolType.UDP, ip.Protocol);
//            Assert.AreEqual(0x7988, ip.ComputeIPChecksum());

            var udp = UdpPacket.GetEncapsulated(p);
            Assert.AreEqual(3619, udp.SourcePort);
            Assert.AreEqual(53, udp.DestinationPort);
//            Assert.AreEqual(47, udp.UDPLength);
//            Assert.AreEqual(0x7988, udp.ComputeIPChecksum());
            Assert.AreEqual(0xbe2d, udp.Checksum);
        }

        // arp
        public void VerifyPacket4(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual("0018F84B17A0", e.SourceHwAddress.ToString());
            Assert.AreEqual("FFFFFFFFFFFF", e.DestinationHwAddress.ToString());
        }

        // icmp
        public void VerifyPacket5(Packet p)
        {
            Console.WriteLine(p.ToString());
            EthernetPacket e = (EthernetPacket)p;
            Assert.AreEqual("0016CFC91E29", e.SourceHwAddress.ToString());
            Assert.AreEqual("0014BFF2EF0A", e.DestinationHwAddress.ToString());

            var ip = IpPacket.GetEncapsulated(p);
            Assert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.SourceAddress);
            Assert.AreEqual(System.Net.IPAddress.Parse("85.195.52.22"), ip.DestinationAddress);
        }

        /// <summary>
        /// Test parsing a handful of packets with known contents as verified by
        /// wireshark.
        /// </summary>
        [Test]
        public void TestParsingKnownPackets()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/test_stream.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            int packetIndex = 0;
            while((rawPacket = dev.GetNextRawPacket()) != null)
            {
                Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                              new PosixTimeval(rawPacket.Timeval.Seconds, rawPacket.Timeval.MicroSeconds),
                                              rawPacket.Data);
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                case 1:
                    VerifyPacket1(p);
                    break;
                case 2:
                    VerifyPacket2(p);
                    break;
                case 3:
                    VerifyPacket3(p);
                    break;
                case 4:
                    VerifyPacket4(p);
                    break;
                case 5:
                    VerifyPacket5(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }

        [Test]
        public void EthernetConstructorFromMacAddresses()
        {
            var srcHwAddressBytes = new Byte[EthernetFields.MacAddressLength];
            for(int i = 0; i < srcHwAddressBytes.Length; i++)
            {
                srcHwAddressBytes[i] = (byte)i;
            }

            var dstHwAddressBytes = new Byte[EthernetFields.MacAddressLength];
            for(int i = 0; i < dstHwAddressBytes.Length; i++)
            {
                dstHwAddressBytes[i] = (byte)(dstHwAddressBytes.Length - i);
            }

            var srcHwAddress = new PhysicalAddress(srcHwAddressBytes);
            var dstHwAddress = new PhysicalAddress(dstHwAddressBytes);
            var ethernetPacket = new EthernetPacket(srcHwAddress,
                                                    dstHwAddress,
                                                    EthernetPacketType.None);

            int expectedLength = 14;
            Assert.AreEqual(expectedLength, ethernetPacket.Bytes.Length);
            //TODO: improve this here
            Console.WriteLine("ethernetPacket.ToString() {0}",
                              ethernetPacket.ToString());
        }

        [Test]
        public void ParsingPacket()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/test_stream.pcap");
            dev.Open();

            SharpPcap.Packets.Packet p;
            p = dev.GetNextPacket();

            var e = new EthernetPacket(p.Bytes, 0);
            Console.WriteLine("ethernet.ToString() {0}", e.ToString());

            Assert.AreEqual(PhysicalAddress.Parse("00-13-10-03-71-47"), e.SourceHwAddress);
            Assert.AreEqual(PhysicalAddress.Parse("00-E0-4C-E5-73-AD"), e.DestinationHwAddress);

            Assert.AreEqual(EthernetPacketType.IpV4, e.Type);

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/test_stream.pcap");
            dev.Open();
            SharpPcap.Packets.RawPacket rawPacket;
            Console.WriteLine("Reading packet data");
            rawPacket = dev.GetNextRawPacket();
            var p = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);

            Console.WriteLine("Parsing");
            var eth = (EthernetPacket)p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(eth.ToString());
        }

        [Test]
        public void RandomPacket()
        {
            EthernetPacket.RandomPacket();
        }
    }
}