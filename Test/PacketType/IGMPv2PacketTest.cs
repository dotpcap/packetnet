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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;
using System.Net;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class IGMPv2PacketTest
    {
        [Test]
        public void Parsing()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/IGMP dataset.pcap");
            dev.Open();

            RawCapture rawCapture;

            int packetIndex = 0;
            while((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                Assert.IsNotNull(p);

                var igmp = (IGMPv2Packet)p.Extract (typeof(IGMPv2Packet));
                Assert.IsNotNull(p);

                if(packetIndex == 0)
                {
                    Assert.AreEqual(igmp.Type, IGMPMessageType.MembershipQuery);
                    Assert.AreEqual(igmp.MaxResponseTime, 100);
                    Assert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[2] { 0xEE, 0x9B }, 0));
                    Assert.AreEqual(igmp.GroupAddress, IPAddress.Parse("0.0.0.0"));
                }

                if(packetIndex == 1)
                {
                    Assert.AreEqual(igmp.Type, IGMPMessageType.MembershipReportIGMPv2);
                    Assert.AreEqual(igmp.MaxResponseTime, 0.0);
                    Assert.AreEqual(igmp.Checksum, BitConverter.ToInt16(new byte[2] { 0x08, 0xC3 }, 0));
                    Assert.AreEqual(igmp.GroupAddress, IPAddress.Parse("224.0.1.60"));
                }

                if(packetIndex > 1)
                    break;

                packetIndex++;
            }
            dev.Close();
        }

         [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/IGMP dataset.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var igmpV2 = (IGMPv2Packet)p.Extract (typeof(IGMPv2Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(igmpV2.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/IGMP dataset.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var igmpV2 = (IGMPv2Packet)p.Extract (typeof(IGMPv2Packet));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(igmpV2.ToString(StringOutputType.Verbose));
        }
    }
}