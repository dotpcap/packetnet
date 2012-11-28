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
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class PPPoEPPPTest
    {
        [Test]
        public void TestParsingPPPoePPPPacket()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();

            RawCapture rawCapture;
            Packet packet;

            // first packet is a udp packet
            rawCapture = dev.GetNextPacket();
            packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            var udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
            Assert.IsNotNull(udpPacket, "Expected a valid udp packet for the first packet");

            // second packet is the PPPoe Ptp packet
            rawCapture = dev.GetNextPacket();
            packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            var anotherUdpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
            Assert.IsNotNull(anotherUdpPacket, "Expected a valid udp packet for the second packet as well");

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var pppoe = (PPPoEPacket)p.Extract(typeof(PPPoEPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var pppoe = (PPPoEPacket)p.Extract(typeof(PPPoEPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString(StringOutputType.Verbose));
        }
    }
}

