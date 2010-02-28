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
using System.Net.NetworkInformation;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test
{
    [TestFixture]
    public class UdpTest
    {
        [Test]
        public void UDPData()
        {
            SharpPcap.Packets.RawPacket rawPacket;
            UdpPacket u;
            Packet p;

            var dev = new OfflinePcapDevice("../../CaptureFiles/udp_dns_request_response.pcap");
            dev.Open();

            // check the first packet
            rawPacket = dev.GetNextRawPacket();

            p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                   new PosixTimeval(rawPacket.Timeval.Seconds,
                                                    rawPacket.Timeval.MicroSeconds),
                                   rawPacket.Data);
            Assert.IsNotNull(p);

            u = UdpPacket.GetType(p);
            Assert.IsNotNull(u, "Expected a non-null UdpPacket");
            Assert.AreEqual(41 - u.Header.Length,
                            u.PayloadData.Length, "UDPData.Length mismatch");

            // check the second packet
            rawPacket = dev.GetNextRawPacket();
            p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                    new PosixTimeval(rawPacket.Timeval.Seconds,
                                                     rawPacket.Timeval.MicroSeconds),
                                    rawPacket.Data);

            Assert.IsNotNull(p);

            u = UdpPacket.GetType(p);
            Assert.IsNotNull(u, "Expected u to be a UdpPacket");
            Assert.AreEqual(356 - u.Header.Length,
                            u.PayloadData.Length, "UDPData.Length mismatch");

            Console.WriteLine("u is {0}", u.ToString());

            dev.Close();
        }

        [Test]
        public void RandomPacket()
        {
            UdpPacket.RandomPacket();
        }
    }
}
