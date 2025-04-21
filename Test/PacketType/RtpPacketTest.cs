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
    public class RtpPacketTest
    {
        // RTP
        [Test]
        public void RtpParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_rtp.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            TransportPacket.CustomPayloadDecoder = (segment, packet) =>
            {
                if (packet is UdpPacket && packet.DestinationPort == 6000)
                {
                    return new PacketOrByteArraySegment
                    {
                        Packet = new RtpPacket(segment, packet)
                    };
                }

                return null;
            };

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            ClassicAssert.IsNotNull(p);

            var rtp = p.Extract<RtpPacket>();
            ClassicAssert.IsNotNull(rtp);
            Console.WriteLine(rtp.GetType());
            ClassicAssert.AreEqual(2, rtp.Version);
            ClassicAssert.IsFalse(rtp.HasPadding);
            ClassicAssert.IsFalse(rtp.HasExtension);
            ClassicAssert.AreEqual(0, rtp.CsrcCount);
            ClassicAssert.IsTrue(rtp.Marker);
            ClassicAssert.AreEqual(112, rtp.PayloadType);
            ClassicAssert.AreEqual(0, rtp.SequenceNumber);
            ClassicAssert.AreEqual(600, rtp.Timestamp);
            ClassicAssert.AreEqual(899629540, rtp.SsrcIdentifier);
            ClassicAssert.AreEqual(0, rtp.ExtensionHeaderLength);
            ClassicAssert.IsTrue(rtp.HasPayloadData);
            ClassicAssert.IsNotNull(rtp.PayloadData);
            ClassicAssert.AreEqual(12, rtp.PayloadData.Length);
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
