/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Net;
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
    public class IPv6PacketTest
    {
        // icmpv6
        public void VerifyPacket0(Packet p, RawCapture rawCapture, LinkLayers linkLayers)
        {
            ClassicAssert.IsNotNull(p);
            Console.WriteLine(p.ToString());

            ClassicAssert.AreEqual(linkLayers, rawCapture.GetLinkLayers());

            if (linkLayers == LinkLayers.Ethernet)
            {
                var e = (EthernetPacket) p;
                ClassicAssert.AreEqual(PhysicalAddress.Parse("00-A0-CC-D9-41-75"), e.SourceHardwareAddress);
                ClassicAssert.AreEqual(PhysicalAddress.Parse("33-33-00-00-00-02"), e.DestinationHardwareAddress);
            }

            var ip = p.Extract<IPPacket>();
            Console.WriteLine("ip {0}", ip);
            ClassicAssert.AreEqual(IPAddress.Parse("fe80::2a0:ccff:fed9:4175"), ip.SourceAddress);
            ClassicAssert.AreEqual(IPAddress.Parse("ff02::2"), ip.DestinationAddress);
            ClassicAssert.AreEqual(IPVersion.IPv6, ip.Version);
            ClassicAssert.AreEqual(ProtocolType.IcmpV6, ip.Protocol);
            ClassicAssert.AreEqual(16, ip.PayloadPacket.Bytes.Length, "ip.PayloadPacket.Bytes.Length mismatch");
            ClassicAssert.AreEqual(255, ip.HopLimit);
            ClassicAssert.AreEqual(255, ip.TimeToLive);
            Console.WriteLine("Failed: ip.ComputeIPChecksum() not implemented.");
            ClassicAssert.AreEqual(1221145299, rawCapture.Timeval.Seconds);
            ClassicAssert.AreEqual(453568.000, rawCapture.Timeval.MicroSeconds);
        }

        public void VerifyPacket1(Packet p, RawCapture rawCapture, LinkLayers linkLayers)
        {
            ClassicAssert.IsNotNull(p);
            Console.WriteLine(p.ToString());

            ClassicAssert.AreEqual(linkLayers, rawCapture.GetLinkLayers());

            if (linkLayers == LinkLayers.Ethernet)
            {
                var e = (EthernetPacket) p;
                ClassicAssert.AreEqual(PhysicalAddress.Parse("F894C22EFAD1"), e.SourceHardwareAddress);
                ClassicAssert.AreEqual(PhysicalAddress.Parse("333300000016"), e.DestinationHardwareAddress);
            }

            var ip = p.Extract<IPv6Packet>();
            Console.WriteLine("ip {0}", ip);
            ClassicAssert.AreEqual(IPAddress.Parse("fe80::d802:3589:15cf:3128"), ip.SourceAddress);
            ClassicAssert.AreEqual(IPAddress.Parse("ff02::16"), ip.DestinationAddress);
            ClassicAssert.AreEqual(IPVersion.IPv6, ip.Version);
            ClassicAssert.AreEqual(ProtocolType.IcmpV6, ip.Protocol);
            ClassicAssert.AreEqual(28, ip.PayloadPacket.Bytes.Length, "ip.PayloadPacket.Bytes.Length mismatch");
            ClassicAssert.AreEqual(1, ip.HopLimit);
            ClassicAssert.AreEqual(1, ip.TimeToLive);
            ClassicAssert.AreEqual(0x3a, (byte) ip.Protocol);
            Console.WriteLine("Failed: ip.ComputeIPChecksum() not implemented.");
            ClassicAssert.AreEqual(1543415539, rawCapture.Timeval.Seconds);
            ClassicAssert.AreEqual(841441.000, rawCapture.Timeval.MicroSeconds);
            ClassicAssert.AreEqual(1, ip.ExtensionHeaders.Count);
            ClassicAssert.AreEqual(6, ip.ExtensionHeaders[0].Payload.Length);
        }

        // Test that we can load and parse an IPv6 packet
        // for multiple LinkLayers types
        [TestCase("ipv6_icmpv6_packet.pcap", LinkLayers.Ethernet)]
        [TestCase("ipv6_icmpv6_packet_raw_linklayer.pcap", LinkLayers.RawLegacy)]
        public void IPv6PacketTestParsing(string pcapPath, LinkLayers linkLayers)
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + pcapPath);
            dev.Open();

            PacketCapture e;
            GetPacketStatus status;
            var packetIndex = 0;
            while ((status = dev.GetNextPacket(out e)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = e.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Console.WriteLine("got packet");
                switch (packetIndex)
                {
                    case 0:
                    {
                        VerifyPacket0(p, rawCapture, linkLayers);
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

        // Test that we can load and parse an with IPv6 packet with an extended (Hop by Hop) header 
        [TestCase("ipv6_icmpv6_hopbyhop_packet.pcap", LinkLayers.Ethernet)]
        public void IPv6PacketHopByHopTestParsing(string pcapPath, LinkLayers linkLayers)
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + pcapPath);
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                switch (packetIndex)
                {
                    case 0:
                    {
                        VerifyPacket1(p, rawCapture, linkLayers);
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

        [Test]
        public void ConstructFromValues()
        {
            var sourceAddress = RandomUtils.GetIPAddress(IPVersion.IPv6);
            var destinationAddress = RandomUtils.GetIPAddress(IPVersion.IPv6);
            var ipPacket = new IPv6Packet(sourceAddress, destinationAddress);

            ClassicAssert.AreEqual(sourceAddress, ipPacket.SourceAddress);
            ClassicAssert.AreEqual(destinationAddress, ipPacket.DestinationAddress);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_http.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = p.Extract<IPv6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_http.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = p.Extract<IPv6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            IPv6Packet.RandomPacket();
        }

        /// <summary>
        /// Test that we can load and parse an IPv6 TCP packet and that
        /// the computed tcp checksum matches the expected checksum
        /// </summary>
        [Test]
        public void TCPChecksumIPv6()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_http.pcap");
            dev.Open();

            // checksums from wireshark of the capture file
            int[] expectedChecksum =
            {
                0x41a2,
                0x4201,
                0x5728,
                0xf448,
                0xee07,
                0x939c,
                0x63e4,
                0x4590,
                0x3725,
                0x3723
            };

            var packetIndex = 0;
            GetPacketStatus status;
            PacketCapture c;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                var t = p.Extract<TcpPacket>();
                ClassicAssert.IsNotNull(t, "Expected t to not be null");
                ClassicAssert.IsTrue(t.ValidChecksum, "t.ValidChecksum isn't true");

                // compare the computed checksum to the expected one
                ClassicAssert.AreEqual(expectedChecksum[packetIndex],
                                t.CalculateTcpChecksum(),
                                "Checksum mismatch");

                packetIndex++;
            }

            dev.Close();
        }

        // Test that we can correctly set the data section of a IPv6 packet
        [Test]
        public void TCPDataIPv6()
        {
            var s = "-++++=== HELLLLOOO ===++++-";
            var data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random packet
            var p = TcpPacket.RandomPacket();

            //replace pkt's data with our string
            p.PayloadData = data;

            //sanity check
            ClassicAssert.AreEqual(s, System.Text.Encoding.Default.GetString(p.PayloadData));
        }
    }