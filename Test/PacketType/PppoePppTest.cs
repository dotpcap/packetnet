/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class PppoePppTest
    {
        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var pppoe = p.Extract<PppoePacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "PPPoEPPP.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket();
            var rawCapture = dev.GetNextPacket();
            dev.Close();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var pppoe = p.Extract<PppoePacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(pppoe.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void TestParsingPppoePppPacket()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "PPPoEPPP.pcap");
            dev.Open();

            // first packet is a udp packet
            var rawCapture = dev.GetNextPacket();
            var packet = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var udpPacket = packet.Extract<UdpPacket>();
            Assert.IsNotNull(udpPacket, "Expected a valid udp packet for the first packet");

            // second packet is the PPPoe Ptp packet
            rawCapture = dev.GetNextPacket();
            packet = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
            var anotherUdpPacket = packet.Extract<UdpPacket>();
            Assert.IsNotNull(anotherUdpPacket, "Expected a valid udp packet for the second packet as well");

            dev.Close();
        }
    }
}