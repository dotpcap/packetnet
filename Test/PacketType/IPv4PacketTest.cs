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

            // make sure the version is what we expect
            Assert.AreEqual(IPv4Packet.ipVersion, ip.Version);

            // retrieve the bytes for this IPv4Packet and construct another IPv4 packet from
            // these bytes
            var bytes = ip.Bytes;
            var ip2 = new IPv4Packet(new ByteArraySegment(bytes));

            // compare some of the the values
            //TODO: add more values here or implement an IPv4Packet equals method and use that here
            Assert.AreEqual(ip.Version, ip2.Version);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = (IPv4Packet)p.Extract(typeof(IPv4Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var ip = (IPv4Packet)p.Extract(typeof(IPv4Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(ip.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            IPv4Packet.RandomPacket();
        }
    }
}
