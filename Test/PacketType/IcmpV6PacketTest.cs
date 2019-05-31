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
    public class IcmpV6PacketTest
    {
        /// <summary>
        /// Test that the checksum can be recalculated properly
        /// </summary>
        [Test]
        public void Checksum()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            // save the checksum
            var icmpv6 = p.Extract<IcmpV6Packet>();
            Assert.IsNotNull(icmpv6);
            var savedChecksum = icmpv6.Checksum;

            // now zero the checksum out
            icmpv6.Checksum = 0;

            // and recalculate the checksum
            icmpv6.UpdateCalculatedValues();

            // compare the checksum values to ensure that they match
            Assert.AreEqual(savedChecksum, icmpv6.Checksum);
        }

        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [Test]
        public void IcmpV6Parsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var icmpv6 = p.Extract<IcmpV6Packet>();
            Console.WriteLine(icmpv6.GetType());

            Assert.AreEqual(IcmpV6Type.RouterSolicitation, icmpv6.Type);
            Assert.AreEqual(0, icmpv6.Code);
            Assert.AreEqual(0x5d50, icmpv6.Checksum);

            // Payload differs based on the icmp.Type field
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpv6 = p.Extract<IcmpV6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpv6.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpV6 = p.Extract<IcmpV6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString(StringOutputType.Verbose));
        }
    }
}