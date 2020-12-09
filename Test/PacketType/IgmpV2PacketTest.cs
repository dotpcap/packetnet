/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class IgmpV2PacketTest
    {
        [Test]
        public void Parsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMP dataset.pcap");
            dev.Open();

            RawCapture rawCapture;

            var packetIndex = 0;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Assert.IsNotNull(p);

                var igmp = p.Extract<IgmpV2Packet>();
                Assert.IsNotNull(p);

                if (packetIndex == 0)
                {
                    Assert.AreEqual(igmp.Type, IgmpV2MessageType.MembershipQuery);
                    Assert.AreEqual(igmp.MaxResponseTime, 100);
                    Assert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[] { 0xEE, 0x9B }, 0));
                    Assert.AreEqual(igmp.GroupAddress, IPAddress.Parse("0.0.0.0"));
                }

                if (packetIndex == 1)
                {
                    Assert.AreEqual(igmp.Type, IgmpV2MessageType.MembershipReportIGMPv2);
                    Assert.AreEqual(igmp.MaxResponseTime, 0.0);
                    Assert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[] { 0x08, 0xC3 }, 0));
                    Assert.AreEqual(igmp.GroupAddress, IPAddress.Parse("224.0.1.60"));
                }

                if (packetIndex > 1)
                    break;


                packetIndex++;
            }

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMP dataset.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var igmp = p.Extract<IgmpV2Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(igmp.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMP dataset.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var igmp = p.Extract<IgmpV2Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(igmp.ToString(StringOutputType.Verbose));
        }
    }
}