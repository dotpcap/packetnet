using System;
using System.Net;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class ArpPacketTest
    {
        // arp request
        private void VerifyPacket0(Packet p)
        {
            Assert.IsTrue(p is ARPPacket, "p isn't an ARPPacket");

            ARPPacket arpPacket = (ARPPacket)p;

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
            Assert.IsTrue(p is ARPPacket, "p isn't an ARPPacket");
            ARPPacket arpPacket = (ARPPacket)p;

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
            PcapOfflineDevice dev = Pcap.GetPcapOfflineDevice("../../capture_files/arp_request_response.pcap");
            dev.Open();                                                                           

            throw new System.NotImplementedException();
#if false
            SharpPcap.Packets.RawPacket rawPacket;
            int packetIndex = 0;
            while((rawPacket = dev.GetNextRawPacket()) != null)
            {
//                Packet p = new Packet(rawPacket.
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
#endif

            dev.Close();
        }
    }
}
