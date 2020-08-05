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
 *  This file is licensed under the Apache License, Version 2.0.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.DhcpV4;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class DhcpV4PacketTest
    {
        [Test]
        public void DhcpV4InsideOfEthernetPacket()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "dhcp.pcap");
            dev.Open();

            RawCapture rawCapture;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Console.WriteLine("Converted a raw packet to a Packet");
                Console.WriteLine(p.ToString());
                var dhcpV4Packet = p.Extract<DhcpV4Packet>();
                Assert.IsNotNull(dhcpV4Packet, "Expected packet to not be null");
            }

            dev.Close();
        }

        [Test]
        public void DhcpV4Data()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "dhcp.pcap");
            dev.Open();

            RawCapture rawCapture;
            if ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                Console.WriteLine("Converted a raw packet to a Packet");
                Console.WriteLine(p.ToString());
                var dhcpV4Packet = p.Extract<DhcpV4Packet>();
                Assert.IsNotNull(dhcpV4Packet, "Expected packet to not be null");

                Assert.AreEqual(DhcpV4MessageType.Discover, dhcpV4Packet.MessageType);
                dhcpV4Packet.MessageType = DhcpV4MessageType.Ack;
                Assert.AreEqual(DhcpV4MessageType.Ack, dhcpV4Packet.MessageType);

                Assert.AreEqual(0, dhcpV4Packet.Hops);
                dhcpV4Packet.Hops = 4;
                Assert.AreEqual(4, dhcpV4Packet.Hops);

                Assert.AreEqual(0x00003d1d, dhcpV4Packet.TransactionId);
                dhcpV4Packet.TransactionId = 100_000_000;
                Assert.AreEqual(100_000_000, dhcpV4Packet.TransactionId);

                Assert.AreEqual(PhysicalAddress.Parse("00-0B-82-01-FC-42"), dhcpV4Packet.ClientHardwareAddress);
                dhcpV4Packet.ClientHardwareAddress = PhysicalAddress.Parse("11-22-33-44-55-66");
                Assert.AreEqual(PhysicalAddress.Parse("11-22-33-44-55-66"), dhcpV4Packet.ClientHardwareAddress);

                Assert.AreEqual(IPAddress.Any, dhcpV4Packet.ClientAddress);
                Assert.AreEqual(IPAddress.Any, dhcpV4Packet.ServerAddress);
                Assert.AreEqual(IPAddress.Any, dhcpV4Packet.YourAddress);
                Assert.AreEqual(IPAddress.Any, dhcpV4Packet.GatewayAddress);

                var testIP = IPAddress.Parse("1.2.3.4");
                dhcpV4Packet.ClientAddress = testIP;
                dhcpV4Packet.ServerAddress = testIP;
                dhcpV4Packet.YourAddress = testIP;
                dhcpV4Packet.GatewayAddress = testIP;

                Assert.AreEqual(testIP, dhcpV4Packet.ClientAddress);
                Assert.AreEqual(testIP, dhcpV4Packet.ServerAddress);
                Assert.AreEqual(testIP, dhcpV4Packet.YourAddress);
                Assert.AreEqual(testIP, dhcpV4Packet.GatewayAddress);

                var options = dhcpV4Packet.GetOptions();

                var addressRequestOption = options.OfType<AddressRequestOption>().FirstOrDefault();
                Assert.NotNull(addressRequestOption);
                Assert.AreEqual(IPAddress.Any, addressRequestOption.RequestedIP);
                
                var clientIdOption = options.OfType<ClientIdOption>().FirstOrDefault();
                Assert.NotNull(clientIdOption);

                dhcpV4Packet.SetOptions(new List<DhcpV4Option> {new MessageTypeOption(DhcpV4MessageType.Discover), new AddressTimeOption(TimeSpan.FromDays(1)) });
                Assert.AreEqual(2, dhcpV4Packet.GetOptions().Count);
            }

            dev.Close();
        }
    }
}
