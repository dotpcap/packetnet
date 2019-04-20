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
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class WakeOnLanTest
    {
        [Test]
        public void CreateFromValues()
        {
            var physicalAddress = PhysicalAddress.Parse("CA-FE-BA-BE-C0-01");
            var wol = new WakeOnLanPacket(physicalAddress);

            Assert.IsTrue(wol.IsValid());
            Assert.AreEqual(wol.DestinationMAC, physicalAddress);

            // convert the wol packet back into bytes
            var wolBytes = wol.Bytes;

            // and now parse it back from bytes into a wol packet
            var wol2 = new WakeOnLanPacket(new ByteArraySegment(wolBytes));

            // make sure the packets match
            Assert.AreEqual(wol, wol2);
        }

        [Test]
        public void WakeOnLanParsing()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/wol.pcap");
            dev.Open();

            RawCapture rawCapture;

            int packetIndex = 0;
            while((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                Assert.IsNotNull(p);

                var wol = (WakeOnLanPacket)p.Extract(typeof(WakeOnLanPacket));
                Assert.IsNotNull(p);

                if(packetIndex == 0)
                    Assert.AreEqual(wol.DestinationMAC, PhysicalAddress.Parse("00-0D-56-DC-9E-35"));

                if(packetIndex == 3)
                    Assert.AreEqual(wol.DestinationMAC, PhysicalAddress.Parse("00-90-27-85-CF-01"));

                packetIndex++;
            }
            dev.Close();
        }

        [Test]
        public void RandomPacket()
        {
            WakeOnLanPacket.RandomPacket();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/wol.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var wol = (WakeOnLanPacket)p.Extract(typeof(WakeOnLanPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(wol.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/wol.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var wol = (WakeOnLanPacket)p.Extract (typeof(WakeOnLanPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(wol.ToString(StringOutputType.Verbose));
        }
    }
}