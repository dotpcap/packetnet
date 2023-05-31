/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class L2tpPacketTest
    {
        // L2TP
        [Test]
        public void L2tpParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "l2tp.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);

            var l2tp = p.Extract<L2tpPacket>();
            Assert.AreEqual(l2tp.TunnelId, 18994);
            Assert.AreEqual(l2tp.SessionId, 54110);
            Console.WriteLine(l2tp.GetType());
        }

        [Test]
        public void L2tpParsingWithUdpCustomDecoder()
        {
            TransportPacket.CustomPayloadDecoder = UdpPayloadCustomDecoderFunc;
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "l2tp.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);

            var l2tp = p.Extract<L2tpPacket>();
            Assert.AreEqual(l2tp.TunnelId, 18994);
            Assert.AreEqual(l2tp.SessionId, 54110);
            Console.WriteLine(l2tp.GetType());
        }
        
        private PacketOrByteArraySegment UdpPayloadCustomDecoderFunc(ByteArraySegment payload, TransportPacket udpPacket)
        {
            if (udpPacket is UdpPacket)
            {
                Console.WriteLine($"Udp Packet from {udpPacket.SourcePort} to {udpPacket.DestinationPort}");
                if (udpPacket.DestinationPort == L2tpFields.Port || udpPacket.SourcePort == L2tpFields.Port)
                {
                    return new PacketOrByteArraySegment { Packet = new L2tpPacket(payload, udpPacket) };
                }
            }

            return null;
        }
    }