/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class IgmpV2PacketTest
    {
        [Test]
        public void Parsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMP dataset.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                ClassicAssert.IsNotNull(p);

                var igmp = p.Extract<IgmpV2Packet>();
                ClassicAssert.IsNotNull(p);

                if (packetIndex == 0)
                {
                    ClassicAssert.AreEqual(igmp.Type, IgmpMessageType.MembershipQuery);
                    ClassicAssert.AreEqual(igmp.MaxResponseTime, 100);
                    ClassicAssert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[] { 0xEE, 0x9B }, 0));
                    ClassicAssert.AreEqual(igmp.GroupAddress, IPAddress.Parse("0.0.0.0"));
                }

                if (packetIndex == 1)
                {
                    ClassicAssert.AreEqual(igmp.Type, IgmpMessageType.MembershipReportIGMPv2);
                    ClassicAssert.AreEqual(igmp.MaxResponseTime, 0.0);
                    ClassicAssert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[] { 0x08, 0xC3 }, 0));
                    ClassicAssert.AreEqual(igmp.GroupAddress, IPAddress.Parse("224.0.1.60"));
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
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
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
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var igmp = p.Extract<IgmpV2Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(igmp.ToString(StringOutputType.Verbose));
        }
    }
