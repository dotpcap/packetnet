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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            RawCapture rawCapture;
            bool foundipv4 = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                var ipv4 = (IPv4Packet)p.Extract(typeof(IPv4Packet));
                if (ipv4 == null)
                {
                    continue;
                }
                foundipv4 = true;

                var memoryStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, ipv4);

                memoryStream.Seek (0, SeekOrigin.Begin);
                BinaryFormatter deserializer = new BinaryFormatter();
                IPv4Packet fromFile = (IPv4Packet)deserializer.Deserialize(memoryStream);

                Assert.AreEqual(ipv4.Bytes, fromFile.Bytes);
                Assert.AreEqual(ipv4.BytesHighPerformance.Bytes, fromFile.BytesHighPerformance.Bytes);
                Assert.AreEqual(ipv4.BytesHighPerformance.BytesLength, fromFile.BytesHighPerformance.BytesLength);
                Assert.AreEqual(ipv4.BytesHighPerformance.Length, fromFile.BytesHighPerformance.Length);
                Assert.AreEqual(ipv4.BytesHighPerformance.NeedsCopyForActualBytes, fromFile.BytesHighPerformance.NeedsCopyForActualBytes);
                Assert.AreEqual(ipv4.BytesHighPerformance.Offset, fromFile.BytesHighPerformance.Offset);
                Assert.AreEqual(ipv4.Color, fromFile.Color);
                Assert.AreEqual(ipv4.Header, fromFile.Header);
                Assert.AreEqual(ipv4.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(ipv4.DestinationAddress, fromFile.DestinationAddress);
                Assert.AreEqual(ipv4.HeaderLength, fromFile.HeaderLength);
                Assert.AreEqual(ipv4.HopLimit, fromFile.HopLimit);
                Assert.AreEqual(ipv4.NextHeader, fromFile.NextHeader);
                Assert.AreEqual(ipv4.PayloadLength, fromFile.PayloadLength);
                Assert.AreEqual(ipv4.Protocol, fromFile.Protocol);
                Assert.AreEqual(ipv4.SourceAddress, fromFile.SourceAddress);
                Assert.AreEqual(ipv4.TimeToLive, fromFile.TimeToLive);
                Assert.AreEqual(ipv4.TotalLength, fromFile.TotalLength);
                Assert.AreEqual(ipv4.Version, fromFile.Version);
                Assert.AreEqual(ipv4.DifferentiatedServices, fromFile.DifferentiatedServices);
                Assert.AreEqual(ipv4.FragmentFlags, fromFile.FragmentFlags);
                Assert.AreEqual(ipv4.FragmentOffset, fromFile.FragmentOffset);
                Assert.AreEqual(ipv4.Id, fromFile.Id);
                Assert.AreEqual(ipv4.TypeOfService, fromFile.TypeOfService);
                Assert.AreEqual(ipv4.ValidChecksum, fromFile.ValidChecksum);
                Assert.AreEqual(ipv4.ValidIPChecksum, fromFile.ValidIPChecksum);

                //Method Invocations to make sure that a deserialized packet does not cause 
                //additional errors.

                ipv4.CalculateIPChecksum();
                ipv4.PrintHex();
                ipv4.UpdateCalculatedValues();
                ipv4.UpdateIPChecksum();
            }

            dev.Close();
            Assert.IsTrue(foundipv4, "Capture file contained no ipv4 packets");
        }
    
    }
}
