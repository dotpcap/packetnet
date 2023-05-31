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

namespace Test.PacketType;

    [TestFixture]
    public class VxlanPacketTest
    {
        // VXLAN
        [Test]
        public void VxlanParsing()
        {
            VxlanParsingGeneric(VxlanFields.DefaultDestinationPort, @"vxlan.pcap");
        }

        // VXLAN with custom port
        [Test]
        [NonParallelizable]
        public void VxlanCustomPortParsing()
        {
            const ushort CustomVxlanPort = 65530;
            VxlanFields.DestinationPort = CustomVxlanPort;
            VxlanParsingGeneric(CustomVxlanPort, @"vxlan_custom_port.pcap");
            VxlanFields.DestinationPort = VxlanFields.DefaultDestinationPort;
        }

        public static void VxlanParsingGeneric(ushort vxlanPort, string filename)
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + filename);
            dev.Open();

            dev.GetNextPacket(out var c);
            var rawCapture = c.GetPacket();

            dev.Close();

            var linkLayers = rawCapture.GetLinkLayers();
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
            Assert.AreEqual(ProtocolType.Udp, ipv4.Protocol);
            Console.WriteLine("UDP inside IPv4");

            // UDP
            var udp = ipv4.Extract<UdpPacket>();
            Assert.IsNotNull(udp);
            Assert.AreEqual(vxlanPort, udp.DestinationPort);

            // Vxlan
            var vxlan = udp.Extract<VxlanPacket>();
            Assert.IsNotNull(vxlan);
            Assert.AreEqual(0x08, vxlan.Flags);
            Assert.AreEqual(0x123456, vxlan.Vni);

            // String output
            Console.WriteLine(vxlan.ToString());
            Console.WriteLine("Vxlan inside UDP");

            // Inner Ethernet
            var innerEth = vxlan.Extract<EthernetPacket>();
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
        }
    }
