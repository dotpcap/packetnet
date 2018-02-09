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
 *  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.IP;
using PacketDotNet.Tcp;

namespace Test.PacketType
{
    [TestFixture]
    public class LinktypeNullCaptureTest
    {
        private void VerifyPacket0(Packet p)
        {
            Assert.AreEqual (p.PayloadPacket.GetType(), typeof(IPv4Packet));
            Assert.AreEqual (p.PayloadPacket.PayloadPacket.GetType(), typeof(TcpPacket));
        }

        private void VerifyPacket1(Packet p)
        {
            Assert.AreEqual (p.PayloadPacket.GetType(), typeof(IPv6Packet));
            Assert.AreEqual (p.PayloadPacket.PayloadPacket.GetType(), typeof(TcpPacket));
        }

        [Test]
        public void LinktypeOfNullCaptureTest()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/linktype_null_capture.pcap");
            dev.Open();

            RawCapture rawCapture;
            int packetIndex = 0;
            while((rawCapture = dev.GetNextPacket()) != null)
            {
                Console.WriteLine ("LinkLayerType: {0}", rawCapture.LinkLayerType);
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
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

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/linktype_null_capture.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var np = (NullPacket)p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(np.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/linktype_null_capture.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var np = (NullPacket)p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(np.ToString(StringOutputType.Verbose));
        }
    }
}
