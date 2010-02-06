using System;
using NUnit.Framework;
using SharpPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class LinuxCookedCaptureTest
    {
        private void VerifyPacket0(Packet p)
        {
            // expect an arp packet
            var arpPacket = ARPPacket.GetType(p);
            Assert.IsNotNull(arpPacket, "Expected arpPacket to not be null");

            // validate some of the LinuxSSLPacket fields
            var l = (LinuxSLLPacket)p;
            Assert.AreEqual(6, l.LinkLayerAddressLength, "Address length");
            Assert.AreEqual(1, l.LinkLayerAddressType);
            Assert.AreEqual(LinuxSLLType.PacketSentToUs, l.Type);

            // validate some of the arp fields
            Assert.AreEqual("192.168.1.1",
                            arpPacket.SenderProtocolAddress.ToString(),
                            "Arp SenderProtocolAddress");
            Assert.AreEqual("192.168.1.102",
                            arpPacket.TargetProtocolAddress.ToString(),
                            "Arp TargetProtocolAddress");
        }

        private void VerifyPacket1(Packet p)
        {
            // expect a udp packet
            Assert.IsNotNull(UdpPacket.GetType(p), "expected a udp packet");
        }

        private void VerifyPacket2(Packet p)
        {
            // expecting a tcp packet
            Assert.IsNotNull(TcpPacket.GetType(p), "expected a tcp packet");
        }

        [Test]
        public void CookedCaptureTest()
        {
            var dev = new OfflinePcapDevice("../../CaptureFiles/LinuxCookedCapture.pcap");
            dev.Open();

            SharpPcap.Packets.RawPacket rawPacket;
            int packetIndex = 0;
            while((rawPacket = dev.GetNextRawPacket()) != null)
            {
                Console.WriteLine("got packet");
                Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                              new PosixTimeval(rawPacket.Timeval.Seconds, rawPacket.Timeval.MicroSeconds),
                                              rawPacket.Data);
                switch(packetIndex)
                {
                case 0:
                    VerifyPacket0(p);
                    break;
                case 1:
                    VerifyPacket1(p);
                    break;
                case 2:
                    VerifyPacket2(p);
                    break;
                default:
                    Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                    break;
                }

                packetIndex++;
            }

            dev.Close();
        }
    }
}
