/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class GrePacketCustomProtocolTest
    {
        // GRE without Custom Protocol parser
        [Test]
        public void GreWithoutCustomParserParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "gre_custom_protocol.pcap");
            dev.Open();

            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();

            dev.Close();

            LinkLayers linkLayers = rawCapture.GetLinkLayers();
            Assert.AreEqual(LinkLayers.Ethernet, linkLayers);
            Console.WriteLine("Linklayer is ethernet");

            // Linklayer
            var p = Packet.ParsePacket(linkLayers, rawCapture.Data);
            Assert.IsNotNull(p);

            // Ethernet
            var eth = p.Extract<EthernetPacket>();
            Assert.IsNotNull(eth);
            Assert.AreEqual(EthernetType.IPv4, eth.Type);
            Console.WriteLine("IPv4 inside ethernet");

            // IPv4
            var ipv4 = eth.Extract<IPv4Packet>();
            Assert.IsNotNull(ipv4);
            Assert.AreEqual(ProtocolType.Gre, ipv4.Protocol);
            Console.WriteLine("GRE inside IPv4");

            // Gre
            var grep = ipv4.Extract<GrePacket>();
            Assert.IsNotNull(grep);
            Assert.IsTrue(grep.HasPayloadData);
            Console.WriteLine("GRE payload");

            // Erspan
            var erspan = grep.Extract<ErspanPacket>();
            Assert.IsNull(erspan);
        }

        // GRE with Custom Protocol parser
        [Test]
        [NonParallelizable]
        public void GreWithCustomParserParsing()
        {
            GrePacket.CustomPayloadDecoder = CustomParser;

            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "gre_custom_protocol.pcap");
            dev.Open();

            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();

            dev.Close();

            LinkLayers linkLayers = rawCapture.GetLinkLayers();
            Assert.AreEqual(LinkLayers.Ethernet, linkLayers);
            Console.WriteLine("Linklayer is ethernet");

            // Linklayer
            var p = Packet.ParsePacket(linkLayers, rawCapture.Data);
            Assert.IsNotNull(p);

            // Ethernet
            var eth = p.Extract<EthernetPacket>();
            Assert.IsNotNull(eth);
            Assert.AreEqual(EthernetType.IPv4, eth.Type);
            Console.WriteLine("IPv4 inside ethernet");

            // IPv4
            var ipv4 = eth.Extract<IPv4Packet>();
            Assert.IsNotNull(ipv4);
            Assert.AreEqual(ProtocolType.Gre, ipv4.Protocol);
            Console.WriteLine("GRE inside IPv4");

            // Gre
            var grep = ipv4.Extract<GrePacket>();
            Assert.IsNotNull(grep);
            Assert.AreEqual(ErspanPacket.ErspanGreProtocol, (ushort)grep.Protocol);
            Console.WriteLine("Custom ERSPAN over GRE");

            // Erspan
            var erspan = grep.Extract<ErspanPacket>();
            Assert.IsNotNull(erspan);

            // Inner Ethernet
            var innerEth = erspan.Extract<EthernetPacket>();
            Assert.IsNotNull(innerEth);
            Assert.AreEqual(EthernetType.IPv4, innerEth.Type);
            Console.WriteLine("inner IPv4 inside inner ethernet");

            // Inner IPv4
            var innerIpv4 = innerEth.Extract<IPv4Packet>();
            Assert.IsNotNull(innerIpv4);
            Assert.AreEqual(ProtocolType.Tcp, innerIpv4.Protocol);
            Console.WriteLine("TCP inside inner IPv4");

            // Inner TCP
            var innerTcp = innerIpv4.Extract<TcpPacket>();
            Assert.IsNotNull(innerTcp);
            Assert.IsNotNull(innerTcp.PayloadData);
            Assert.AreEqual(2, innerTcp.PayloadData.Length);
            Assert.AreEqual(0xab, innerTcp.PayloadData[0]);
            Assert.AreEqual(0xcd, innerTcp.PayloadData[1]);

            GrePacket.CustomPayloadDecoder = null;
        }

        public PacketOrByteArraySegment CustomParser(ByteArraySegment payload, GrePacket parent)
        {
            switch ((ushort)parent.Protocol)
            {
                case ErspanPacket.ErspanGreProtocol:
                    return ErspanPacket.CustomErspanGreParser(payload, parent);
                default:
                    return null;
            }
        }
    }

    // A custom parser for ERSPAN Type I 
    // Ref: https://datatracker.ietf.org/doc/html/draft-foschiano-erspan-03
    public sealed class ErspanPacket : Packet
    {
        public ErspanPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            // ERSPAN type 1 has no extra header.
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = 0
            };

            // Parse the encapsulated ethernet packet.
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment { Packet = new EthernetPacket(Header.NextSegment()) });
            ParentPacket = parentPacket;
        }

        public static PacketOrByteArraySegment CustomErspanGreParser(ByteArraySegment payload, GrePacket parent)
        {
            if (0 == (parent.FlagsAndVersion & 0x1000))
            {
                // Type 1
                return new PacketOrByteArraySegment() { Packet = new ErspanPacket(payload, parent) };
            }
            else
            {
                // Type 2
                throw new NotImplementedException();
            }
        }

        public const ushort ErspanGreProtocol = 0x88be;
    }
}
