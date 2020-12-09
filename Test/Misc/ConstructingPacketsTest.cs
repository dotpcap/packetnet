/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using PacketDotNet;

namespace Test.Misc
{
    [TestFixture]
    public class ConstructingPacketsTest
    {
        /// <summary>
        /// Build a complete ethernet packet
        /// </summary>
        [Test]
        public void BuildEthernetPacket()
        {
            var tcpPacket = TcpPacket.RandomPacket();
            var ipPacket = IPPacket.RandomPacket(IPVersion.IPv4);
            var ethernetPacket = EthernetPacket.RandomPacket();

            // put these all together into a single packet
            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            Console.WriteLine("random packet: {0}", ethernetPacket);

            // and get a byte array that represents the single packet
            var bytes = ethernetPacket.Bytes;

            // and re-parse that packet
            var newPacket = Packet.ParsePacket(LinkLayers.Ethernet,
                                               bytes);

            Console.WriteLine("re-parsed random packet: {0}", newPacket);
        }
    }
}