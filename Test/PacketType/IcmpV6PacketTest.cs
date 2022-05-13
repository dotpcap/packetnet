/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Net;
using System.Net.NetworkInformation;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Ndp;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class IcmpV6PacketTest
    {
        /// <summary>
        /// Test that the checksum can be recalculated properly
        /// </summary>
        [Test]
        public void Checksum()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            dev.GetNextPacket(out PacketCapture c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            // save the checksum
            var icmpv6 = p.Extract<IcmpV6Packet>();
            Assert.IsNotNull(icmpv6);
            var savedChecksum = icmpv6.Checksum;
            Assert.True(icmpv6.ValidIcmpChecksum);

            // now zero the checksum out
            icmpv6.Checksum = 0;

            // and recalculate the checksum
            icmpv6.UpdateIcmpChecksum();

            // compare the checksum values to ensure that they match
            Assert.AreEqual(savedChecksum, icmpv6.Checksum);
        }

        /// <summary>
        /// Test that we can parse a icmp v4 request and reply
        /// </summary>
        [Test]
        public void IcmpV6Parsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            dev.GetNextPacket(out PacketCapture c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Assert.IsNotNull(p);

            var icmpv6 = p.Extract<IcmpV6Packet>();
            Console.WriteLine(icmpv6.GetType());

            Assert.AreEqual(4, icmpv6.HeaderSegment.Length);

            Assert.AreEqual(IcmpV6Type.RouterSolicitation, icmpv6.Type);
            Assert.AreEqual(0, icmpv6.Code);
            Assert.AreEqual(0x5d50, icmpv6.Checksum);

            // Payload differs based on the icmp.Type field
        }

        [Test]
        public void NdpParsing()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_ndp.pcap");
            dev.Open();

            // Neighbor solicitation.
            dev.GetNextPacket(out PacketCapture ns);
            var nsRawCapture = ns.GetPacket();

            var pNs = Packet.ParsePacket(nsRawCapture.GetLinkLayers(), nsRawCapture.Data);
            Assert.IsNotNull(pNs);

            var neighborSolicitationPacket = pNs.Extract<NdpNeighborSolicitationPacket>();
            Assert.IsNotNull(neighborSolicitationPacket);
            Assert.AreEqual(IPAddress.Parse("fe80::c000:54ff:fef5:0"), neighborSolicitationPacket.TargetAddress);
            neighborSolicitationPacket.TargetAddress = IPAddress.Parse("fd00::fd00:fd00:fd00:0");
            Assert.AreEqual(IPAddress.Parse("fd00::fd00:fd00:fd00:0"), neighborSolicitationPacket.TargetAddress);

            // Neighbor advertisement.
            dev.GetNextPacket(out PacketCapture na);
            var naRawCapture = na.GetPacket();

            var pNa = Packet.ParsePacket(naRawCapture.GetLinkLayers(), naRawCapture.Data);
            Assert.IsNotNull(pNa);

            var neighborAdvertisementPacket = pNa.Extract<NdpNeighborAdvertisementPacket>();

            Assert.AreEqual(IPAddress.Parse("fe80::c000:54ff:fef5:0"), neighborAdvertisementPacket.TargetAddress);
            neighborAdvertisementPacket.TargetAddress = IPAddress.Parse("fd00::fd00:fd00:fd00:0");
            Assert.AreEqual(IPAddress.Parse("fd00::fd00:fd00:fd00:0"), neighborAdvertisementPacket.TargetAddress);

            Assert.True(neighborAdvertisementPacket.Router);
            Assert.True(neighborAdvertisementPacket.Override);
            Assert.False(neighborAdvertisementPacket.Solicited);

            neighborAdvertisementPacket.Router = false;
            Assert.False(neighborAdvertisementPacket.Router);
            Assert.True(neighborAdvertisementPacket.Override);
            Assert.False(neighborAdvertisementPacket.Solicited);

            Assert.AreEqual(1, neighborAdvertisementPacket.OptionsCollection.Count);
            var option = neighborAdvertisementPacket.OptionsCollection[0];
            Assert.IsInstanceOf(typeof(NdpLinkLayerAddressOption), option);

            var ndLinkLayerAddressOption = (NdpLinkLayerAddressOption) option;
            Assert.AreEqual(PhysicalAddress.Parse("C20054F50000"), ndLinkLayerAddressOption.LinkLayerAddress);

            // Router advertisement.
            dev.GetNextPacket(out PacketCapture ra);
            var raRawCapture = ra.GetPacket();

            var pRa = Packet.ParsePacket(raRawCapture.GetLinkLayers(), raRawCapture.Data);
            Assert.IsNotNull(pRa);

            var routerAdvertisementPacket = pRa.Extract<NdpRouterAdvertisementPacket>();
            Assert.AreEqual(0, routerAdvertisementPacket.ReachableTime);
            routerAdvertisementPacket.ReachableTime = 1234;
            Assert.AreEqual(1234, routerAdvertisementPacket.ReachableTime);

            Assert.AreEqual(1800, routerAdvertisementPacket.RouterLifetime);
            Assert.AreEqual(64, routerAdvertisementPacket.CurrentHopLimit);
            Assert.False(routerAdvertisementPacket.ManagedAddressConfiguration);
            Assert.False(routerAdvertisementPacket.OtherConfiguration);

            Assert.AreEqual(3, routerAdvertisementPacket.OptionsCollection.Count);

            var prefixOption = routerAdvertisementPacket.OptionsCollection.Find(x => x is NdpLinkPrefixInformationOption) as NdpLinkPrefixInformationOption;
            Assert.NotNull(prefixOption);

            Assert.AreEqual(64, prefixOption.PrefixLength);
            Assert.True(prefixOption.OnLink);
            Assert.True(prefixOption.AutonomousAddressConfiguration);
            Assert.AreEqual(IPAddress.Parse("2001:db8:0:1::"), prefixOption.Prefix);

            var mtuOption = routerAdvertisementPacket.OptionsCollection.Find(x => x is NdpMtuOption) as NdpMtuOption;
            Assert.NotNull(mtuOption);

            Assert.AreEqual(1500, mtuOption.Mtu);
            mtuOption.Mtu = 1400;
            Assert.AreEqual(1400, mtuOption.Mtu);

            dev.Close();

            // Redirect message.
            dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_redirect.pcap");
            dev.Open();

            for (int i = 0; i < 10; i++)
            {
                dev.GetNextPacket(out _);
            }

            dev.GetNextPacket(out PacketCapture rm);
            var rmRawCapture = rm.GetPacket();

            var pRm = Packet.ParsePacket(rmRawCapture.GetLinkLayers(), rmRawCapture.Data);
            Assert.IsNotNull(pRm);

            var redirectMessagePacket = pRm.Extract<NdpRedirectMessagePacket>();
            Assert.NotNull(redirectMessagePacket);

            Assert.AreEqual(IPAddress.Parse("fe80::20c:29ff:fefc:2c3b"), redirectMessagePacket.TargetAddress);
            Assert.AreEqual(IPAddress.Parse("2001:db8:2::1"), redirectMessagePacket.DestinationAddress);

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket(out PacketCapture c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpv6 = p.Extract<IcmpV6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpv6.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ipv6_icmpv6_packet.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            dev.GetNextPacket(out PacketCapture c);
            var rawCapture = c.GetPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var icmpV6 = p.Extract<IcmpV6Packet>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(icmpV6.ToString(StringOutputType.Verbose));
        }
    }
}