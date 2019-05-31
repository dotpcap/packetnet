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

using System.IO;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using PacketDotNet.Utils.Converters;
using SharpPcap.LibPcap;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class RadioPacketTest
    {
        [Test]
        public void ConstructFrameWithSinglePresenceField()
        {
            var p = new RadioPacket();

            var expectedLength = 8;
            Assert.AreEqual(expectedLength, p.Length);

            var flagsField = new FlagsRadioTapField();
            flagsField.Flags |= RadioTapFlags.FcsIncludedInFrame;
            flagsField.Flags |= RadioTapFlags.WepEncrypted;
            p.Add(flagsField);

            expectedLength += flagsField.Length;
            Assert.AreEqual(expectedLength, p.Length);

            //We will add the noise field before the signal field. This is not the order required
            //for radiotap and so will test that the fields are correctly reordered when written
            var dbAntennaNoiseField = new DbAntennaNoiseRadioTapField { AntennaNoisedB = 33 };
            p.Add(dbAntennaNoiseField);

            expectedLength += dbAntennaNoiseField.Length;
            Assert.AreEqual(expectedLength, p.Length);

            var dbAntennaSignalField = new DbAntennaSignalRadioTapField { SignalStrengthdB = 44 };
            p.Add(dbAntennaSignalField);

            expectedLength += dbAntennaSignalField.Length;
            Assert.AreEqual(expectedLength, p.Length);

            //we will just put a single byte of data because we dont want it to be parsed into 
            //an 802.11 frame in this test
            p.PayloadData = new byte[] { 0xFF };

            var frameBytes = p.Bytes;

            var recreatedFrame = Packet.ParsePacket(LinkLayers.Ieee80211Radio, frameBytes) as RadioPacket;

            Assert.IsNotNull(p[RadioTapType.Flags]);
            Assert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            Assert.IsNotNull(p[RadioTapType.DbAntennaNoise]);
            Assert.AreEqual(new byte[] { 0xFF }, recreatedFrame.PayloadData);
        }

        /// <summary>
        /// Test that parsing an ip packet yields the proper field values
        /// </summary>
        [Test]
        public void ReadingPacketsFromFile()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
            Assert.IsNotNull(p);
            Assert.IsNotNull(p[RadioTapType.Flags]);
            Assert.IsNotNull(p[RadioTapType.Rate]);
            Assert.IsNotNull(p[RadioTapType.Channel]);
            Assert.IsNotNull(p[RadioTapType.DbmAntennaSignal]);
            Assert.IsNotNull(p[RadioTapType.DbmAntennaNoise]);
            Assert.IsNotNull(p[RadioTapType.LockQuality]);
            Assert.IsNotNull(p[RadioTapType.Antenna]);
            Assert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            var macFrame = p.PayloadPacket as MacFrame;
            Assert.IsNotNull(macFrame);
            Assert.IsTrue(macFrame.AppendFcs);
        }

        [Test]
        public void ReadPacketWithNoFcs()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_radio_without_fcs.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
            Assert.IsNotNull(p.PayloadPacket);

            var tsftField = p[RadioTapType.Tsft] as TsftRadioTapField;
            Assert.IsNotNull(tsftField);
            Assert.AreEqual(38724775, tsftField.TimestampUsec);

            var flagsField = p[RadioTapType.Flags] as FlagsRadioTapField;
            Assert.IsNotNull(flagsField);
            Assert.AreEqual(0, (int) flagsField.Flags);

            var rateField = p[RadioTapType.Rate] as RateRadioTapField;
            Assert.IsNotNull(rateField);
            Assert.AreEqual(1, rateField.RateMbps);

            var channelField = p[RadioTapType.Channel] as ChannelRadioTapField;
            Assert.IsNotNull(channelField);
            Assert.AreEqual(2462, channelField.FrequencyMHz);
            Assert.AreEqual(11, channelField.Channel);
            Assert.AreEqual(RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Cck, channelField.Flags);

            var dbmSignalField = p[RadioTapType.DbmAntennaSignal] as DbmAntennaSignalRadioTapField;
            Assert.IsNotNull(dbmSignalField);
            Assert.AreEqual(-61, dbmSignalField.AntennaSignalDbm);

            var dbmNoiseField = p[RadioTapType.DbmAntennaNoise] as DbmAntennaNoiseRadioTapField;
            Assert.IsNotNull(dbmNoiseField);
            Assert.AreEqual(-84, dbmNoiseField.AntennaNoisedBm);

            var antennaField = p[RadioTapType.Antenna] as AntennaRadioTapField;
            Assert.IsNotNull(antennaField);
            Assert.AreEqual(0, antennaField.Antenna);

            var dbSignalField = p[RadioTapType.DbAntennaSignal] as DbAntennaSignalRadioTapField;
            Assert.IsNotNull(dbSignalField);
            Assert.AreEqual(23, dbSignalField.SignalStrengthdB);

            var macFrame = p.PayloadPacket as MacFrame;
            Assert.IsFalse(macFrame.AppendFcs);
            Assert.IsFalse(macFrame.FcsValid);
        }

        [Test]
        public void RemoveRadioTapField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
            Assert.IsNotNull(p);
            Assert.IsNotNull(p[RadioTapType.Flags]);
            Assert.IsNotNull(p[RadioTapType.Rate]);
            Assert.IsNotNull(p[RadioTapType.Channel]);
            Assert.IsNotNull(p[RadioTapType.DbmAntennaSignal]);
            Assert.IsNotNull(p[RadioTapType.DbmAntennaNoise]);
            Assert.IsNotNull(p[RadioTapType.LockQuality]);
            Assert.IsNotNull(p[RadioTapType.Antenna]);
            Assert.IsNotNull(p[RadioTapType.DbAntennaSignal]);
            var macFrame = p.PayloadPacket as MacFrame;
            Assert.IsNotNull(macFrame);
            Assert.IsTrue(macFrame.AppendFcs);
            Assert.IsTrue(macFrame.FcsValid);

            //Now remove a couple of radio tap fields and check that it is still valid
            p.Remove(RadioTapType.Rate);
            p.Remove(RadioTapType.Antenna);

            var recreatedFrame = Packet.ParsePacket(rawCapture.LinkLayerType, p.Bytes) as RadioPacket;
            Assert.IsNotNull(recreatedFrame);
            Assert.IsNotNull(recreatedFrame[RadioTapType.Flags]);
            Assert.IsNull(recreatedFrame[RadioTapType.Rate]);
            Assert.IsNotNull(recreatedFrame[RadioTapType.Channel]);
            Assert.IsNotNull(recreatedFrame[RadioTapType.DbmAntennaSignal]);
            Assert.IsNotNull(recreatedFrame[RadioTapType.DbmAntennaNoise]);
            Assert.IsNotNull(recreatedFrame[RadioTapType.LockQuality]);
            Assert.IsNull(recreatedFrame[RadioTapType.Antenna]);
            Assert.IsNotNull(recreatedFrame[RadioTapType.DbAntennaSignal]);
            var recreatedMacFrame = p.PayloadPacket as MacFrame;
            Assert.IsNotNull(recreatedMacFrame);
            Assert.IsTrue(recreatedMacFrame.AppendFcs);
            Assert.IsTrue(recreatedMacFrame.FcsValid);
        }

        [Test]
        public void TestContainsField()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
            Assert.IsNotNull(p);
            Assert.IsTrue(p.Contains(RadioTapType.Flags));
            Assert.IsFalse(p.Contains(RadioTapType.Fhss));
        }

        [Test]
        public void UnhandledRadioTapField()
        {
            var ms = new MemoryStream();
            var bs = new BinaryWriter(ms);
            bs.Write((byte) 0x0); //version
            bs.Write((byte) 0x0); //pad
            bs.Write((ushort) 0x0010); //length
            bs.Write(0x80000002); //present 1 (wth flags field)
            bs.Write((uint) 0x00010000); //present 2 (with unhandled field)
            bs.Write((ushort) 0x0010); //Flags field (FCS included flag set)
            bs.Write((ushort) 0x1234); //a made up field that we want to keep even though we dont know what it is

            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "80211_plus_radiotap_header.pcap");
            dev.Open();
            var rawCapture = dev.GetNextPacket();
            dev.Close();

            var anotherRadioPacket = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
            bs.Write(anotherRadioPacket.PayloadPacket.Bytes);

            var radioTap = ms.ToArray();

            var p = Packet.ParsePacket(LinkLayers.Ieee80211Radio, radioTap) as RadioPacket;
            Assert.AreEqual(16, p.Length);
            p.Add(new TsftRadioTapField(0x123456789));
            var finalFrame = Packet.ParsePacket(LinkLayers.Ieee80211Radio, p.Bytes) as RadioPacket;

            Assert.AreEqual(24, finalFrame.Length);
            Assert.AreEqual(0x1234, EndianBitConverter.Little.ToUInt16(finalFrame.Bytes, finalFrame.Length - 2));
        }
    }
}