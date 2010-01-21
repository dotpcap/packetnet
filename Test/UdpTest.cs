using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test
{
    [TestFixture]
    public class UdpTest
    {
        [Test]
        public void UDPData()
        {
            SharpPcap.Packets.RawPacket rawPacket;
            UdpPacket u;
            Packet p;

            var dev = new OfflinePcapDevice("../../CaptureFiles/udp_dns_request_response.pcap");
            dev.Open();

            // check the first packet
            rawPacket = dev.GetNextRawPacket();

            p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                   new PosixTimeval(rawPacket.Timeval.Seconds,
                                                    rawPacket.Timeval.MicroSeconds),
                                   rawPacket.Data);
            Assert.IsNotNull(p);
            Assert.IsTrue(UdpPacket.IsType(p));

            u = UdpPacket.GetType(p);
            Assert.AreEqual(41 - u.Header.Length,
                            u.PayloadData.Length, "UDPData.Length mismatch");

            // check the second packet
            rawPacket = dev.GetNextRawPacket();
            p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                    new PosixTimeval(rawPacket.Timeval.Seconds,
                                                     rawPacket.Timeval.MicroSeconds),
                                    rawPacket.Data);

            Assert.IsNotNull(p);
            Assert.IsTrue(UdpPacket.IsType(p));

            u = UdpPacket.GetType(p);
            Assert.AreEqual(356 - u.Header.Length,
                            u.PayloadData.Length, "UDPData.Length mismatch");

            Console.WriteLine("u is {0}", u.ToString());

            dev.Close();
        }

        [Test]
        public void RandomPacket()
        {
            UdpPacket.RandomPacket();
        }
    }
}
