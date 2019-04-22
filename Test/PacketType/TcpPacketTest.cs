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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Tcp;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class TcpPacketTest
    {
        [Test]
        public void BinarySerialization()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            RawCapture rawCapture;
            var foundTcpPacket = false;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                var t = p.Extract<TcpPacket>();
                if (t == null)
                {
                    continue;
                }

                foundTcpPacket = true;

                var memoryStream = new MemoryStream();
                var serializer = new BinaryFormatter();
                serializer.Serialize(memoryStream, t);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var deserializer = new BinaryFormatter();
                var fromFile = (TcpPacket) deserializer.Deserialize(memoryStream);

                Assert.AreEqual(t.Bytes, fromFile.Bytes);
                Assert.AreEqual(t.BytesSegment.Bytes, fromFile.BytesSegment.Bytes);
                Assert.AreEqual(t.BytesSegment.BytesLength, fromFile.BytesSegment.BytesLength);
                Assert.AreEqual(t.BytesSegment.Length, fromFile.BytesSegment.Length);
                Assert.AreEqual(t.BytesSegment.NeedsCopyForActualBytes, fromFile.BytesSegment.NeedsCopyForActualBytes);
                Assert.AreEqual(t.BytesSegment.Offset, fromFile.BytesSegment.Offset);
                Assert.AreEqual(t.Color, fromFile.Color);
                Assert.AreEqual(t.HeaderData, fromFile.HeaderData);
                Assert.AreEqual(t.PayloadData, fromFile.PayloadData);
                Assert.AreEqual(t.Ack, fromFile.Ack);
                Assert.AreEqual(t.AcknowledgmentNumber, fromFile.AcknowledgmentNumber);
                Assert.AreEqual(t.Flags, fromFile.Flags);
                Assert.AreEqual(t.Checksum, fromFile.Checksum);
                Assert.AreEqual(t.NS, fromFile.NS);
                Assert.AreEqual(t.Cwr, fromFile.Cwr);
                Assert.AreEqual(t.DataOffset, fromFile.DataOffset);
                Assert.AreEqual(t.DestinationPort, fromFile.DestinationPort);
                Assert.AreEqual(t.Ecn, fromFile.Ecn);
                Assert.AreEqual(t.Fin, fromFile.Fin);
                Assert.AreEqual(t.Options, fromFile.Options);
                Assert.AreEqual(t.Psh, fromFile.Psh);
                Assert.AreEqual(t.Rst, fromFile.Rst);
                Assert.AreEqual(t.SequenceNumber, fromFile.SequenceNumber);
                Assert.AreEqual(t.SourcePort, fromFile.SourcePort);
                Assert.AreEqual(t.Syn, fromFile.Syn);
                Assert.AreEqual(t.Urg, fromFile.Urg);
                Assert.AreEqual(t.UrgentPointer, fromFile.UrgentPointer);
                Assert.AreEqual(t.ValidChecksum, fromFile.ValidChecksum);
                Assert.AreEqual(t.ValidTcpChecksum, fromFile.ValidTcpChecksum);
                Assert.AreEqual(t.WindowSize, fromFile.WindowSize);

                //Method Invocations to make sure that a deserialized packet does not cause 
                //additional errors.

                t.CalculateTcpChecksum();
                t.IsValidChecksum(TransportPacket.TransportChecksumOption.None);
                t.PrintHex();
                t.UpdateCalculatedValues();
                t.UpdateTcpChecksum();
            }

            dev.Close();
            Assert.IsTrue(foundTcpPacket, "Capture file contained no tcpPacket packets");
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
            var t = p.Extract<TcpPacket>();
            Assert.IsNotNull(t, "Expected t not to be null");

            // even though the packet has 6 bytes of extra data, the ip packet shows a size of
            // 40 and the ip header has a length of 20. The TCP header is also 20 bytes so
            // there should be zero bytes in the TCPData value
            var expectedTcpDataLength = 0;
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

            var t = p.Extract<TcpPacket>();
            Assert.IsNotNull(t, "Expected t to not be null");
            Console.WriteLine("Checksum: " + t.Checksum.ToString("X"));
            Assert.IsTrue(t.ValidChecksum, "ValidChecksum indicates invalid checksum");

            dev.Close();
        }

        [Test]
        public void CreationAndReparsing()
        {
            var tcpPacket = TcpPacket.RandomPacket();

            var tcpPacket2 = new TcpPacket(new ByteArraySegment(tcpPacket.Bytes));

            Console.WriteLine("tcpPacket {0}", tcpPacket);
            Console.WriteLine("tcpPacket2 {0}", tcpPacket2);
        }

        [Test]
        public void PayloadModification()
        {
            var s = "-++++=== HELLLLOOO ===++++-";
            var data = System.Text.Encoding.UTF8.GetBytes(s);

            //create random pkt
            var p = TcpPacket.RandomPacket();

            //replace pkt's data with our string
            p.PayloadData = data;

            //sanity check
            Assert.AreEqual(s, System.Text.Encoding.Default.GetString(p.PayloadData));
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
            var t = p.Extract<TcpPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(t.ToString());
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
            var t = p.Extract<TcpPacket>();

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(t.ToString(StringOutputType.Verbose));
        }

        [Test]
        public void RandomPacket()
        {
            TcpPacket.RandomPacket();
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
        public void TCPOptions()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/tcp.pcap");
            dev.Open();

            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            Assert.IsNotNull(p);

            var t = p.Extract<TcpPacket>();
            Assert.IsNotNull(t, "Expected t to not be null");

            // verify that the options byte match what we expect
            byte[] expectedOptions =
            {
                0x1, 0x1, 0x8, 0xa, 0x0, 0x14,
                0x3d, 0xe5, 0x1d, 0xf5, 0xf8, 0x84
            };

            Assert.AreEqual(expectedOptions, t.Options);

            var options = t.OptionsCollection;

            Assert.AreEqual(true, options.Any(x => x is TimeStamp));

            foreach (var option in options)
            {
                if (option is TimeStamp timeStamp)
                {
                    Assert.AreEqual(1326565, timeStamp.Value);

                    timeStamp.Value = 1234321;

                    Assert.AreEqual(1234321, timeStamp.Value);
                }
            }

            var optionsCollection = new List<Option>(options);
            t.OptionsCollection = optionsCollection;

            foreach (var option in t.OptionsCollection)
            {
                if (option is TimeStamp timeStamp)
                {
                    Assert.AreEqual(1234321, timeStamp.Value);
                }
            }

            dev.Close();
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

            var t = p.Extract<TcpPacket>();
            Assert.IsNotNull(t, "Expected t to not be null");

            dev.Close();
        }
    }
}