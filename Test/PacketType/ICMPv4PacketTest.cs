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
using SharpPcap;
using PacketDotNet;

namespace Test.PacketType
{
    [TestFixture]
    public class ICMPv4PacketTest
    {
        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [Test]
        public void ICMPv4Parsing ()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/ICMPv4.pcap");
            dev.Open();
            var rawPacket = dev.GetNextRawPacket();
            dev.Close();

            // Parse an icmp request
            Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                          new PosixTimeval(rawPacket.Timeval.Seconds,
                                                           rawPacket.Timeval.MicroSeconds),
                                          rawPacket.Data);

            Assert.IsNotNull(p);

            var icmp = ICMPv4Packet.GetEncapsulated(p);
            Console.WriteLine(icmp.GetType());

            Assert.AreEqual(ICMPv4TypeCodes.EchoRequest, icmp.TypeCode);
            Assert.AreEqual(0xe05b, icmp.Checksum);
            Assert.AreEqual(0x0200, icmp.ID);
            Assert.AreEqual(0x6b00, icmp.Sequence);

            // check that the message matches
            string expectedString = "abcdefghijklmnopqrstuvwabcdefghi";
            byte[] expectedData = System.Text.ASCIIEncoding.ASCII.GetBytes(expectedString);
            Assert.AreEqual(expectedData, icmp.Data);
        }
    }
}

