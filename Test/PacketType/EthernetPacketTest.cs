/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class EthernetPacketTest
    {
        // tcp
        public void VerifyPacket0(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());

            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual(PhysicalAddress.Parse("00-13-10-03-71-47"), e.SourceHardwareAddress);
            ClassicAssert.AreEqual(PhysicalAddress.Parse("00-E0-4C-E5-73-AD"), e.DestinationHardwareAddress);

            var ip = (IPPacket) e.PayloadPacket;
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("82.165.240.134"), ip.SourceAddress);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.221"), ip.DestinationAddress);
            ClassicAssert.AreEqual(IPVersion.IPv4, ip.Version);
            ClassicAssert.AreEqual(ProtocolType.Tcp, ip.Protocol);
            ClassicAssert.AreEqual(254, ip.TimeToLive);
            ClassicAssert.AreEqual(0x0df8, ((IPv4Packet) ip).CalculateIPChecksum());
            ClassicAssert.AreEqual(1176685346, rawCapture.Timeval.Seconds);
            ClassicAssert.AreEqual(885259.000, rawCapture.Timeval.MicroSeconds);

            var tcp = (TcpPacket) ip.PayloadPacket;
            ClassicAssert.AreEqual(80, tcp.SourcePort);
            ClassicAssert.AreEqual(4324, tcp.DestinationPort);
            ClassicAssert.IsTrue(tcp.Acknowledgment);
            ClassicAssert.AreEqual(3536, tcp.WindowSize);
            ClassicAssert.AreEqual(0xc835, tcp.CalculateTcpChecksum());
            Console.WriteLine("tcp.Checksum is {0}", tcp.Checksum);
            ClassicAssert.AreEqual(0xc835, tcp.Checksum, "tcp.Checksum mismatch");
            ClassicAssert.IsTrue(tcp.ValidTcpChecksum);
        }

        // tcp
        public void VerifyPacket1(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());

            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual("0016CFC91E29", e.SourceHardwareAddress.ToString());
            ClassicAssert.AreEqual("0014BFF2EF0A", e.DestinationHardwareAddress.ToString());

            var ip = (IPPacket) p.PayloadPacket;
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.SourceAddress);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("86.42.196.13"), ip.DestinationAddress);
            ClassicAssert.AreEqual(64, ip.TimeToLive);
            ClassicAssert.AreEqual(0x2ff4, ((IPv4Packet) ip).CalculateIPChecksum());
            ClassicAssert.AreEqual(1171483600, rawCapture.Timeval.Seconds);
            ClassicAssert.AreEqual(125234.000, rawCapture.Timeval.MicroSeconds);

            var tcp = (TcpPacket) ip.PayloadPacket;
            ClassicAssert.AreEqual(56925, tcp.SourcePort);
            ClassicAssert.AreEqual(50199, tcp.DestinationPort);
            ClassicAssert.IsTrue(tcp.Acknowledgment);
            ClassicAssert.IsTrue(tcp.Push);
            ClassicAssert.AreEqual(16666, tcp.WindowSize);
            ClassicAssert.AreEqual(0x9b02, tcp.CalculateTcpChecksum());
            ClassicAssert.AreEqual(0x9b02, tcp.Checksum);
            ClassicAssert.IsTrue(tcp.ValidTcpChecksum);
        }

        // udp
        public void VerifyPacket2(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());
            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual("0014BFF2EF0A", e.SourceHardwareAddress.ToString());
            ClassicAssert.AreEqual("0016CFC91E29", e.DestinationHardwareAddress.ToString());

            var ip = p.Extract<IPPacket>();
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.210.164.56"), ip.SourceAddress);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.DestinationAddress);
            ClassicAssert.AreEqual(IPVersion.IPv4, ip.Version);
            ClassicAssert.AreEqual(ProtocolType.Udp, ip.Protocol);
            ClassicAssert.AreEqual(112, ip.TimeToLive);
            ClassicAssert.AreEqual(0xe0a2, ((IPv4Packet) ip).CalculateIPChecksum());
            ClassicAssert.AreEqual(1171483602, rawCapture.Timeval.Seconds);
            ClassicAssert.AreEqual(578641.000, rawCapture.Timeval.MicroSeconds);

            var udp = p.Extract<UdpPacket>();
            ClassicAssert.AreEqual(52886, udp.SourcePort);
            ClassicAssert.AreEqual(56924, udp.DestinationPort);
            ClassicAssert.AreEqual(71, udp.Length);
            ClassicAssert.AreEqual(0xc8b8, udp.Checksum);
        }

        // dns
        public void VerifyPacket3(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());
            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual("0016CFC91E29", e.SourceHardwareAddress.ToString());
            ClassicAssert.AreEqual("0014BFF2EF0A", e.DestinationHardwareAddress.ToString());

            var ip = p.Extract<IPPacket>();
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.172"), ip.SourceAddress);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("66.189.0.29"), ip.DestinationAddress);
            ClassicAssert.AreEqual(ProtocolType.Udp, ip.Protocol);
            ClassicAssert.AreEqual(0x7988, ((IPv4Packet) ip).CalculateIPChecksum());

            var udp = p.Extract<UdpPacket>();
            ClassicAssert.AreEqual(3619, udp.SourcePort);
            ClassicAssert.AreEqual(53, udp.DestinationPort);
            ClassicAssert.AreEqual(47, udp.Length);
            ClassicAssert.AreEqual(0xbe2d, udp.Checksum);
        }

        // arp
        public void VerifyPacket4(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());
            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual("0018F84B17A0", e.SourceHardwareAddress.ToString());
            ClassicAssert.AreEqual("FFFFFFFFFFFF", e.DestinationHardwareAddress.ToString());
        }

        // icmp
        public void VerifyPacket5(Packet p, RawCapture rawCapture)
        {
            Console.WriteLine(p.ToString());
            var e = (EthernetPacket) p;
            ClassicAssert.AreEqual("0016CFC91E29", e.SourceHardwareAddress.ToString());
            ClassicAssert.AreEqual("0014BFF2EF0A", e.DestinationHardwareAddress.ToString());

            var ip = p.Extract<IPPacket>();
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.104"), ip.SourceAddress);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("85.195.52.22"), ip.DestinationAddress);
        }

        [Test]
        public void EthernetConstructorFromMacAddresses()
        {
            var srcHwAddressBytes = new byte[EthernetFields.MacAddressLength];
            for (var i = 0; i < srcHwAddressBytes.Length; i++)
                srcHwAddressBytes[i] = (byte) i;

            var dstHwAddressBytes = new byte[EthernetFields.MacAddressLength];
            for (var i = 0; i < dstHwAddressBytes.Length; i++)
                dstHwAddressBytes[i] = (byte) (dstHwAddressBytes.Length - i);

            var srcHwAddress = new PhysicalAddress(srcHwAddressBytes);
            var dstHwAddress = new PhysicalAddress(dstHwAddressBytes);
            var ethernetPacket = new EthernetPacket(srcHwAddress,
                                                    dstHwAddress,
                                                    EthernetType.None);

            ClassicAssert.AreEqual(14, ethernetPacket.Bytes.Length);
            ClassicAssert.AreEqual(srcHwAddress, ethernetPacket.SourceHardwareAddress);
            ClassicAssert.AreEqual(dstHwAddress, ethernetPacket.DestinationHardwareAddress);

            ethernetPacket.SourceHardwareAddress = dstHwAddress;
            ethernetPacket.DestinationHardwareAddress = srcHwAddress;

            ClassicAssert.AreEqual(dstHwAddress, ethernetPacket.SourceHardwareAddress);
            ClassicAssert.AreEqual(srcHwAddress, ethernetPacket.DestinationHardwareAddress);

            Console.WriteLine("ethernetPacket.ToString() {0}", ethernetPacket);
        }

        [Test]
        public void ParsingPacket()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "test_stream.pcap");
            dev.Open();

            PacketCapture c;
            dev.GetNextPacket(out c);
            var p = c.GetPacket();

            var e = new EthernetPacket(new ByteArraySegment(p.Data));
            Console.WriteLine("ethernet.ToString() {0}", e);

            ClassicAssert.AreEqual(PhysicalAddress.Parse("00-13-10-03-71-47"), e.SourceHardwareAddress);
            ClassicAssert.AreEqual(PhysicalAddress.Parse("00-E0-4C-E5-73-AD"), e.DestinationHardwareAddress);

            ClassicAssert.AreEqual(EthernetType.IPv4, e.Type);

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "test_stream.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            var status = dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var eth = (EthernetPacket) p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(eth.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "test_stream.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            var status = dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var eth = (EthernetPacket) p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(eth.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            EthernetPacket.RandomPacket();
        }

        /// <summary>
        /// Test parsing a handful of packets with known contents as verified by
        /// wireshark.
        /// </summary>
        [Test]
        public void TestParsingKnownPackets()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "test_stream.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(),
                                           rawCapture.Data);

                switch (packetIndex)
                {
                    case 0:
                    {
                        VerifyPacket0(p, rawCapture);
                        break;
                    }
                    case 1:
                    {
                        VerifyPacket1(p, rawCapture);
                        break;
                    }
                    case 2:
                    {
                        VerifyPacket2(p, rawCapture);
                        break;
                    }
                    case 3:
                    {
                        VerifyPacket3(p, rawCapture);
                        break;
                    }
                    case 4:
                    {
                        VerifyPacket4(p, rawCapture);
                        break;
                    }
                    case 5:
                    {
                        VerifyPacket5(p, rawCapture);
                        break;
                    }
                    default:
                    {
                        ClassicAssert.Fail("didn't expect to get to packetIndex " + packetIndex);
                        break;
                    }
                }

                packetIndex++;
            }

            dev.Close();
        }
    }