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
 *  Copyright 2017 Andrew <pandipd@outlook.com>
 */

using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Collections.Generic;
using PacketDotNet.Utils.Converters;

namespace Test.PacketType
{
    [TestFixture]
    public class DrdaPacketTest
    {
        DrdaDdmPacket excsatPacket;
        DrdaDdmPacket secchkPacket;
        DrdaDdmPacket accrdbPacket;
        DrdaDdmPacket accrdbrmPacket;
        List<DrdaDdmPacket> sqlsttPackets = new List<DrdaDdmPacket>();
        DrdaDdmPacket prpsqlsttPacket;
        DrdaDdmPacket sqlattrPacket;
        bool packetsLoaded = false;

        [SetUp]
        public void Init()
        {
            if (packetsLoaded)
                return;
            RawCapture raw;
            int packetIndex = 0;
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/db2_select.pcap");
            dev.Open();


            while ((raw = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(raw.LinkLayerType, raw.Data).Extract<DrdaPacket>();
                if (p != null)
                {
                    foreach (var ddm in p.DrdaDdmPackets)
                    {
                        switch (ddm.CodePoint)
                        {
                            case DrdaCodePointType.EXCSAT:
                                excsatPacket = ddm;
                                break;
                            case DrdaCodePointType.ACCRDB:
                                accrdbPacket = ddm;
                                break;
                            case DrdaCodePointType.SECCHK:
                                secchkPacket = ddm;
                                break;
                            case DrdaCodePointType.ACCRDBRM:
                                accrdbrmPacket = ddm;
                                break;
                            case DrdaCodePointType.SQLSTT:
                                sqlsttPackets.Add(ddm);
                                break;
                            case DrdaCodePointType.PRPSQLSTT:
                                prpsqlsttPacket = ddm;
                                break;
                            case DrdaCodePointType.SQLATTR:
                                sqlattrPacket = ddm;
                                break;
                            //Still have SQLCARD and QRYDTA decode work to do
                            default: /* do nothing */break;
                        }
                    }
                }
                packetIndex++;
            }
            dev.Close();
            packetsLoaded = true;
        }

        [Test]
        public void TestExcsatPacket()
        {
            Assert.IsNotNull(excsatPacket);
            Assert.IsNotNull(excsatPacket.Parameters);
            Assert.AreEqual(DrdaCodePointType.EXCSAT, excsatPacket.CodePoint);
            foreach (var parameter in excsatPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.EXTNAM:
                        Assert.IsTrue(parameter.Data.Contains("db2jcc_application"));
                        break;
                    case DrdaCodePointType.SRVNAM:
                        Assert.AreEqual("192.168.137.1", parameter.Data);
                        break;
                    case DrdaCodePointType.SRVRLSLV:
                        Assert.AreEqual("JCC03670", parameter.Data);
                        break;
                    case DrdaCodePointType.SRVCLSNM:
                        Assert.AreEqual("QDB2/JVM", parameter.Data);
                        break;
                    default: /* do nothing */break;
                }
            }
        }

        [Test]
        public void TestAccrdbPacket()
        {
            Assert.IsNotNull(accrdbPacket);
            Assert.IsNotNull(accrdbPacket.Parameters);
            Assert.AreEqual(DrdaCodePointType.ACCRDB, accrdbPacket.CodePoint);
            foreach (var parameter in accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.RDBNAM:
                        Assert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    case DrdaCodePointType.PRDID:
                        Assert.AreEqual("JCC03670", parameter.Data);
                        break;
                    case DrdaCodePointType.TYPDEFNAM:
                        Assert.AreEqual("QTDSQLASC", parameter.Data);
                        break;
                    default: /* do nothing */break;
                }
            }
        }

        [Test]
        public void TestSecchkPacket()
        {
            Assert.IsNotNull(secchkPacket);
            Assert.IsNotNull(secchkPacket.Parameters);
            Assert.AreEqual(DrdaCodePointType.SECCHK, secchkPacket.CodePoint);
            foreach (var parameter in accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.RDBNAM:
                        Assert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    case DrdaCodePointType.USRID:
                        Assert.AreEqual("db2inst1", parameter.Data);
                        break;
                    case DrdaCodePointType.PASSWORD:
                        Assert.AreEqual("db2inst1", parameter.Data);
                        break;
                    default: /* do nothing */break;
                }
            }
        }

        [Test]
        public void TestSqlsttPacket()
        {
            int packetIndex = 0;
            foreach (var packet in sqlsttPackets)
            {
                Assert.IsNotNull(packet);
                Assert.IsNotNull(packet.Parameters);
                Assert.AreEqual(DrdaCodePointType.SQLSTT, packet.CodePoint);
                if (packet.Parameters[0].DrdaCodepoint == DrdaCodePointType.DATA)
                {
                    if (packetIndex == 0)
                        Assert.AreEqual("SET CLIENT WRKSTNNAME '192.168.137.1'", packet.Parameters[0].Data);
                    else if (packetIndex == 1)
                        Assert.AreEqual("SELECT * FROM SYSCAT.TABLES", packet.Parameters[0].Data);
                }
                packetIndex++;
            }
        }

        [Test]
        public void TestSqlattrPacket()
        {
            Assert.IsNotNull(sqlattrPacket);
            Assert.IsNotNull(sqlattrPacket.Parameters);
            Assert.AreEqual(DrdaCodePointType.SQLATTR, sqlattrPacket.CodePoint);
            if (sqlattrPacket.Parameters[0].DrdaCodepoint == DrdaCodePointType.DATA)
            {
                Assert.AreEqual("FOR READ ONLY", sqlattrPacket.Parameters[0].Data);
            }
        }

        [Test]
        public void TestStringConverter()
        {
            var bytes = new byte[] { 0xd8, 0xc4, 0xc2, 0xf2, 0x61, 0xd1, 0xe5, 0xd4 };
            Assert.AreEqual("QDB2/JVM", StringConverter.EbcdicToAscii(bytes, 0, bytes.Length));
        }
    }
}
