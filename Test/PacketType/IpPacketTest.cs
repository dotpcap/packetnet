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
    public class IpPacketTest
    {
        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void IpPacketFields()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var ip = (IpPacket)p.Extract(typeof(IpPacket));
            Console.WriteLine(ip.GetType());

            Assert.AreEqual(20, ip.Header.Length, "Header.Length doesn't match expected length");
            Console.WriteLine(ip.ToString());
        }
    }
}
