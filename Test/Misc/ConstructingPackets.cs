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

namespace Test.Misc
{
    [TestFixture]
    public class ConstructingPackets
    {
        /// <summary>
        /// Build a complete ethernet packet
        /// </summary>
        [Test]
        public void BuildEthernetPacket()
        {
            var tcpPacket = TcpPacket.RandomPacket();
            var ipPacket = IpPacket.RandomPacket(IpVersion.IPv4);
            var ethernetPacket = EthernetPacket.RandomPacket();

            // put these all together into a single packet
            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            Console.WriteLine("random packet: {0}", ethernetPacket.ToString());

            // and get a byte array that represents the single packet
            var bytes = ethernetPacket.Bytes;

            // and re-parse that packet
            var newPacket = Packet.ParsePacket(LinkLayers.Ethernet,
                                               bytes);

            Console.WriteLine("re-parsed random packet: {0}", newPacket.ToString());
        }
    }
}
