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
    public class ICMPv6PacketTest
    {
        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [Test]
        public void ICMPv6Parsing ()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var icmp = (ICMPv6Packet)p.Extract(typeof(ICMPv6Packet));
            Console.WriteLine(icmp.GetType());

            Assert.AreEqual(ICMPv6Types.RouterSolicitation, icmp.Type);
            Assert.AreEqual(0, icmp.Code);
            Assert.AreEqual(0x5d50, icmp.Checksum);

            // Payload differs based on the icmp.Type field
        }

        /// <summary>
        /// Test that the checksum can be recalculated properly
        /// </summary>
        [Test]
        public void Checksum()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            // save the checksum
            var icmpv6 = (ICMPv6Packet)p.Extract(typeof(ICMPv6Packet));
            Assert.IsNotNull(icmpv6);
            var savedChecksum = icmpv6.Checksum;

            // now zero the checksum out
            icmpv6.Checksum = 0;

            // and recalculate the checksum
            icmpv6.UpdateCalculatedValues();

            // compare the checksum values to ensure that they match
            Assert.AreEqual(savedChecksum, icmpv6.Checksum);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            RawCapture rawCapture;
            Console.WriteLine("Reading packet data");
            rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpV6 = (ICMPv6Packet)p.Extract (typeof(ICMPv6Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();
            RawCapture rawCapture;
            Console.WriteLine("Reading packet data");
            rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpV6 = (ICMPv6Packet)p.Extract (typeof(ICMPv6Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString(StringOutputType.Verbose));
        }
        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv6_icmpv6_packet.pcap");
            dev.Open();

            RawCapture rawCapture;
            bool foundicmpv6 = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {

                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                var icmpv6 = (ICMPv6Packet)p.Extract(typeof(ICMPv6Packet));
                if (icmpv6 == null)
                {
                    continue;
                }
                foundicmpv6 = true;

                Stream outFile = File.Create("icmpv6.dat");
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(outFile, icmpv6);
                outFile.Close();

                Stream inFile = File.OpenRead("icmpv6.dat");
                BinaryFormatter deserializer = new BinaryFormatter();
                ICMPv6Packet fromFile = (ICMPv6Packet)deserializer.Deserialize(inFile);
                inFile.Close();


                Assert.AreEqual(icmpv6.Bytes, fromFile.Bytes);
                Assert.AreEqual(icmpv6.BytesHighPerformance.Bytes, fromFile.BytesHighPerformance.Bytes);
                Assert.AreEqual(icmpv6.BytesHighPerformance.BytesLength, fromFile.BytesHighPerformance.BytesLength);
                Assert.AreEqual(icmpv6.BytesHighPerformance.Length, fromFile.BytesHighPerformance.Length);
                Assert.AreEqual(icmpv6.BytesHighPerformance.NeedsCopyForActualBytes, fromFile.BytesHighPerformance.NeedsCopyForActualBytes);
                Assert.AreEqual(icmpv6.BytesHighPerformance.Offset, fromFile.BytesHighPerformance.Offset);
                Assert.AreEqual(icmpv6.Checksum, fromFile.Checksum);
                Assert.AreEqual(icmpv6.Color, fromFile.Color);
                Assert.AreEqual(icmpv6.Header, fromFile.Header);
                Assert.AreEqual(icmpv6.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(icmpv6.Code, fromFile.Code);
                Assert.AreEqual(icmpv6.Type, fromFile.Type);



            }

            dev.Close();
            Assert.IsTrue(foundicmpv6, "Capture file contained no icmpv6 packets");


        }
    
    }
}

