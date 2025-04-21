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
    public class IgmpV3PacketTest
    {
        [Test]
        public void Parsing()
        {
            bool membershipReportTested = false;
            bool membershipQueryTested = false;

            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMPv3_Multihost.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;

            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                ClassicAssert.IsNotNull(p);

                var igmp = p.Extract<PacketDotNet.IgmpPacket>();
                ClassicAssert.IsNotNull(igmp);

                if (packetIndex == 0 && igmp.Type == IgmpMessageType.MembershipReportIGMPv3)
                {
                    IgmpV3MembershipReportPacket igmpv3MemRep = (IgmpV3MembershipReportPacket)igmp;

                    ClassicAssert.AreEqual(igmpv3MemRep.Type, IgmpMessageType.MembershipReportIGMPv3);
                    ClassicAssert.AreEqual(igmpv3MemRep.Checksum, BitConverter.ToInt16(new byte[] { 0x4E, 0xEB }, 0));
                    ClassicAssert.AreEqual(igmpv3MemRep.NumberOfGroupRecords, 1);
                    ClassicAssert.AreEqual(igmpv3MemRep.GroupRecords[0].RecordType, IgmpV3MembershipReportGroupRecordType.ChangeToIncludeMode);
                    ClassicAssert.AreEqual(igmpv3MemRep.GroupRecords[0].AuxiliaryDataLength, 0);
                    ClassicAssert.AreEqual(igmpv3MemRep.GroupRecords[0].NumberOfSources, 1);
                    ClassicAssert.AreEqual(igmpv3MemRep.GroupRecords[0].MulticastAddress, IPAddress.Parse("232.2.3.2"));
                    ClassicAssert.AreEqual(igmpv3MemRep.GroupRecords[0].SourceAddresses[0], IPAddress.Parse("192.168.224.100"));

                    membershipReportTested = true;
                }

                if (packetIndex == 20 && igmp.Type == IgmpMessageType.MembershipQuery)
                {
                    IgmpV3MembershipQueryPacket igmpv3MemQuery = (IgmpV3MembershipQueryPacket)igmp;

                    ClassicAssert.AreEqual(igmpv3MemQuery.Type, IgmpMessageType.MembershipQuery);
                    ClassicAssert.AreEqual(igmpv3MemQuery.MaxResponseTime, 10);
                    ClassicAssert.AreEqual(igmpv3MemQuery.Checksum, BitConverter.ToInt16(new byte[] { 0x60, 0x42 }, 0));
                    ClassicAssert.AreEqual(igmpv3MemQuery.GroupAddress, IPAddress.Parse("232.2.3.2"));
                    ClassicAssert.AreEqual(igmpv3MemQuery.SuppressRouterSideProcessingFlag, false);
                    ClassicAssert.AreEqual(igmpv3MemQuery.QueriersRobustnessVariable, 2);
                    ClassicAssert.AreEqual(igmpv3MemQuery.QueriersQueryInterval, 60);
                    ClassicAssert.AreEqual(igmpv3MemQuery.NumberOfSources, 1);
                    ClassicAssert.AreEqual(igmpv3MemQuery.SourceAddresses[0], IPAddress.Parse("192.168.224.200"));

                    membershipQueryTested = true;
                }

                if (packetIndex == 20)
                    break;

                packetIndex++;
            }

            ClassicAssert.IsTrue(membershipQueryTested && membershipReportTested);

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMPv3_Multihost.pcap");
            dev.Open();
            
            Console.WriteLine("Reading packet data");
            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;

            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

                Console.WriteLine("Parsing");

                if (packetIndex == 0)
                {
                    var igmpv3MemRep = p.Extract<IgmpV3MembershipReportPacket>();
                    Console.WriteLine("Printing human readable string");
                    Console.WriteLine(igmpv3MemRep.ToString());
                }

                if (packetIndex == 20)
                {
                    var igmpv3MemQuery = p.Extract<IgmpV3MembershipQueryPacket>();
                    Console.WriteLine("Printing human readable string");
                    Console.WriteLine(igmpv3MemQuery.ToString());
                }

                if (packetIndex == 20)
                    break;

                packetIndex++;
            }

            dev.Close();
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "IGMPv3_Multihost.pcap");
            dev.Open();

            Console.WriteLine("Reading packet data");
            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;

            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

                Console.WriteLine("Parsing");

                if (packetIndex == 0)
                {
                    var igmpv3MemRep = p.Extract<IgmpV3MembershipReportPacket>();
                    Console.WriteLine("Printing human readable string");
                    Console.WriteLine(igmpv3MemRep.ToString(StringOutputType.Verbose));
                }

                if (packetIndex == 20)
                {
                    var igmpv3MemQuery = p.Extract<IgmpV3MembershipQueryPacket>();
                    Console.WriteLine("Printing human readable string");
                    Console.WriteLine(igmpv3MemQuery.ToString(StringOutputType.Verbose));
                }

                if (packetIndex == 20)
                    break;

                packetIndex++;
            }

            dev.Close();
        }
    }
