/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class PerPacketInformationTest
    {
        [Test]
        public void AddUnknownField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_multiplefields.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;

            var unknownField = new PpiUnknown(99, new byte[] { 0xAA, 0xBB, 0xCC, 0xDD });
            p.Add(unknownField);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;

            ClassicAssert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            ClassicAssert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));
            ClassicAssert.IsTrue(recreatedPacket.Contains((PpiFieldType) 99));
            var recreatedUnknownField = recreatedPacket.FindFirstByType((PpiFieldType) 99) as PpiUnknown;
            ClassicAssert.AreEqual(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }, recreatedUnknownField.UnknownBytes);

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(macFrame);
            ClassicAssert.IsTrue(macFrame.FcsValid);
        }

        [Test]
        public void ConstructPacketWithMultipleAlignedFields()
        {
            var packet = new PpiPacket();
            packet.Flags |= PpiPacket.HeaderFlags.Alignment32Bit;

            var commonField = new PpiCommon { ChannelFrequency = 2142, AntennaSignalPower = 50, AntennaSignalNoise = 25 };
            packet.Add(commonField);

            ClassicAssert.AreEqual(32, packet.Length);

            var processInfoField = new PpiProcessInfo { UserId = 0x1111, UserName = "Hester the tester", GroupId = 0x2222, GroupName = "Test Group" };
            packet.Add(processInfoField);

            ClassicAssert.AreEqual(84, packet.Length);

            var aggregationField = new PpiAggregation(0x3333);
            packet.Add(aggregationField);

            ClassicAssert.AreEqual(92, packet.Length);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            var recreatedCommonField = recreatedPacket[0] as PpiCommon;
            ClassicAssert.IsNotNull(recreatedCommonField);
            ClassicAssert.AreEqual(2142, recreatedCommonField.ChannelFrequency);
            ClassicAssert.AreEqual(50, recreatedCommonField.AntennaSignalPower);
            ClassicAssert.AreEqual(25, recreatedCommonField.AntennaSignalNoise);

            var recreatedProcessField = recreatedPacket[1] as PpiProcessInfo;
            ClassicAssert.IsNotNull(recreatedProcessField);
            ClassicAssert.AreEqual(0x1111, recreatedProcessField.UserId);
            ClassicAssert.AreEqual("Hester the tester", recreatedProcessField.UserName);
            ClassicAssert.AreEqual(0x2222, recreatedProcessField.GroupId);
            ClassicAssert.AreEqual("Test Group", recreatedProcessField.GroupName);

            var recreatedAggregationField = recreatedPacket[2] as PpiAggregation;

            ClassicAssert.IsNotNull(recreatedAggregationField);
            ClassicAssert.AreEqual(0x3333, recreatedAggregationField.InterfaceId);
        }

        [Test]
        public void ConstructPacketWithMultipleFields()
        {
            var packet = new PpiPacket();

            var commonField = new PpiCommon { ChannelFrequency = 2142, AntennaSignalPower = 50, AntennaSignalNoise = 25 };
            packet.Add(commonField);

            ClassicAssert.AreEqual(32, packet.Length);

            var processInfoField = new PpiProcessInfo { UserId = 0x1111, UserName = "Hester the tester", GroupId = 0x2222, GroupName = "Test Group" };
            packet.Add(processInfoField);

            ClassicAssert.AreEqual(82, packet.Length);

            var aggregationField = new PpiAggregation(0x3333);
            packet.Add(aggregationField);

            ClassicAssert.AreEqual(90, packet.Length);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            var recreatedCommonField = recreatedPacket[0] as PpiCommon;
            ClassicAssert.IsNotNull(recreatedCommonField);
            ClassicAssert.AreEqual(2142, recreatedCommonField.ChannelFrequency);
            ClassicAssert.AreEqual(50, recreatedCommonField.AntennaSignalPower);
            ClassicAssert.AreEqual(25, recreatedCommonField.AntennaSignalNoise);

            var recreatedProcessField = recreatedPacket[1] as PpiProcessInfo;
            ClassicAssert.IsNotNull(recreatedProcessField);
            ClassicAssert.AreEqual(0x1111, recreatedProcessField.UserId);
            ClassicAssert.AreEqual("Hester the tester", recreatedProcessField.UserName);
            ClassicAssert.AreEqual(0x2222, recreatedProcessField.GroupId);
            ClassicAssert.AreEqual("Test Group", recreatedProcessField.GroupName);

            var recreatedAggregationField = recreatedPacket[2] as PpiAggregation;

            ClassicAssert.IsNotNull(recreatedAggregationField);
            ClassicAssert.AreEqual(0x3333, recreatedAggregationField.InterfaceId);
        }

        [Test]
        public void ConstructPacketWithNoFields()
        {
            var packet = new PpiPacket();

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            ClassicAssert.AreEqual(0, recreatedPacket.Version);
            ClassicAssert.IsFalse((recreatedPacket.Flags & PpiPacket.HeaderFlags.Alignment32Bit) == PpiPacket.HeaderFlags.Alignment32Bit);
        }

        [Test]
        public void ContainsField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_multiplefields.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;
            ClassicAssert.IsTrue(p.Contains(PpiFieldType.PpiCommon));
            ClassicAssert.IsTrue(p.Contains(PpiFieldType.PpiMacPhy));
            ClassicAssert.IsFalse(p.Contains(PpiFieldType.PpiProcessInfo));
        }

        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_per_packet_information.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;

            ClassicAssert.IsNotNull(p);
            ClassicAssert.AreEqual(0, p.Version);
            ClassicAssert.AreEqual(32, p.Length);
            ClassicAssert.AreEqual(1, p.Count);

            var commonField = p.FindFirstByType(PpiFieldType.PpiCommon) as PpiCommon;

            ClassicAssert.AreEqual(PpiFieldType.PpiCommon, commonField.FieldType);
            ClassicAssert.AreEqual(0, commonField.TSFTimer);
            ClassicAssert.IsTrue((commonField.Flags & PpiCommon.CommonFlags.FcsIncludedInFrame) == PpiCommon.CommonFlags.FcsIncludedInFrame);
            ClassicAssert.AreEqual(2, commonField.Rate);
            ClassicAssert.AreEqual(2437, commonField.ChannelFrequency);
            ClassicAssert.AreEqual(0x00A0, (int) commonField.ChannelFlags);
            ClassicAssert.AreEqual(0, commonField.FhssHopset);
            ClassicAssert.AreEqual(0, commonField.FhssPattern);
            ClassicAssert.AreEqual(-84, commonField.AntennaSignalPower);
            ClassicAssert.AreEqual(-100, commonField.AntennaSignalNoise);

            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, macFrame.FrameControl.SubType);
            ClassicAssert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithInvalidFcs()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_fcs_present_and_invalid.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;

            //The packet is corrupted in such a way that the type field has been changed
            //to a reserved/unused type. Therefore we don't expect there to be a packet
            ClassicAssert.IsNull(p.PayloadPacket);
            ClassicAssert.IsNotNull(p.PayloadData);
        }

        [Test]
        public void ReadPacketWithNoFcs()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_without_fcs.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;
            ClassicAssert.IsNotNull(p.PayloadPacket);
            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsFalse(macFrame.FcsValid);
            ClassicAssert.IsFalse(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithValidFcs()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_fcs_present_and_valid.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;
            ClassicAssert.IsNotNull(p.PayloadPacket);
            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsTrue(macFrame.FcsValid);
            ClassicAssert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void RemoveField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_multiplefields.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;
            var expectedLength = p.Length - p[1].Length - PpiHeaderFields.FieldHeaderLength;
            p.Remove(p[1]);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;
            ClassicAssert.AreEqual(expectedLength, recreatedPacket.Length);
            ClassicAssert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            ClassicAssert.IsFalse(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(macFrame);
            ClassicAssert.IsTrue(macFrame.FcsValid);
        }

        [Test]
        public void RemoveFieldByType()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_ppi_multiplefields.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as PpiPacket;
            p.RemoveAll(PpiFieldType.PpiMacPhy);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;

            ClassicAssert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            ClassicAssert.IsFalse(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(macFrame);
            ClassicAssert.IsTrue(macFrame.FcsValid);
        }
    }