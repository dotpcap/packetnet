/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.IO;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils.Converters;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class RadioPacketTest
    {
        [Test]
        public void ConstructFrameWithSinglePresenceField()
        {
            var p = new RadioPacket();

            var expectedLength = 8;
            ClassicAssert.AreEqual(expectedLength, p.Length);

            var flagsField = new FlagsRadioTapField();
            flagsField.Flags |= RadioTapFlags.FcsIncludedInFrame;
            flagsField.Flags |= RadioTapFlags.WepEncrypted;
            p.Add(flagsField);

            expectedLength += flagsField.Length;
            ClassicAssert.AreEqual(expectedLength, p.Length);

            //We will add the noise field before the signal field. This is not the order required
            //for radiotap and so will test that the fields are correctly reordered when written
            var dbAntennaNoiseField = new DbAntennaNoiseRadioTapField { AntennaNoisedB = 33 };
            p.Add(dbAntennaNoiseField);

            expectedLength += dbAntennaNoiseField.Length;
            ClassicAssert.AreEqual(expectedLength, p.Length);

            var dbAntennaSignalField = new DbAntennaSignalRadioTapField { SignalStrengthdB = 44 };
            p.Add(dbAntennaSignalField);

            expectedLength += dbAntennaSignalField.Length;
            ClassicAssert.AreEqual(expectedLength, p.Length);

            //we will just put a single byte of data because we dont want it to be parsed into 
            //an 802.11 frame in this test
            p.PayloadData = new byte[] { 0xFF };

            var frameBytes = p.Bytes;

            var recreatedFrame = Packet.ParsePacket(LinkLayers.Ieee80211RadioTap, frameBytes) as RadioPacket;

            ClassicAssert.IsNotNull(p[RadioTapType.Flags]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbAntennaNoise]);
            ClassicAssert.AreEqual(new byte[] { 0xFF }, recreatedFrame.PayloadData);
        }

        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            SharpPcap.PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;
            ClassicAssert.IsNotNull(p);
            ClassicAssert.IsNotNull(p[RadioTapType.Flags]);
            ClassicAssert.IsNotNull(p[RadioTapType.Rate]);
            ClassicAssert.IsNotNull(p[RadioTapType.Channel]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbmAntennaSignal]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbmAntennaNoise]);
            ClassicAssert.IsNotNull(p[RadioTapType.LockQuality]);
            ClassicAssert.IsNotNull(p[RadioTapType.Antenna]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(macFrame);
            ClassicAssert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithNoFcs()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_radio_without_fcs.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;
            ClassicAssert.IsNotNull(p.PayloadPacket);

            var tsftField = p[RadioTapType.Tsft] as TsftRadioTapField;
            ClassicAssert.IsNotNull(tsftField);
            ClassicAssert.AreEqual(38724775, tsftField.TimestampUsec);

            var flagsField = p[RadioTapType.Flags] as FlagsRadioTapField;
            ClassicAssert.IsNotNull(flagsField);
            ClassicAssert.AreEqual(0, (int) flagsField.Flags);

            var rateField = p[RadioTapType.Rate] as RateRadioTapField;
            ClassicAssert.IsNotNull(rateField);
            ClassicAssert.AreEqual(1, rateField.RateMbps);

            var channelField = p[RadioTapType.Channel] as ChannelRadioTapField;
            ClassicAssert.IsNotNull(channelField);
            ClassicAssert.AreEqual(2462, channelField.FrequencyMHz);
            ClassicAssert.AreEqual(11, channelField.Channel);
            ClassicAssert.AreEqual(RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Cck, channelField.Flags);

            var dbmSignalField = p[RadioTapType.DbmAntennaSignal] as DbmAntennaSignalRadioTapField;
            ClassicAssert.IsNotNull(dbmSignalField);
            ClassicAssert.AreEqual(-61, dbmSignalField.AntennaSignalDbm);

            var dbmNoiseField = p[RadioTapType.DbmAntennaNoise] as DbmAntennaNoiseRadioTapField;
            ClassicAssert.IsNotNull(dbmNoiseField);
            ClassicAssert.AreEqual(-84, dbmNoiseField.AntennaNoisedBm);

            var antennaField = p[RadioTapType.Antenna] as AntennaRadioTapField;
            ClassicAssert.IsNotNull(antennaField);
            ClassicAssert.AreEqual(0, antennaField.Antenna);

            var dbSignalField = p[RadioTapType.DbAntennaSignal] as DbAntennaSignalRadioTapField;
            ClassicAssert.IsNotNull(dbSignalField);
            ClassicAssert.AreEqual(23, dbSignalField.SignalStrengthdB);

            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsFalse(macFrame.AppendFcs);
            ClassicAssert.IsFalse(macFrame.FcsValid);
        }

        [Test]
        public void RemoveRadioTapField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;
            ClassicAssert.IsNotNull(p);
            ClassicAssert.IsNotNull(p[RadioTapType.Flags]);
            ClassicAssert.IsNotNull(p[RadioTapType.Rate]);
            ClassicAssert.IsNotNull(p[RadioTapType.Channel]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbmAntennaSignal]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbmAntennaNoise]);
            ClassicAssert.IsNotNull(p[RadioTapType.LockQuality]);
            ClassicAssert.IsNotNull(p[RadioTapType.Antenna]);
            ClassicAssert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            var macFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(macFrame);
            ClassicAssert.IsTrue(macFrame.AppendFcs);
            ClassicAssert.IsTrue(macFrame.FcsValid);

            //Now remove a couple of radio tap fields and check that it is still valid
            p.Remove(RadioTapType.Rate);
            p.Remove(RadioTapType.Antenna);

            var recreatedFrame = Packet.ParsePacket(rawCapture.GetLinkLayers(), p.Bytes) as RadioPacket;
            ClassicAssert.IsNotNull(recreatedFrame);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.Flags]);
            ClassicAssert.IsNull(recreatedFrame[RadioTapType.Rate]);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.Channel]);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.DbmAntennaSignal]);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.DbmAntennaNoise]);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.LockQuality]);
            ClassicAssert.IsNull(recreatedFrame[RadioTapType.Antenna]);
            ClassicAssert.IsNotNull(recreatedFrame[RadioTapType.DbAntennaSignal]);
            var recreatedMacFrame = p.PayloadPacket as MacFrame;
            ClassicAssert.IsNotNull(recreatedMacFrame);
            ClassicAssert.IsTrue(recreatedMacFrame.AppendFcs);
            ClassicAssert.IsTrue(recreatedMacFrame.FcsValid);
        }

        [Test]
        public void TestContainsField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;
            ClassicAssert.IsNotNull(p);
            ClassicAssert.IsTrue(p.Contains(RadioTapType.Flags));
            ClassicAssert.IsFalse(p.Contains(RadioTapType.Fhss));
        }

        [Test]
        public void UnhandledRadioTapField()
        {
            var ms = new MemoryStream();
            var bs = new BinaryWriter(ms);
            bs.Write((byte) 0x0); //version
            bs.Write((byte) 0x0); //pad
            var originalLength = 0x10;
            bs.Write((ushort) originalLength); //length
            bs.Write(0x80000002); //present 1 (wth flags field)
            bs.Write((uint) 0x00010000); //present 2 (with unhandled field)
            bs.Write((ushort) 0x0010); //Flags field (FCS included flag set)
            bs.Write((ushort) 0x1234); //a made up field that we want to keep even though we dont know what it is

            // write the content of the packet in the captured file, without header, to the memory stream to continue to build up the generated
            // radiotap packet
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var anotherRadioPacket = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;
            var anotherRadioPacketBytes = anotherRadioPacket.PayloadPacket.Bytes;
            bs.Write(anotherRadioPacketBytes);

            var radioTap = ms.ToArray();
            var p = Packet.ParsePacket(LinkLayers.Ieee80211RadioTap, radioTap) as RadioPacket;

            var numberOfPayloadBytesWrittenFromAnotherRadioPacket = 14;
            ClassicAssert.AreEqual(numberOfPayloadBytesWrittenFromAnotherRadioPacket, p.PayloadPacket.Bytes.Length);
            ClassicAssert.AreEqual(originalLength, p.Length);
            ulong newTsftValue = 0x123456789;
            p.Add(new TsftRadioTapField(newTsftValue));
            var timestampUsec = ((TsftRadioTapField)p[RadioTapType.Tsft]).TimestampUsec;
            ClassicAssert.AreEqual(newTsftValue, timestampUsec);
            var paddingByteCount = 4;
            var tsftLength = 8; // TODO: We have no way to get this without creating an instance of this RadioTapField
            var addedLength = tsftLength + paddingByteCount;
            var radioTapWithTsftBytes = p.Bytes;

            var finalFrame = Packet.ParsePacket(LinkLayers.Ieee80211RadioTap, radioTapWithTsftBytes) as RadioPacket;

            ClassicAssert.AreEqual(originalLength + addedLength, finalFrame.Length);
            ClassicAssert.AreEqual(0x1234, EndianBitConverter.Little.ToUInt16(finalFrame.Bytes, finalFrame.Length - 2));
            ClassicAssert.AreEqual(numberOfPayloadBytesWrittenFromAnotherRadioPacket, p.PayloadPacket.Bytes.Length);
        }

        [Test]
        public void ExtendedPresenceMaskFlags()
        {
            //Test for https://www.radiotap.org/#extended-presence-masks
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_radiotap_with_extended_presence_mask.pcap");
            dev.Open();
            PacketCapture c;
            dev.GetNextPacket(out c);
            var rawCapture = c.GetPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data) as RadioPacket;

            var timestampUsec = ((TsftRadioTapField)p[RadioTapType.Tsft]).TimestampUsec;
            ClassicAssert.AreEqual(5508414380885, timestampUsec);

            var rate = ((RateRadioTapField)p[RadioTapType.Rate]).RateMbps;
            ClassicAssert.AreEqual(1.0f, rate);
        }
    }
