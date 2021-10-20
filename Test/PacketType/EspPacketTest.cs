/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class EspPacketTest
    {
        [Test]
        public void EspParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_esp.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);
            var esp = p.Extract<EspPacket>();
            Assert.IsNotNull(esp);
            Console.WriteLine(esp.GetType());
            Assert.AreEqual(156633505, esp.SecurityParametersIndex);
            Assert.AreEqual(1, esp.SequenceNumber);
            Assert.AreEqual(ProtocolType.Tcp, esp.NextHeader);
            Assert.AreEqual(2, esp.PadLength);
            Assert.IsNotNull(esp.PayloadPacket);
            var tcp = p.Extract<TcpPacket>();
            Assert.IsNotNull(tcp);
            Assert.AreEqual(0, tcp.UrgentPointer);
            Assert.AreEqual(16000, tcp.WindowSize);
            Assert.AreEqual(4, tcp.OptionsCollection.Count);
        }

        [Test]
        public void ConstructEspPacketFromValues()
        {
            var esp = new EspPacket
            {
                SequenceNumber = 1,
                SecurityParametersIndex = 156633505,
                NextHeader = ProtocolType.Tcp,
            };
        }
    }
}
