/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class GreNvgrePacketTest
    {
        // GRE_NVGRE
        [Test]
        public void GreNvgreParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "gre_nvgre.pcap");
            dev.Open();

            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();

            dev.Close();

            LinkLayers linkLayers = rawCapture.GetLinkLayers();
            Assert.AreEqual(LinkLayers.Ethernet, linkLayers);
            Console.WriteLine("Linklayer is ethernet");

            // Linklayer
            Packet p = Packet.ParsePacket(linkLayers, rawCapture.Data);
            Assert.IsNotNull(p);

            // Ethernet
            EthernetPacket eth = p.Extract<EthernetPacket>();
            Assert.IsNotNull(eth);
            Assert.AreEqual(EthernetType.IPv4, eth.Type);
            Console.WriteLine("IPv4 inside ethernet");

            // IPv4
            IPv4Packet ipv4 = eth.Extract<IPv4Packet>();
            Assert.IsNotNull(ipv4);
            Assert.AreEqual(ProtocolType.Gre, ipv4.Protocol);
            Console.WriteLine("GRE inside IPv4");

            // Gre
            GrePacket grep = ipv4.Extract<GrePacket>();
            Assert.IsNotNull(grep);

            // String output
            Console.WriteLine(grep.ToString());

            // Get header
            Assert.AreEqual(false, grep.HasCheckSum);
            Assert.AreEqual(true, grep.HasKey);
            Assert.AreEqual(false, grep.HasSequence);
            Assert.AreEqual(EthernetType.TransparentEthernetBridging, grep.Protocol);
            Console.WriteLine("Transparent Ethernet Bridging over GRE");

            // Inner Ethernet
            EthernetPacket innerEth = grep.Extract<EthernetPacket>();
            Assert.IsNotNull(innerEth);
            Assert.AreEqual(EthernetType.IPv4, innerEth.Type);
            Console.WriteLine("inner IPv4 inside inner ethernet");

            // Inner IPv4
            IPv4Packet innerIpv4 = innerEth.Extract<IPv4Packet>();
            Assert.IsNotNull(innerIpv4);
            Assert.AreEqual(ProtocolType.Tcp, innerIpv4.Protocol);
            Console.WriteLine("TCP inside inner IPv4");

            // InnerTCP
            TcpPacket innerTcp = innerIpv4.Extract<TcpPacket>();
            Assert.IsNotNull(innerTcp);
            Assert.IsNotNull(innerTcp.PayloadData);
            Assert.AreEqual(2, innerTcp.PayloadData.Length);
            Assert.AreEqual(0xab, innerTcp.PayloadData[0]);
            Assert.AreEqual(0xcd, innerTcp.PayloadData[1]);
        }
    }
}