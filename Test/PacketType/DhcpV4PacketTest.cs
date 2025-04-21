/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.DhcpV4;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class DhcpV4PacketTest
    {
        [Test]
        public void DhcpV4InsideOfEthernetPacket()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "dhcp.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Console.WriteLine("Converted a raw packet to a Packet");
                Console.WriteLine(p.ToString());
                var dhcpV4Packet = p.Extract<DhcpV4Packet>();
                ClassicAssert.IsNotNull(dhcpV4Packet, "Expected packet to not be null");
            }

            dev.Close();
        }

        [Test]
        public void DhcpV4Data()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "dhcp.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            if ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var rawCapture = c.GetPacket();
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Console.WriteLine("Converted a raw packet to a Packet");
                Console.WriteLine(p.ToString());
                var dhcpV4Packet = p.Extract<DhcpV4Packet>();
                ClassicAssert.IsNotNull(dhcpV4Packet, "Expected packet to not be null");

                ClassicAssert.AreEqual(DhcpV4MessageType.Discover, dhcpV4Packet.MessageType);
                dhcpV4Packet.MessageType = DhcpV4MessageType.Ack;
                ClassicAssert.AreEqual(DhcpV4MessageType.Ack, dhcpV4Packet.MessageType);

                ClassicAssert.AreEqual(0, dhcpV4Packet.Hops);
                dhcpV4Packet.Hops = 4;
                ClassicAssert.AreEqual(4, dhcpV4Packet.Hops);

                ClassicAssert.AreEqual(0x00003d1d, dhcpV4Packet.TransactionId);
                dhcpV4Packet.TransactionId = 100_000_000;
                ClassicAssert.AreEqual(100_000_000, dhcpV4Packet.TransactionId);

                ClassicAssert.AreEqual(PhysicalAddress.Parse("00-0B-82-01-FC-42"), dhcpV4Packet.ClientHardwareAddress);
                dhcpV4Packet.ClientHardwareAddress = PhysicalAddress.Parse("11-22-33-44-55-66");
                ClassicAssert.AreEqual(PhysicalAddress.Parse("11-22-33-44-55-66"), dhcpV4Packet.ClientHardwareAddress);

                ClassicAssert.AreEqual(IPAddress.Any, dhcpV4Packet.ClientAddress);
                ClassicAssert.AreEqual(IPAddress.Any, dhcpV4Packet.ServerAddress);
                ClassicAssert.AreEqual(IPAddress.Any, dhcpV4Packet.YourAddress);
                ClassicAssert.AreEqual(IPAddress.Any, dhcpV4Packet.GatewayAddress);

                var testIP = IPAddress.Parse("1.2.3.4");
                dhcpV4Packet.ClientAddress = testIP;
                dhcpV4Packet.ServerAddress = testIP;
                dhcpV4Packet.YourAddress = testIP;
                dhcpV4Packet.GatewayAddress = testIP;

                ClassicAssert.AreEqual(testIP, dhcpV4Packet.ClientAddress);
                ClassicAssert.AreEqual(testIP, dhcpV4Packet.ServerAddress);
                ClassicAssert.AreEqual(testIP, dhcpV4Packet.YourAddress);
                ClassicAssert.AreEqual(testIP, dhcpV4Packet.GatewayAddress);

                var options = dhcpV4Packet.GetOptions();

                var addressRequestOption = options.OfType<AddressRequestOption>().FirstOrDefault();
                ClassicAssert.NotNull(addressRequestOption);
                ClassicAssert.AreEqual(IPAddress.Any, addressRequestOption.RequestedIP);
                
                var clientIdOption = options.OfType<ClientIdOption>().FirstOrDefault();
                ClassicAssert.NotNull(clientIdOption);

                dhcpV4Packet.SetOptions(new List<DhcpV4Option> {new MessageTypeOption(DhcpV4MessageType.Discover), new AddressTimeOption(TimeSpan.FromDays(1)) });
                ClassicAssert.AreEqual(2, dhcpV4Packet.GetOptions().Count);
            }

            dev.Close();
        }
    }
