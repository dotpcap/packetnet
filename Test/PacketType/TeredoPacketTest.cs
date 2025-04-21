/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class TeredoPacketTest
    {
        [Test]
        public void TeredoParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "teredo.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var ethernetPacket = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            ClassicAssert.IsNotNull(ethernetPacket);
            ClassicAssert.AreEqual(typeof(EthernetPacket), ethernetPacket.GetType());

            var ip4Packet = ethernetPacket.PayloadPacket;
            ClassicAssert.IsNotNull(ip4Packet);
            ClassicAssert.AreEqual(typeof(IPv4Packet), ip4Packet.GetType());

            var udpPacket = ip4Packet.PayloadPacket;
            ClassicAssert.IsNotNull(udpPacket);
            ClassicAssert.AreEqual(typeof(UdpPacket), udpPacket.GetType());

            var tunneledIp6Packet = udpPacket.PayloadPacket;
            ClassicAssert.IsNotNull(tunneledIp6Packet);
            ClassicAssert.AreEqual(typeof(IPv6Packet), tunneledIp6Packet.GetType());

            var tunneledTcpPacket = tunneledIp6Packet.PayloadPacket;
            ClassicAssert.IsNotNull(tunneledTcpPacket);
            ClassicAssert.AreEqual(typeof(IcmpV6Packet), tunneledTcpPacket.GetType());
        }
    }