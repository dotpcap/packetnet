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

namespace Test
{
    [TestFixture]
    public class IPv4PacketTest
    {
        [Test]
        public void ConstructingFromValues()
        {
            var sourceAddress = RandomUtils.GetIPAddress(IpVersion.IPv4);
            var destinationAddress = RandomUtils.GetIPAddress(IpVersion.IPv4);
            var ip = new IPv4Packet(sourceAddress, destinationAddress);

            Assert.AreEqual(sourceAddress, ip.SourceAddress);
            Assert.AreEqual(destinationAddress, ip.DestinationAddress);
        }

        [Test]
        public void RandomPacket()
        {
            IPv4Packet.RandomPacket();
        }

        /// <summary>
        /// Test that an ipv4 packet with an invalid total length field
        /// is caught when being parsed
        /// </summary>
        [Test]
        public void IPv4InvalidTotalLength()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/ipv4_invalid_total_length.pcap");
            dev.Open();

            var rawPacket = dev.GetNextRawPacket();

            dev.Close();

            bool caughtExpectedException = false;
            try
            {
                SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);
            } catch(System.InvalidOperationException)
            {
                caughtExpectedException = true;
            }

            Assert.IsTrue(caughtExpectedException);
        }
    }
}
