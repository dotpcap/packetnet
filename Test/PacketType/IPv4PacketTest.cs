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
            var sourceAddress = RandomUtils.GetIPAddress(IPVersion.IPv4);
            var destinationAddress = RandomUtils.GetIPAddress(IPVersion.IPv4);
            var ip = new IPv4Packet(sourceAddress, destinationAddress);

            Assert.AreEqual(sourceAddress, ip.SourceAddress);
            Assert.AreEqual(destinationAddress, ip.DestinationAddress);

            // make sure the version is what we expect
            Assert.AreEqual(IPv4Packet.IPVersion, ip.Version);

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
            var ip = p.Extract<IPv4Packet>();

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
            var ip = p.Extract<IPv4Packet>();

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
                var ip = p.Extract<IPv4Packet>();
                if (ip == null)
                {
                    continue;
                }
                foundipv4 = true;

                var memoryStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, ip);

                memoryStream.Seek (0, SeekOrigin.Begin);
                BinaryFormatter deserializer = new BinaryFormatter();
                IPv4Packet fromFile = (IPv4Packet)deserializer.Deserialize(memoryStream);

                Assert.AreEqual(ip.Bytes, fromFile.Bytes);
                Assert.AreEqual(ip.BytesSegment.Bytes, fromFile.BytesSegment.Bytes);
                Assert.AreEqual(ip.BytesSegment.BytesLength, fromFile.BytesSegment.BytesLength);
                Assert.AreEqual(ip.BytesSegment.Length, fromFile.BytesSegment.Length);
                Assert.AreEqual(ip.BytesSegment.NeedsCopyForActualBytes, fromFile.BytesSegment.NeedsCopyForActualBytes);
                Assert.AreEqual(ip.BytesSegment.Offset, fromFile.BytesSegment.Offset);
                Assert.AreEqual(ip.Color, fromFile.Color);
                Assert.AreEqual(ip.HeaderData, fromFile.HeaderData);
                Assert.AreEqual(ip.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(ip.DestinationAddress, fromFile.DestinationAddress);
                Assert.AreEqual(ip.HeaderLength, fromFile.HeaderLength);
                Assert.AreEqual(ip.HopLimit, fromFile.HopLimit);
                Assert.AreEqual(ip.PayloadLength, fromFile.PayloadLength);
                Assert.AreEqual(ip.Protocol, fromFile.Protocol);
                Assert.AreEqual(ip.SourceAddress, fromFile.SourceAddress);
                Assert.AreEqual(ip.TimeToLive, fromFile.TimeToLive);
                Assert.AreEqual(ip.TotalLength, fromFile.TotalLength);
                Assert.AreEqual(ip.Version, fromFile.Version);
                Assert.AreEqual(ip.DifferentiatedServices, fromFile.DifferentiatedServices);
                Assert.AreEqual(ip.FragmentFlags, fromFile.FragmentFlags);
                Assert.AreEqual(ip.FragmentOffset, fromFile.FragmentOffset);
                Assert.AreEqual(ip.Id, fromFile.Id);
                Assert.AreEqual(ip.TypeOfService, fromFile.TypeOfService);
                Assert.AreEqual(ip.ValidChecksum, fromFile.ValidChecksum);
                Assert.AreEqual(ip.ValidIPChecksum, fromFile.ValidIPChecksum);

                //Method Invocations to make sure that a deserialized packet does not cause 
                //additional errors.

                ip.CalculateIPChecksum();
                ip.PrintHex();
                ip.UpdateCalculatedValues();
                ip.UpdateIPChecksum();
            }

            dev.Close();
            Assert.IsTrue(foundipv4, "Capture file contained no ipv4 packets");
        }
    
    }
}
