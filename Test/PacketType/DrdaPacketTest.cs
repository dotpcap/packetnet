/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Utils.Converters;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class DrdaPacketTest
    {
        [SetUp]
        public void Init()
        {
            if (_packetsLoaded)
                return;


            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "db2_select.pcap");
            dev.Open();

            PacketCapture c;
            GetPacketStatus status;
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var raw = c.GetPacket();
                var p = Packet.ParsePacket(raw.GetLinkLayers(), raw.Data).Extract<DrdaPacket>();
                if (p != null)
                {
                    foreach (var ddm in p.DrdaDdmPackets)
                    {
                        switch (ddm.CodePoint)
                        {
                            case DrdaCodePointType.ExchangeServerAttributes:
                            {
                                _excsatPacket = ddm;
                                break;
                            }
                            case DrdaCodePointType.AccessRdb:
                            {
                                _accrdbPacket = ddm;
                                break;
                            }
                            case DrdaCodePointType.SecurityCheck:
                            {
                                _secchkPacket = ddm;
                                break;
                            }
                            case DrdaCodePointType.AccessToRdbCompleted:
                            {
                                _accrdbrmPacket = ddm;
                                break;
                            }
                            case DrdaCodePointType.SqlStatement:
                            {
                                _sqlsttPackets.Add(ddm);
                                break;
                            }
                            case DrdaCodePointType.PrepareSqlStatement:
                            {
                                _prpsqlsttPacket = ddm;
                                break;
                            }
                            case DrdaCodePointType.SqlStatementAttributes:
                            {
                                _sqlattrPacket = ddm;
                                break;
                            }
                            //Still have SQLCARD and QRYDTA decode work to do
                        }
                    }
                }
            }

            dev.Close();
            _packetsLoaded = true;
        }

        private DrdaDdmPacket _excsatPacket;
        private DrdaDdmPacket _secchkPacket;
        private DrdaDdmPacket _accrdbPacket;
        private DrdaDdmPacket _accrdbrmPacket;
        private readonly List<DrdaDdmPacket> _sqlsttPackets = new List<DrdaDdmPacket>();
        private DrdaDdmPacket _prpsqlsttPacket;
        private DrdaDdmPacket _sqlattrPacket;
        private bool _packetsLoaded;

        [Test]
        public void TestAccrdbPacket()
        {
            ClassicAssert.IsNotNull(_accrdbPacket);
            ClassicAssert.IsNotNull(_accrdbPacket.Parameters);
            ClassicAssert.AreEqual(DrdaCodePointType.AccessRdb, _accrdbPacket.CodePoint);
            foreach (var parameter in _accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.RelationalDatabaseName:
                    {
                        ClassicAssert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.ProductSpecificIdentifier:
                    {
                        ClassicAssert.AreEqual("JCC03670", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.DataTypeDefinitionName:
                    {
                        ClassicAssert.AreEqual("QTDSQLASC", parameter.Data);
                        break;
                    }
                }
            }
        }

        [Test]
        public void TestExcsatPacket()
        {
            ClassicAssert.IsNotNull(_excsatPacket);
            ClassicAssert.IsNotNull(_excsatPacket.Parameters);
            ClassicAssert.AreEqual(DrdaCodePointType.ExchangeServerAttributes, _excsatPacket.CodePoint);
            foreach (var parameter in _excsatPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.ExternalName:
                    {
                        ClassicAssert.IsTrue(parameter.Data.Contains("db2jcc_application"));
                        break;
                    }
                    case DrdaCodePointType.ServerName:
                    {
                        ClassicAssert.AreEqual("192.168.137.1", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.ServerProductReleaseLevel:
                    {
                        ClassicAssert.AreEqual("JCC03670", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.ServerClassName:
                    {
                        ClassicAssert.AreEqual("QDB2/JVM", parameter.Data);
                        break;
                    }
                }
            }
        }

        [Test]
        public void TestSecchkPacket()
        {
            ClassicAssert.IsNotNull(_secchkPacket);
            ClassicAssert.IsNotNull(_secchkPacket.Parameters);
            ClassicAssert.AreEqual(DrdaCodePointType.SecurityCheck, _secchkPacket.CodePoint);
            foreach (var parameter in _accrdbPacket.Parameters)
            {
                switch (parameter.DrdaCodepoint)
                {
                    case DrdaCodePointType.RelationalDatabaseName:
                    {
                        ClassicAssert.AreEqual("SAMPLE", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.UserIdAtTargetSystem:
                    {
                        ClassicAssert.AreEqual("db2inst1", parameter.Data);
                        break;
                    }
                    case DrdaCodePointType.Password:
                    {
                        ClassicAssert.AreEqual("db2inst1", parameter.Data);
                        break;
                    }
                }
            }
        }

        [Test]
        public void TestSqlattrPacket()
        {
            ClassicAssert.IsNotNull(_sqlattrPacket);
            ClassicAssert.IsNotNull(_sqlattrPacket.Parameters);
            ClassicAssert.AreEqual(DrdaCodePointType.SqlStatementAttributes, _sqlattrPacket.CodePoint);
            if (_sqlattrPacket.Parameters[0].DrdaCodepoint == DrdaCodePointType.Data)
            {
                ClassicAssert.AreEqual("FOR READ ONLY", _sqlattrPacket.Parameters[0].Data);
            }
        }

        [Test]
        public void TestSqlsttPacket()
        {
            var packetIndex = 0;
            foreach (var packet in _sqlsttPackets)
            {
                ClassicAssert.IsNotNull(packet);
                ClassicAssert.IsNotNull(packet.Parameters);
                ClassicAssert.AreEqual(DrdaCodePointType.SqlStatement, packet.CodePoint);
                if (packet.Parameters[0].DrdaCodepoint == DrdaCodePointType.Data)
                {
                    if (packetIndex == 0)
                        ClassicAssert.AreEqual("SET CLIENT WRKSTNNAME '192.168.137.1'", packet.Parameters[0].Data);
                    else if (packetIndex == 1)
                        ClassicAssert.AreEqual("SELECT * FROM SYSCAT.TABLES", packet.Parameters[0].Data);
                }

                packetIndex++;
            }
        }

        [Test]
        public void TestStringConverter()
        {
            var bytes = new byte[] { 0xd8, 0xc4, 0xc2, 0xf2, 0x61, 0xd1, 0xe5, 0xd4 };
            ClassicAssert.AreEqual("QDB2/JVM", StringConverter.EbcdicToAscii(bytes, 0, bytes.Length));
        }
    }