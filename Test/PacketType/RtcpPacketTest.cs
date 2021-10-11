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
    public class RtcpPacketTest
    {
        // RTCP
        [Test]
        public void RtcpGoodByeParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_rtcp1.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            TransportPacket.CustomPayloadDecoder = (segment, packet) =>
            {
                if (packet is UdpPacket && packet.DestinationPort == 6001)
                {
                    return new PacketOrByteArraySegment
                    {
                        Packet = new RtcpPacket(segment, packet)
                    };
                }

                return null;
            };

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);

            var rtcp = p.Extract<RtcpPacket>();
            Assert.IsNotNull(rtcp);
            Console.WriteLine(rtcp.GetType());
            Assert.IsTrue(rtcp.IsValid());
            Assert.AreEqual(2, rtcp.Version);
            Assert.IsFalse(rtcp.HasPadding);
            Assert.AreEqual(1, rtcp.ReceptionReportCount);
            Assert.AreEqual(203, rtcp.PacketType);
            Assert.AreEqual(1, rtcp.Length);
            Assert.AreEqual(1199516466, rtcp.SsrcIdentifier);
            Assert.IsFalse(rtcp.HasPayloadData);
            Assert.IsFalse(rtcp.HasPayloadPacket);
        }

        // RTCP
        [Test]
        public void RtcpReportsParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_rtcp2.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            TransportPacket.CustomPayloadDecoder = (segment, packet) =>
            {
                if (packet is UdpPacket && packet.DestinationPort == 6001)
                {
                    return new PacketOrByteArraySegment
                    {
                        Packet = new RtcpPacket(segment, packet)
                    };
                }

                return null;
            };

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);

            var rtcp = p.Extract<RtcpPacket>();
            Assert.IsNotNull(rtcp);
            Console.WriteLine(rtcp.GetType());
            Assert.IsTrue(rtcp.IsValid());
            Assert.AreEqual(2, rtcp.Version);
            Assert.IsFalse(rtcp.HasPadding);
            Assert.AreEqual(0, rtcp.ReceptionReportCount);
            Assert.AreEqual(200, rtcp.PacketType);
            Assert.AreEqual(6, rtcp.Length);
            Assert.AreEqual(899629540, rtcp.SsrcIdentifier);
            Assert.IsFalse(rtcp.HasPayloadData);
            Assert.IsTrue(rtcp.HasPayloadPacket);

            var nextRtcp = rtcp.PayloadPacket as RtcpPacket;
            Assert.IsNotNull(nextRtcp);
            Console.WriteLine(nextRtcp.GetType());
            Assert.IsTrue(nextRtcp.IsValid());
            Assert.AreEqual(2, nextRtcp.Version);
            Assert.IsFalse(nextRtcp.HasPadding);
            Assert.AreEqual(1, nextRtcp.ReceptionReportCount);
            Assert.AreEqual(202, nextRtcp.PacketType);
            Assert.AreEqual(6, nextRtcp.Length);
            Assert.AreEqual(899629540, nextRtcp.SsrcIdentifier);
            Assert.IsFalse(nextRtcp.HasPayloadData);
            Assert.IsFalse(nextRtcp.HasPayloadPacket);
        }

        [Test]
        public void ConstructRtpPacketFromValues()
        {
            byte[] data = new byte[] { 0xef, 0x00, 0x00, 0x00, 0xef, 0x00, 0x00, 0x00, 0x6f, 0xef, 0xbb, 0xbf };

            var rtp = new RtpPacket
            {
                PayloadData = data,
                Version = 2,
                HasPadding = false,
                HasExtension = false,
                CsrcCount = 3,
                Marker = true,
                PayloadType = 112,
                SequenceNumber = 1234,
                Timestamp = 1200,
                SsrcIdentifier = 899629540
            };
            Assert.IsNotNull(rtp);
            Assert.AreEqual(2, rtp.Version);
            Assert.IsFalse(rtp.HasPadding);
            Assert.IsFalse(rtp.HasExtension);
            Assert.AreEqual(3, rtp.CsrcCount);
            Assert.IsTrue(rtp.Marker);
            Assert.AreEqual(112, rtp.PayloadType);
            Assert.AreEqual(1234, rtp.SequenceNumber);
            Assert.AreEqual(1200, rtp.Timestamp);
            Assert.AreEqual(899629540, rtp.SsrcIdentifier);
            Assert.AreEqual(0, rtp.ExtensionHeaderLength);
            Assert.IsTrue(rtp.HasPayloadData);
            Assert.IsNotNull(rtp.PayloadData);
            Assert.AreEqual(12, rtp.PayloadData.Length);
        }
    }
}
