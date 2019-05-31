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