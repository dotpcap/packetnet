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
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class IcmpV4PacketTest
    {
        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [TestCase("ICMPv4.pcap", LinkLayers.Ethernet)]
        [TestCase("ICMPv4_raw_linklayer.pcap", LinkLayers.RawLegacy)]
        public void IcmpV4Parsing(string pcapPath, LinkLayers linkLayers)
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + pcapPath);
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            // Parse an icmp request
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);
            Assert.AreEqual(linkLayers, rawCapture.LinkLayerType);

            var icmp = p.Extract<IcmpV4Packet>();
            Console.WriteLine(icmp.GetType());

            Assert.AreEqual(IcmpV4TypeCode.EchoRequest, icmp.TypeCode);
            Assert.AreEqual(0xe05b, icmp.Checksum);
            Assert.AreEqual(0x0200, icmp.Id);
            Assert.AreEqual(0x6b00, icmp.Sequence);

            // check that the message matches
            const string expectedString = "abcdefghijklmnopqrstuvwabcdefghi";
            var expectedData = System.Text.Encoding.ASCII.GetBytes(expectedString);
            Assert.AreEqual(expectedData, icmp.Data);
        }

        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ICMPv4.pcap");
            dev.Open();

            RawCapture rawCapture;
            var foundicmp = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                var icmp = p.Extract<IcmpV4Packet>();
                if (icmp == null)
                {
                    continue;
                }

                foundicmp = true;

                var memoryStream = new MemoryStream();
                var serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, icmp);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var deserializer = new BinaryFormatter();
                var fromFile = (IcmpV4Packet) deserializer.Deserialize(memoryStream);

                Assert.AreEqual(icmp.Bytes, fromFile.Bytes);
                Assert.AreEqual(icmp.BytesSegment.Bytes, fromFile.BytesSegment.Bytes);
                Assert.AreEqual(icmp.BytesSegment.BytesLength, fromFile.BytesSegment.BytesLength);
                Assert.AreEqual(icmp.BytesSegment.Length, fromFile.BytesSegment.Length);
                Assert.AreEqual(icmp.BytesSegment.NeedsCopyForActualBytes, fromFile.BytesSegment.NeedsCopyForActualBytes);
                Assert.AreEqual(icmp.BytesSegment.Offset, fromFile.BytesSegment.Offset);
                Assert.AreEqual(icmp.Checksum, fromFile.Checksum);
                Assert.AreEqual(icmp.Color, fromFile.Color);
                Assert.AreEqual(icmp.Data, fromFile.Data);
                Assert.AreEqual(icmp.HeaderData, fromFile.HeaderData);
                Assert.AreEqual(icmp.Id, fromFile.Id);
                Assert.AreEqual(icmp.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(icmp.Sequence, fromFile.Sequence);
                Assert.AreEqual(icmp.TypeCode, fromFile.TypeCode);
            }

            dev.Close();
            Assert.IsTrue(foundicmp, "Capture file contained no icmp packets");
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ICMPv4.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmp = p.Extract<IcmpV4Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmp.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ICMPv4.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmp = p.Extract<IcmpV4Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmp.ToString(StringOutputType.Verbose));
        }
    }
}