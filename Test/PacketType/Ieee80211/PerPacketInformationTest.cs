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

using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class PerPacketInformationTest
    {
        [Test]
        public void AddUnknownField()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_multiplefields.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;

            var unknownField = new PpiUnknown(99, new byte[] { 0xAA, 0xBB, 0xCC, 0xDD });
            p.Add(unknownField);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;

            Assert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            Assert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));
            Assert.IsTrue(recreatedPacket.Contains((PpiFieldType) 99));
            var recreatedUnknownField = recreatedPacket.FindFirstByType((PpiFieldType) 99) as PpiUnknown;
            Assert.AreEqual(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }, recreatedUnknownField.UnknownBytes);

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            Assert.IsNotNull(macFrame);
            Assert.IsTrue(macFrame.FcsValid);
        }

        [Test]
        public void ConstructPacketWithMultipleAlignedFields()
        {
            var packet = new PpiPacket();
            packet.Flags |= PpiPacket.HeaderFlags.Alignment32Bit;

            var commonField = new PpiCommon { ChannelFrequency = 2142, AntennaSignalPower = 50, AntennaSignalNoise = 25 };
            packet.Add(commonField);

            Assert.AreEqual(32, packet.Length);

            var processInfoField = new PpiProcessInfo { UserId = 0x1111, UserName = "Hester the tester", GroupId = 0x2222, GroupName = "Test Group" };
            packet.Add(processInfoField);

            Assert.AreEqual(84, packet.Length);

            var aggregationField = new PpiAggregation(0x3333);
            packet.Add(aggregationField);

            Assert.AreEqual(92, packet.Length);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            var recreatedCommonField = recreatedPacket[0] as PpiCommon;
            Assert.IsNotNull(recreatedCommonField);
            Assert.AreEqual(2142, recreatedCommonField.ChannelFrequency);
            Assert.AreEqual(50, recreatedCommonField.AntennaSignalPower);
            Assert.AreEqual(25, recreatedCommonField.AntennaSignalNoise);

            var recreatedProcessField = recreatedPacket[1] as PpiProcessInfo;
            Assert.IsNotNull(recreatedProcessField);
            Assert.AreEqual(0x1111, recreatedProcessField.UserId);
            Assert.AreEqual("Hester the tester", recreatedProcessField.UserName);
            Assert.AreEqual(0x2222, recreatedProcessField.GroupId);
            Assert.AreEqual("Test Group", recreatedProcessField.GroupName);

            var recreatedAggregationField = recreatedPacket[2] as PpiAggregation;

            Assert.IsNotNull(recreatedAggregationField);
            Assert.AreEqual(0x3333, recreatedAggregationField.InterfaceId);
        }

        [Test]
        public void ConstructPacketWithMultipleFields()
        {
            var packet = new PpiPacket();

            var commonField = new PpiCommon { ChannelFrequency = 2142, AntennaSignalPower = 50, AntennaSignalNoise = 25 };
            packet.Add(commonField);

            Assert.AreEqual(32, packet.Length);

            var processInfoField = new PpiProcessInfo { UserId = 0x1111, UserName = "Hester the tester", GroupId = 0x2222, GroupName = "Test Group" };
            packet.Add(processInfoField);

            Assert.AreEqual(82, packet.Length);

            var aggregationField = new PpiAggregation(0x3333);
            packet.Add(aggregationField);

            Assert.AreEqual(90, packet.Length);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            var recreatedCommonField = recreatedPacket[0] as PpiCommon;
            Assert.IsNotNull(recreatedCommonField);
            Assert.AreEqual(2142, recreatedCommonField.ChannelFrequency);
            Assert.AreEqual(50, recreatedCommonField.AntennaSignalPower);
            Assert.AreEqual(25, recreatedCommonField.AntennaSignalNoise);

            var recreatedProcessField = recreatedPacket[1] as PpiProcessInfo;
            Assert.IsNotNull(recreatedProcessField);
            Assert.AreEqual(0x1111, recreatedProcessField.UserId);
            Assert.AreEqual("Hester the tester", recreatedProcessField.UserName);
            Assert.AreEqual(0x2222, recreatedProcessField.GroupId);
            Assert.AreEqual("Test Group", recreatedProcessField.GroupName);

            var recreatedAggregationField = recreatedPacket[2] as PpiAggregation;

            Assert.IsNotNull(recreatedAggregationField);
            Assert.AreEqual(0x3333, recreatedAggregationField.InterfaceId);
        }

        [Test]
        public void ConstructPacketWithNoFields()
        {
            var packet = new PpiPacket();

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, packet.Bytes) as PpiPacket;

            Assert.AreEqual(0, recreatedPacket.Version);
            Assert.IsFalse((recreatedPacket.Flags & PpiPacket.HeaderFlags.Alignment32Bit) == PpiPacket.HeaderFlags.Alignment32Bit);
        }

        [Test]
        public void ContainsField()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_multiplefields.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
            Assert.IsTrue(p.Contains(PpiFieldType.PpiCommon));
            Assert.IsTrue(p.Contains(PpiFieldType.PpiMacPhy));
            Assert.IsFalse(p.Contains(PpiFieldType.PpiProcessInfo));
        }

        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_per_packet_information.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;

            Assert.IsNotNull(p);
            Assert.AreEqual(0, p.Version);
            Assert.AreEqual(32, p.Length);
            Assert.AreEqual(1, p.Count);

            var commonField = p.FindFirstByType(PpiFieldType.PpiCommon) as PpiCommon;

            Assert.AreEqual(PpiFieldType.PpiCommon, commonField.FieldType);
            Assert.AreEqual(0, commonField.TSFTimer);
            Assert.IsTrue((commonField.Flags & PpiCommon.CommonFlags.FcsIncludedInFrame) == PpiCommon.CommonFlags.FcsIncludedInFrame);
            Assert.AreEqual(2, commonField.Rate);
            Assert.AreEqual(2437, commonField.ChannelFrequency);
            Assert.AreEqual(0x00A0, (int) commonField.ChannelFlags);
            Assert.AreEqual(0, commonField.FhssHopset);
            Assert.AreEqual(0, commonField.FhssPattern);
            Assert.AreEqual(-84, commonField.AntennaSignalPower);
            Assert.AreEqual(-100, commonField.AntennaSignalNoise);

            var macFrame = p.PayloadPacket as MacFrame;
            Assert.AreEqual(FrameControlField.FrameSubTypes.ControlCts, macFrame.FrameControl.SubType);
            Assert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithInvalidFcs()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_fcs_present_and_invalid.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;

            //The packet is corrupted in such a way that the type field has been changed
            //to a reserved/unused type. Therefore we don't expect there to be a packet
            Assert.IsNull(p.PayloadPacket);
            Assert.IsNotNull(p.PayloadData);
        }

        [Test]
        public void ReadPacketWithNoFcs()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_without_fcs.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
            Assert.IsNotNull(p.PayloadPacket);
            var macFrame = p.PayloadPacket as MacFrame;
            Assert.IsFalse(macFrame.FcsValid);
            Assert.IsFalse(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithValidFcs()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_fcs_present_and_valid.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
            Assert.IsNotNull(p.PayloadPacket);
            var macFrame = p.PayloadPacket as MacFrame;
            Assert.IsTrue(macFrame.FcsValid);
            Assert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void RemoveField()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_multiplefields.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
            var expectedLength = p.Length - p[1].Length - PpiHeaderFields.FieldHeaderLength;
            p.Remove(p[1]);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;
            Assert.AreEqual(expectedLength, recreatedPacket.Length);
            Assert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            Assert.IsFalse(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            Assert.IsNotNull(macFrame);
            Assert.IsTrue(macFrame.FcsValid);
        }

        [Test]
        public void RemoveFieldByType()
        {
            var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_ppi_multiplefields.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as PpiPacket;
            p.RemoveAll(PpiFieldType.PpiMacPhy);

            var recreatedPacket = Packet.ParsePacket(LinkLayers.Ppi, p.Bytes) as PpiPacket;

            Assert.IsTrue(recreatedPacket.Contains(PpiFieldType.PpiCommon));
            Assert.IsFalse(recreatedPacket.Contains(PpiFieldType.PpiMacPhy));

            var macFrame = recreatedPacket.PayloadPacket as MacFrame;
            Assert.IsNotNull(macFrame);
            Assert.IsTrue(macFrame.FcsValid);
        }
    }
}