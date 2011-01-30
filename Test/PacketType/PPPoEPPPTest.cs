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
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class PPPoEPPPTest
    {
        [Test]
        public void TestParsingPPPoePPPPacket()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();

            RawPacket rawPacket;
            Packet packet;

            // first packet is a udp packet
            rawPacket = dev.GetNextPacket();
            packet = Packet.ParsePacket(rawPacket);
            var udpPacket = UdpPacket.GetEncapsulated(packet);
            Assert.IsNotNull(udpPacket, "Expected a valid udp packet for the first packet");

            // second packet is the PPPoe Ptp packet
            rawPacket = dev.GetNextPacket();
            packet = Packet.ParsePacket(rawPacket);
            var anotherUdpPacket = UdpPacket.GetEncapsulated(packet);
            Assert.IsNotNull(anotherUdpPacket, "Expected a valid udp packet for the second packet as well");

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawPacket = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawPacket);

            Console.WriteLine("Parsing");
            var pppoe = PPPoEPacket.GetEncapsulated(p);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new OfflinePcapDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawPacket = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawPacket);

            Console.WriteLine("Parsing");
            var pppoe = PPPoEPacket.GetEncapsulated(p);

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString(StringOutputType.Verbose));
        }
    }
}

