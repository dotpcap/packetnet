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
using System.Collections.Generic;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Drda;
using PacketDotNet.Utils.Conversion;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class DrdaPacketTest
    {
        private readonly List<DrdaDDMPacket> _sqlsttPackets = new List<DrdaDDMPacket>();
        private DrdaDDMPacket _accrdbPacket;
        private DrdaDDMPacket _accrdbrmPacket;
        private DrdaDDMPacket _excsatPacket;
        private Boolean _packetsLoaded;
        private DrdaDDMPacket _prpsqlsttPacket;
        private DrdaDDMPacket _secchkPacket;
        private DrdaDDMPacket _sqlattrPacket;

        [SetUp]
        public void Init()
        {
            if (this._packetsLoaded)
                return;
            RawCapture raw;
            Int32 packetIndex = 0;
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/db2_select.pcap");
            dev.Open();


            while ((raw = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(raw.LinkLayerType, raw.Data).Extract(typeof(DrdaPacket)) as DrdaPacket;
                if (p != null)
                {
                    foreach (var ddm in p.DrdaDDMPackets)
                    {
                        switch (ddm.CodePoint)
                        {
                            case DrdaCodepointType.EXCSAT:
                                this._excsatPacket = ddm;
                                break;
                            case DrdaCodepointType.ACCRDB:
                                this._accrdbPacket = ddm;
                                break;
                            case DrdaCodepointType.SECCHK:
                                this._secchkPacket = ddm;
                                break;
                            case DrdaCodepointType.ACCRDBRM:
                                this._accrdbrmPacket = ddm;
                                break;
                            case DrdaCodepointType.SQLSTT:
                                this._sqlsttPackets.Add(ddm);
                                break;
                            case DrdaCodepointType.PRPSQLSTT:
                                this._prpsqlsttPacket = ddm;
                                break;
                            case DrdaCodepointType.SQLATTR:
                                this._sqlattrPacket = ddm;
                                break;
                            //Still have SQLCARD and QRYDTA decode work to do
                            default: /* do nothing */ break;
                        }
                    }
                }

                packetIndex++;
            }

            dev.Close();
            this._packetsLoaded = true;
        }

        [Test]
        public void TestAccrdbPacket()
        {
            Assert.IsNotNull(this._accrdbPacket);
            Assert.IsNotNull(this._accrdbPacket.Parameters);
            Assert.AreEqual(DrdaCodepointType.ACCRDB, this._accrdbPacket.CodePoint);
            foreach (var parameter in this._accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodepointType.RDBNAM:
                        Assert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    case DrdaCodepointType.PRDID:
                        Assert.AreEqual("JCC03670", parameter.Data);
                        break;
                    case DrdaCodepointType.TYPDEFNAM:
                        Assert.AreEqual("QTDSQLASC", parameter.Data);
                        break;
                    default: /* do nothing */ break;
                }
            }
        }

        [Test]
        public void TestExcsatPacket()
        {
            Assert.IsNotNull(this._excsatPacket);
            Assert.IsNotNull(this._excsatPacket.Parameters);
            Assert.AreEqual(DrdaCodepointType.EXCSAT, this._excsatPacket.CodePoint);
            foreach (var parameter in this._excsatPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodepointType.EXTNAM:
                        Assert.IsTrue(parameter.Data.Contains("db2jcc_application"));
                        break;
                    case DrdaCodepointType.SRVNAM:
                        Assert.AreEqual("192.168.137.1", parameter.Data);
                        break;
                    case DrdaCodepointType.SRVRLSLV:
                        Assert.AreEqual("JCC03670", parameter.Data);
                        break;
                    case DrdaCodepointType.SRVCLSNM:
                        Assert.AreEqual("QDB2/JVM", parameter.Data);
                        break;
                    default: /* do nothing */ break;
                }
            }
        }

        [Test]
        public void TestSecchkPacket()
        {
            Assert.IsNotNull(this._secchkPacket);
            Assert.IsNotNull(this._secchkPacket.Parameters);
            Assert.AreEqual(DrdaCodepointType.SECCHK, this._secchkPacket.CodePoint);
            foreach (var parameter in this._accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodepointType.RDBNAM:
                        Assert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    case DrdaCodepointType.USRID:
                        Assert.AreEqual("db2inst1", parameter.Data);
                        break;
                    case DrdaCodepointType.PASSWORD:
                        Assert.AreEqual("db2inst1", parameter.Data);
                        break;
                    default: /* do nothing */ break;
                }
            }
        }

        [Test]
        public void TestSqlattrPacket()
        {
            Assert.IsNotNull(this._sqlattrPacket);
            Assert.IsNotNull(this._sqlattrPacket.Parameters);
            Assert.AreEqual(DrdaCodepointType.SQLATTR, this._sqlattrPacket.CodePoint);
            if (this._sqlattrPacket.Parameters[0].DrdaCodepoint == DrdaCodepointType.DATA)
            {
                Assert.AreEqual("FOR READ ONLY", this._sqlattrPacket.Parameters[0].Data);
            }
        }

        [Test]
        public void TestSqlsttPacket()
        {
            Int32 packetIndex = 0;
            foreach (var packet in this._sqlsttPackets)
            {
                Assert.IsNotNull(packet);
                Assert.IsNotNull(packet.Parameters);
                Assert.AreEqual(DrdaCodepointType.SQLSTT, packet.CodePoint);
                if (packet.Parameters[0].DrdaCodepoint == DrdaCodepointType.DATA)
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
        public void TestStringConverter()
        {
            var bytes = new Byte[] {0xd8, 0xc4, 0xc2, 0xf2, 0x61, 0xd1, 0xe5, 0xd4};
            Assert.AreEqual("QDB2/JVM", StringConverter.EbcdicToAscii(bytes, 0, bytes.Length));
        }
    }
}