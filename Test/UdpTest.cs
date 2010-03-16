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
        /// <summary>
        /// Test that we can load a udp packet and that the udp properties are
        /// as we expect them
        /// </summary>
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

        /// <summary>
        /// Test that we can build a udp packet from values, convert it into a byte[]
        /// and then re-parse it back into a UdpPacket.
        /// 
        /// Also test that the UdpPacket.Length field is updated properly in the
        /// conversion to a byte[]
        /// </summary>
        [Test]
        public void ConstructUdpPacketFromValuesAndCheckThatLengthIsUpdated()
        {
            // build a udp packet
            ushort sourcePort = 200;
            ushort destinationPort = 300;
            byte[] dataBytes = new byte[32];
            for(int i = 0; i < dataBytes.Length; i++)
            {
                dataBytes[i] = (byte)i;
            }

            var udpPacket = new UdpPacket(sourcePort, destinationPort);
            udpPacket.PayloadData = dataBytes;

            // retrieve the bytes, this should cause UdpPacket.Length to be updated
            var packetBytes = udpPacket.Bytes;

            // now reparse the packet again
            var udpPacket2 = new UdpPacket(packetBytes, 0);

            Assert.AreEqual(sourcePort, udpPacket.SourcePort);
            Assert.AreEqual(destinationPort, udpPacket.DestinationPort);

            Console.WriteLine("udpPacket.Length {0}", udpPacket.Length);
            udpPacket.PayloadData = dataBytes;

            Assert.AreEqual(sourcePort, udpPacket.SourcePort);
            Assert.AreEqual(destinationPort, udpPacket.DestinationPort);

            // make sure the data matches up
            Assert.AreEqual(dataBytes, udpPacket2.PayloadData, "PayloadData mismatch");

            // and make sure the length is what we expect
            Assert.AreEqual(dataBytes.Length + UdpFields.HeaderLength, udpPacket2.Length);
        }

        [Test]
        public void RandomPacket()
        {
            UdpPacket.RandomPacket();
        }
    }
}
