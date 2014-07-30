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
using NUnit.Framework;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SharpPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class TcpPacketTest
    {
        [Test]
        public void CreationAndReparsing()
        {
            var tcpPacket = TcpPacket.RandomPacket();

            var tcpPacket2 = new TcpPacket(new ByteArraySegment(tcpPacket.Bytes));

            Console.WriteLine("tcpPacket {0}", tcpPacket);
            Console.WriteLine("tcpPacket2 {0}", tcpPacket2);
        }

        [Test]
        public void ByteLengthInternalLengthMismatch()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp_with_extra_bytes.pcap");
            dev.Open();

            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());
            var t = (TcpPacket)p.Extract(typeof(TcpPacket));
            Assert.IsNotNull(t, "Expected t not to be null");

            // even though the packet has 6 bytes of extra data, the ip packet shows a size of
            // 40 and the ip header has a length of 20. The TCP header is also 20 bytes so
            // there should be zero bytes in the TCPData value
            int expectedTcpDataLength = 0;
            Assert.AreEqual(expectedTcpDataLength, t.PayloadData.Length);

            dev.Close();
        }

        [Test]
        public virtual void Checksum()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            Console.WriteLine(p.GetType());

            var t = (TcpPacket)p.Extract (typeof(TcpPacket));
            Assert.IsNotNull(t, "Expected t to not be null");
            Console.WriteLine("Checksum: " + t.Checksum.ToString("X"));
            Assert.IsTrue(t.ValidChecksum, "ValidChecksum indicates invalid checksum");

            dev.Close();
        }

        [Test]
        public void PayloadModification()
        {
            String s = "-++++=== HELLLLOOO ===++++-";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random pkt
            var p = TcpPacket.RandomPacket();

            //replace pkt's data with our string
            p.PayloadData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.PayloadData));
        }

        /// <summary>
        /// Test that TSO works, see http://en.wikipedia.org/wiki/TCP_offload_engine
        /// </summary>
        [Test]
        public void TcpSegmentOffload()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/ipv4_tso_frame.pcap");
            dev.Open();

            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var t = (TcpPacket)p.Extract (typeof(TcpPacket));
            Assert.IsNotNull(t, "Expected t to not be null");

            dev.Close();            
        }

        [Test]
        public void TCPOptions()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var t = (TcpPacket)p.Extract (typeof(TcpPacket));
            Assert.IsNotNull(t, "Expected t to not be null");

            // verify that the options byte match what we expect
            byte[] expectedOptions = new byte[] { 0x1, 0x1, 0x8, 0xa, 0x0, 0x14,
                                                  0x3d, 0xe5, 0x1d, 0xf5, 0xf8, 0x84 };
            Assert.AreEqual(expectedOptions, t.Options);

            dev.Close();
        }

        [Test]
        public void TCPConstructorFromValues()
        {
            ushort sourcePort = 100;
            ushort destinationPort = 101;
            var tcpPacket = new TcpPacket(sourcePort, destinationPort);

            Assert.AreEqual(sourcePort, tcpPacket.SourcePort);
            Assert.AreEqual(destinationPort, tcpPacket.DestinationPort);
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var tcp = (TcpPacket)p.Extract (typeof(TcpPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(tcp.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Console.WriteLine("Parsing");
            var tcp = (TcpPacket)p.Extract (typeof(TcpPacket));

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(tcp.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            TcpPacket.RandomPacket();
        }
        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            RawCapture rawCapture;
            bool foundtcpPacket = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = PacketDotNet.Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                var tcpPacket = (TcpPacket)p.Extract(typeof(TcpPacket));
                if (tcpPacket == null)
                {
                    continue;
                }
                foundtcpPacket = true;

                var memoryStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, tcpPacket);

                memoryStream.Seek (0, SeekOrigin.Begin);
                BinaryFormatter deserializer = new BinaryFormatter();
                TcpPacket fromFile = (TcpPacket)deserializer.Deserialize(memoryStream);

                Assert.AreEqual(tcpPacket.Bytes, fromFile.Bytes);
                Assert.AreEqual(tcpPacket.BytesHighPerformance.Bytes, fromFile.BytesHighPerformance.Bytes);
                Assert.AreEqual(tcpPacket.BytesHighPerformance.BytesLength, fromFile.BytesHighPerformance.BytesLength);
                Assert.AreEqual(tcpPacket.BytesHighPerformance.Length, fromFile.BytesHighPerformance.Length);
                Assert.AreEqual(tcpPacket.BytesHighPerformance.NeedsCopyForActualBytes, fromFile.BytesHighPerformance.NeedsCopyForActualBytes);
                Assert.AreEqual(tcpPacket.BytesHighPerformance.Offset, fromFile.BytesHighPerformance.Offset);
                Assert.AreEqual(tcpPacket.Color, fromFile.Color);
                Assert.AreEqual(tcpPacket.Header, fromFile.Header);
                Assert.AreEqual(tcpPacket.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(tcpPacket.Ack, fromFile.Ack);
                Assert.AreEqual(tcpPacket.AcknowledgmentNumber, fromFile.AcknowledgmentNumber);
                Assert.AreEqual(tcpPacket.AllFlags, fromFile.AllFlags);
                Assert.AreEqual(tcpPacket.Checksum, fromFile.Checksum);
                Assert.AreEqual(tcpPacket.CWR, fromFile.CWR);
                Assert.AreEqual(tcpPacket.DataOffset, fromFile.DataOffset);
                Assert.AreEqual(tcpPacket.DestinationPort, fromFile.DestinationPort);
                Assert.AreEqual(tcpPacket.ECN, fromFile.ECN);
                Assert.AreEqual(tcpPacket.Fin, fromFile.Fin);
                Assert.AreEqual(tcpPacket.Options, fromFile.Options);
                Assert.AreEqual(tcpPacket.Psh, fromFile.Psh);
                Assert.AreEqual(tcpPacket.Rst, fromFile.Rst);
                Assert.AreEqual(tcpPacket.SequenceNumber, fromFile.SequenceNumber);
                Assert.AreEqual(tcpPacket.SourcePort, fromFile.SourcePort);
                Assert.AreEqual(tcpPacket.Syn, fromFile.Syn);
                Assert.AreEqual(tcpPacket.Urg, fromFile.Urg);
                Assert.AreEqual(tcpPacket.UrgentPointer, fromFile.UrgentPointer);
                Assert.AreEqual(tcpPacket.ValidChecksum, fromFile.ValidChecksum);
                Assert.AreEqual(tcpPacket.ValidTCPChecksum, fromFile.ValidTCPChecksum);
                Assert.AreEqual(tcpPacket.WindowSize, fromFile.WindowSize);

                //Method Invocations to make sure that a deserialized packet does not cause 
                //additional errors.

                tcpPacket.CalculateTCPChecksum();
                tcpPacket.IsValidChecksum(TransportPacket.TransportChecksumOption.None);
                tcpPacket.PrintHex();
                tcpPacket.UpdateCalculatedValues();
                tcpPacket.UpdateTCPChecksum();
            }

            dev.Close();
            Assert.IsTrue(foundtcpPacket, "Capture file contained no tcpPacket packets");
        }
    }
}