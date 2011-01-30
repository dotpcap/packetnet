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
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace Test.PacketType
{
    [TestFixture]
    public class ICMPv6PacketTest
    {
        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [Test]
        public void ICMPv6Parsing ()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            var rawPacket = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawPacket);

            Assert.IsNotNull(p);

            var icmp = ICMPv6Packet.GetEncapsulated(p);
            Console.WriteLine(icmp.GetType());

            Assert.AreEqual(ICMPv6Types.RouterSolicitation, icmp.Type);
            Assert.AreEqual(0, icmp.Code);
            Assert.AreEqual(0x5d50, icmp.Checksum);

            // Payload differs based on the icmp.Type field
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            RawPacket rawPacket;
            Console.WriteLine("Reading packet data");
            rawPacket = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawPacket);

            Console.WriteLine("Parsing");
            var icmpV6 = ICMPv6Packet.GetEncapsulated(p);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            RawPacket rawPacket;
            Console.WriteLine("Reading packet data");
            rawPacket = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawPacket);

            Console.WriteLine("Parsing");
            var icmpV6 = ICMPv6Packet.GetEncapsulated(p);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString(StringOutputType.Verbose));
        }
    }
}

