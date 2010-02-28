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

namespace Test
{
    [TestFixture]
    public class PPPoEPPPTest
    {
        [Test]
        public void TestParsingPPPoePPPPacket()
        {
            var dev = new SharpPcap.OfflinePcapDevice("../../CaptureFiles/PPPoEPPP.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            Packet packet;

            // first packet is a udp packet
            rawPacket = dev.GetNextRawPacket();
            packet = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);
            var udpPacket = UdpPacket.GetType(packet);
            Assert.IsNotNull(udpPacket, "Expected a valid udp packet for the first packet");

            // second packet is the PPPoe Ptp packet
            rawPacket = dev.GetNextRawPacket();
            packet = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);
            var anotherUdpPacket = UdpPacket.GetType(packet);
            Assert.IsNotNull(anotherUdpPacket, "Expected a valid udp packet for the second packet as well");

            dev.Close();
        }
    }
}

