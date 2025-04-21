/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

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
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            // Parse an icmp request
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            ClassicAssert.IsNotNull(p);
            ClassicAssert.AreEqual(linkLayers, rawCapture.GetLinkLayers());

            var icmp = p.Extract<IcmpV4Packet>();
            Console.WriteLine(icmp.GetType());

            ClassicAssert.AreEqual(IcmpV4TypeCode.EchoRequest, icmp.TypeCode);
            ClassicAssert.AreEqual(0xe05b, icmp.Checksum);
            ClassicAssert.AreEqual(0x0200, icmp.Id);
            ClassicAssert.AreEqual(0x6b00, icmp.Sequence);
            ClassicAssert.True(icmp.ValidIcmpChecksum);

            // check that the message matches
            const string expectedString = "abcdefghijklmnopqrstuvwabcdefghi";
            var expectedData = System.Text.Encoding.ASCII.GetBytes(expectedString);
            ClassicAssert.AreEqual(expectedData, icmp.Data);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ICMPv4.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

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
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmp = p.Extract<IcmpV4Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmp.ToString(StringOutputType.Verbose));
        }
    }