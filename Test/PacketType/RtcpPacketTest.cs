/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

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
                        Packet = new RtcpContainerPacket(segment, packet)
                    };
                }

                return null;
            };

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            ClassicAssert.IsNotNull(p);

            var rtcpContainer = p.Extract<RtcpContainerPacket>();
            ClassicAssert.IsNotNull(rtcpContainer);
            Console.WriteLine(rtcpContainer.GetType());
            ClassicAssert.IsFalse(rtcpContainer.HasPayloadData);
            ClassicAssert.IsFalse(rtcpContainer.HasPayloadPacket);

            ClassicAssert.AreEqual(1,rtcpContainer.Packets.Count);
            var rtcp = rtcpContainer.Packets[0];
            ClassicAssert.IsTrue(rtcp.IsValid());
            ClassicAssert.AreEqual(2, rtcp.Version);
            ClassicAssert.IsFalse(rtcp.HasPadding);
            ClassicAssert.AreEqual(1, rtcp.ReceptionReportCount);
            ClassicAssert.AreEqual(RtcpFields.GoodbyeType, rtcp.PacketType);
            ClassicAssert.AreEqual(1, rtcp.Length);
            ClassicAssert.AreEqual(1199516466, rtcp.SsrcIdentifier);
            ClassicAssert.IsFalse(rtcp.HasPayloadData);
            ClassicAssert.IsFalse(rtcp.HasPayloadPacket);
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
                        Packet = new RtcpContainerPacket(segment, packet)
                    };
                }

                return null;
            };

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            ClassicAssert.IsNotNull(p);

            var rtcpContainer = p.Extract<RtcpContainerPacket>();
            ClassicAssert.IsNotNull(rtcpContainer);
            Console.WriteLine(rtcpContainer.GetType());
            ClassicAssert.IsFalse(rtcpContainer.HasPayloadData);
            ClassicAssert.IsFalse(rtcpContainer.HasPayloadPacket);

            ClassicAssert.AreEqual(2,rtcpContainer.Packets.Count);
            var rtcp = rtcpContainer.Packets[0];
            ClassicAssert.IsTrue(rtcp.IsValid());
            ClassicAssert.AreEqual(2, rtcp.Version);
            ClassicAssert.IsFalse(rtcp.HasPadding);
            ClassicAssert.AreEqual(0, rtcp.ReceptionReportCount);
            ClassicAssert.AreEqual(RtcpFields.SenderReportType, rtcp.PacketType);
            ClassicAssert.AreEqual(6, rtcp.Length);
            ClassicAssert.AreEqual(899629540, rtcp.SsrcIdentifier);
            ClassicAssert.IsTrue(rtcp.HasPayloadData);
            ClassicAssert.IsFalse(rtcp.HasPayloadPacket);

            var nextRtcp = rtcpContainer.Packets[1];
            ClassicAssert.IsNotNull(nextRtcp);
            Console.WriteLine(nextRtcp.GetType());
            ClassicAssert.IsTrue(nextRtcp.IsValid());
            ClassicAssert.AreEqual(2, nextRtcp.Version);
            ClassicAssert.IsFalse(nextRtcp.HasPadding);
            ClassicAssert.AreEqual(1, nextRtcp.ReceptionReportCount);
            ClassicAssert.AreEqual(RtcpFields.SourceDescriptionType, nextRtcp.PacketType);
            ClassicAssert.AreEqual(6, nextRtcp.Length);
            ClassicAssert.AreEqual(899629540, nextRtcp.SsrcIdentifier);
            ClassicAssert.IsTrue(nextRtcp.HasPayloadData);
            ClassicAssert.IsFalse(nextRtcp.HasPayloadPacket);
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
            ClassicAssert.IsNotNull(rtp);
            ClassicAssert.AreEqual(2, rtp.Version);
            ClassicAssert.IsFalse(rtp.HasPadding);
            ClassicAssert.IsFalse(rtp.HasExtension);
            ClassicAssert.AreEqual(3, rtp.CsrcCount);
            ClassicAssert.IsTrue(rtp.Marker);
            ClassicAssert.AreEqual(112, rtp.PayloadType);
            ClassicAssert.AreEqual(1234, rtp.SequenceNumber);
            ClassicAssert.AreEqual(1200, rtp.Timestamp);
            ClassicAssert.AreEqual(899629540, rtp.SsrcIdentifier);
            ClassicAssert.AreEqual(0, rtp.ExtensionHeaderLength);
            ClassicAssert.IsTrue(rtp.HasPayloadData);
            ClassicAssert.IsNotNull(rtp.PayloadData);
            ClassicAssert.AreEqual(12, rtp.PayloadData.Length);
        }
    }
