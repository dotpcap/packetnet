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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace Test.PacketType
{
    [TestFixture]
    public class ArpPacketTest
    {
        // arp request
        private void VerifyPacket0(Packet p)
        {
            var arpPacket = ARPPacket.GetEncapsulated(p);
            Assert.IsNotNull(arpPacket, "Expected arpPacket to not be null");

            IPAddress senderIp = IPAddress.Parse("192.168.1.202");
            IPAddress targetIp = IPAddress.Parse("192.168.1.214");

            Assert.AreEqual(senderIp, arpPacket.SenderProtocolAddress);
            Assert.AreEqual(targetIp, arpPacket.TargetProtocolAddress);

            string senderMacAddress = "000461990154";
            string targetMacAddress = "000000000000";
            Assert.AreEqual(senderMacAddress, arpPacket.SenderHardwareAddress.ToString());
            Assert.AreEqual(targetMacAddress, arpPacket.TargetHardwareAddress.ToString());
        }

        // arp response
        private void VerifyPacket1(Packet p)
        {
            var arpPacket = ARPPacket.GetEncapsulated(p);
            Assert.IsNotNull(arpPacket, "Expected arpPacket to not be null");

            IPAddress senderIp = IPAddress.Parse("192.168.1.214");
            IPAddress targetIp = IPAddress.Parse("192.168.1.202");

            Assert.AreEqual(senderIp, arpPacket.SenderProtocolAddress);
            Assert.AreEqual(targetIp, arpPacket.TargetProtocolAddress);

            string senderMacAddress = "00216A020854";
            string targetMacAddress = "000461990154";
            Assert.AreEqual(senderMacAddress, arpPacket.SenderHardwareAddress.ToString());
            Assert.AreEqual(targetMacAddress, arpPacket.TargetHardwareAddress.ToString());
        }

        [Test]
        public void ParsingArpPacketRequestResponse()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/arp_request_response.pcap");
            dev.Open();                                                                           

            SharpPcap.Packets.RawPacket rawPacket;
            int packetIndex = 0;
            while((rawPacket = dev.GetNextRawPacket()) != null)
            {
                var p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                           new PosixTimeval(rawPacket.Timeval.Seconds,
                                                            rawPacket.Timeval.MicroSeconds),
                                           rawPacket.Data);
                
                Console.WriteLine("got packet");
                Console.WriteLine("{0}", p.ToString());
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                case 1:
                    VerifyPacket1(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }

        /// <summary>
        /// Test that we can build an ARPPacket from values
        /// </summary>
        [Test]
        public void ConstructingFromValues()
        {
            var localIPBytes = new byte[4] {124, 10, 10, 20};
            var localIP = new System.Net.IPAddress(localIPBytes);

            var destinationIPBytes = new byte[4] {192, 168, 1, 10};
            var destinationIP = new System.Net.IPAddress(destinationIPBytes);

            var localMac = System.Net.NetworkInformation.PhysicalAddress.Parse("AA-BB-CC-DD-EE-FF");

            new PacketDotNet.ARPPacket(PacketDotNet.ARPOperation.Request,
                                       System.Net.NetworkInformation.PhysicalAddress.Parse("00-00-00-00-00-00"),
                                       destinationIP,
                                       localMac,
                                       localIP);
        }
    }
}
