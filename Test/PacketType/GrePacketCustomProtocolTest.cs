/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

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
            ClassicAssert.AreEqual(LinkLayers.Ethernet, linkLayers);
            Console.WriteLine("Linklayer is ethernet");

            // Linklayer
            var p = Packet.ParsePacket(linkLayers, rawCapture.Data);
            ClassicAssert.IsNotNull(p);

            // Ethernet
            var eth = p.Extract<EthernetPacket>();
            ClassicAssert.IsNotNull(eth);
            ClassicAssert.AreEqual(EthernetType.IPv4, eth.Type);
            Console.WriteLine("IPv4 inside ethernet");

            // IPv4
            var ipv4 = eth.Extract<IPv4Packet>();
            ClassicAssert.IsNotNull(ipv4);
            ClassicAssert.AreEqual(ProtocolType.Gre, ipv4.Protocol);
            Console.WriteLine("GRE inside IPv4");

            // Gre
            var grep = ipv4.Extract<GrePacket>();
            ClassicAssert.IsNotNull(grep);
            ClassicAssert.IsTrue(grep.HasPayloadData);
            Console.WriteLine("GRE payload");

            // Erspan
            var erspan = grep.Extract<ErspanPacket>();
            ClassicAssert.IsNull(erspan);
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
            ClassicAssert.AreEqual(LinkLayers.Ethernet, linkLayers);
            Console.WriteLine("Linklayer is ethernet");

            // Linklayer
            var p = Packet.ParsePacket(linkLayers, rawCapture.Data);
            ClassicAssert.IsNotNull(p);

            // Ethernet
            var eth = p.Extract<EthernetPacket>();
            ClassicAssert.IsNotNull(eth);
            ClassicAssert.AreEqual(EthernetType.IPv4, eth.Type);
            Console.WriteLine("IPv4 inside ethernet");

            // IPv4
            var ipv4 = eth.Extract<IPv4Packet>();
            ClassicAssert.IsNotNull(ipv4);
            ClassicAssert.AreEqual(ProtocolType.Gre, ipv4.Protocol);
            Console.WriteLine("GRE inside IPv4");

            // Gre
            var grep = ipv4.Extract<GrePacket>();
            ClassicAssert.IsNotNull(grep);
            ClassicAssert.AreEqual(ErspanPacket.ErspanGreProtocol, (ushort)grep.Protocol);
            Console.WriteLine("Custom ERSPAN over GRE");

            // Erspan
            var erspan = grep.Extract<ErspanPacket>();
            ClassicAssert.IsNotNull(erspan);

            // Inner Ethernet
            var innerEth = erspan.Extract<EthernetPacket>();
            ClassicAssert.IsNotNull(innerEth);
            ClassicAssert.AreEqual(EthernetType.IPv4, innerEth.Type);
            Console.WriteLine("inner IPv4 inside inner ethernet");

            // Inner IPv4
            var innerIpv4 = innerEth.Extract<IPv4Packet>();
            ClassicAssert.IsNotNull(innerIpv4);
            ClassicAssert.AreEqual(ProtocolType.Tcp, innerIpv4.Protocol);
            Console.WriteLine("TCP inside inner IPv4");

            // Inner TCP
            var innerTcp = innerIpv4.Extract<TcpPacket>();
            ClassicAssert.IsNotNull(innerTcp);
            ClassicAssert.IsNotNull(innerTcp.PayloadData);
            ClassicAssert.AreEqual(2, innerTcp.PayloadData.Length);
            ClassicAssert.AreEqual(0xab, innerTcp.PayloadData[0]);
            ClassicAssert.AreEqual(0xcd, innerTcp.PayloadData[1]);

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
