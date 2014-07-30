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
using SharpPcap.LibPcap;
using PacketDotNet;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ICMPv4.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            // Parse an icmp request
            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var icmp = (ICMPv4Packet)p.Extract (typeof(ICMPv4Packet));
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

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ICMPv4.pcap");
            dev.Open();
            RawCapture rawCapture;
            Console.WriteLine("Reading packet data");
            rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmp = (ICMPv4Packet)p.Extract(typeof(ICMPv4Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmp.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ICMPv4.pcap");
            dev.Open();
            RawCapture rawCapture;
            Console.WriteLine("Reading packet data");
            rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmp = (ICMPv4Packet)p.Extract (typeof(ICMPv4Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmp.ToString(StringOutputType.Verbose));
        }
        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ICMPv4.pcap");
            dev.Open();

            RawCapture rawCapture;
            bool foundicmp = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                var icmp = (ICMPv4Packet)p.Extract(typeof(ICMPv4Packet));
                if (icmp == null)
                {
                    continue;
                }
                foundicmp = true;

                var memoryStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, icmp);

                memoryStream.Seek (0, SeekOrigin.Begin);
                BinaryFormatter deserializer = new BinaryFormatter();
                ICMPv4Packet fromFile = (ICMPv4Packet)deserializer.Deserialize(memoryStream);

                Assert.AreEqual(icmp.Bytes, fromFile.Bytes);
                Assert.AreEqual(icmp.BytesHighPerformance.Bytes, fromFile.BytesHighPerformance.Bytes);
                Assert.AreEqual(icmp.BytesHighPerformance.BytesLength, fromFile.BytesHighPerformance.BytesLength);
                Assert.AreEqual(icmp.BytesHighPerformance.Length, fromFile.BytesHighPerformance.Length);
                Assert.AreEqual(icmp.BytesHighPerformance.NeedsCopyForActualBytes, fromFile.BytesHighPerformance.NeedsCopyForActualBytes);
                Assert.AreEqual(icmp.BytesHighPerformance.Offset, fromFile.BytesHighPerformance.Offset);
                Assert.AreEqual(icmp.Checksum, fromFile.Checksum);
                Assert.AreEqual(icmp.Color, fromFile.Color);
                Assert.AreEqual(icmp.Data, fromFile.Data);
                Assert.AreEqual(icmp.Header, fromFile.Header);
                Assert.AreEqual(icmp.ID, fromFile.ID);
                Assert.AreEqual(icmp.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(icmp.Sequence, fromFile.Sequence);
                Assert.AreEqual(icmp.TypeCode, fromFile.TypeCode);
            }

            dev.Close();
            Assert.IsTrue(foundicmp, "Capture file contained no icmp packets");
        }
    
    }
}

