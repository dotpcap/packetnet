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
using PacketDotNet.Ieee80211;
using System.IO;
using MiscUtil.Conversion;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class RadioPacketTest
        {
            /// <summary>
            /// Test that parsing an ip packet yields the proper field values
            /// </summary>
            [Test]
            public void ReadingPacketsFromFile()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_plus_radiotap_header.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                RadioPacket p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                Assert.IsNotNull(p);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_FLAGS]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_RATE]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_CHANNEL]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_ANTENNA]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL]);
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.IsNotNull(macFrame);
                Assert.IsTrue(macFrame.AppendFcs);
            }
			
			[Test]
			public void ReadPacketWithNoFcs()
			{
				var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_radio_without_fcs.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();
                
                RadioPacket p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                Assert.IsNotNull (p.PayloadPacket);

                TsftRadioTapField tsftField = p[RadioTapType.IEEE80211_RADIOTAP_TSFT] as TsftRadioTapField;
                Assert.IsNotNull(tsftField);
                Assert.AreEqual(38724775, tsftField.TimestampUsec);
                
                FlagsRadioTapField flagsField = p[RadioTapType.IEEE80211_RADIOTAP_FLAGS] as FlagsRadioTapField;
                Assert.IsNotNull(flagsField);
                Assert.AreEqual(RadioTapFlags.None, flagsField.Flags);
                
                RateRadioTapField rateField = p[RadioTapType.IEEE80211_RADIOTAP_RATE] as RateRadioTapField;
                Assert.IsNotNull(rateField);
                Assert.AreEqual(1, rateField.RateMbps);
                    
                ChannelRadioTapField channelField = p[RadioTapType.IEEE80211_RADIOTAP_CHANNEL] as ChannelRadioTapField;
                Assert.IsNotNull(channelField);
                Assert.AreEqual(2462, channelField.FrequencyMHz);
                Assert.AreEqual(11, channelField.Channel);
                Assert.AreEqual(RadioTapChannelFlags.IEEE80211_CHAN_2GHZ | RadioTapChannelFlags.IEEE80211_CHAN_CCK, channelField.Flags);
                
                DbmAntennaSignalRadioTapField dbmSignalField = p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL] as DbmAntennaSignalRadioTapField;
                Assert.IsNotNull(dbmSignalField);
                Assert.AreEqual(-61, dbmSignalField.AntennaSignalDbm);
                
                DbmAntennaNoiseRadioTapField dbmNoiseField = p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE] as DbmAntennaNoiseRadioTapField;
                Assert.IsNotNull(dbmNoiseField);
                Assert.AreEqual(-84, dbmNoiseField.AntennaNoisedBm);
                
                AntennaRadioTapField antennaField = p[RadioTapType.IEEE80211_RADIOTAP_ANTENNA] as AntennaRadioTapField;
                Assert.IsNotNull(antennaField);
                Assert.AreEqual(0, antennaField.Antenna);
                
                DbAntennaSignalRadioTapField dbSignalField = p[RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL] as DbAntennaSignalRadioTapField;
                Assert.IsNotNull(dbSignalField);
                Assert.AreEqual(23, dbSignalField.SignalStrengthdB);
                
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.IsFalse(macFrame.AppendFcs);
                Assert.IsFalse(macFrame.FCSValid);
			}
   
            [Test]
            public void ConstructFrameWithSinglePresenceField()
            {
                RadioPacket p = new RadioPacket();
                
                var expectedLength = 8;
                Assert.AreEqual(expectedLength, p.Length);
                
                FlagsRadioTapField flagsField = new FlagsRadioTapField();
                flagsField.Flags |= RadioTapFlags.FcsIncludedInFrame;
                flagsField.Flags |= RadioTapFlags.WepEncrypted;
                p.Add(flagsField);
                
                expectedLength += flagsField.Length;
                Assert.AreEqual(expectedLength, p.Length);
                
                //We will add the noise field before the signal field. This is not the order required
                //for radiotap and so will test that the fields are correctly reordered when written
                DbAntennaNoiseRadioTapField dbAntennaNoiseField = new DbAntennaNoiseRadioTapField();
                dbAntennaNoiseField.AntennaNoisedB = 33;
                p.Add(dbAntennaNoiseField);
                
                expectedLength += dbAntennaNoiseField.Length;
                Assert.AreEqual(expectedLength, p.Length);
                
                DbAntennaSignalRadioTapField dbAntennaSignalField = new DbAntennaSignalRadioTapField();
                dbAntennaSignalField.SignalStrengthdB = 44;
                p.Add(dbAntennaSignalField);
                
                expectedLength += dbAntennaSignalField.Length;
                Assert.AreEqual(expectedLength, p.Length);
                
                //we will just put a single byte of data because we dont want it to be parsed into 
                //an 802.11 frame in this test
                p.PayloadData = new byte[]{0xFF};
                
                var frameBytes = p.Bytes;
                
                RadioPacket recreatedFrame = Packet.ParsePacket(LinkLayers.Ieee80211_Radio, frameBytes) as RadioPacket;
                
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_FLAGS]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DB_ANTNOISE]);
                Assert.AreEqual(new byte[]{0xFF}, recreatedFrame.PayloadData);
            }
            
            
            [Test]
            public void RemoveRadioTapField()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_plus_radiotap_header.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                RadioPacket p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                Assert.IsNotNull(p);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_FLAGS]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_RATE]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_CHANNEL]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_ANTENNA]);
                Assert.IsNotNull(p[RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL]);
                MacFrame macFrame = p.PayloadPacket as MacFrame;
                Assert.IsNotNull(macFrame);
                Assert.IsTrue(macFrame.AppendFcs);
                Assert.IsTrue(macFrame.FCSValid);
                
                //Now remove a couple of radio tap fields and check that it is still valid
                p.Remove(RadioTapType.IEEE80211_RADIOTAP_RATE);
                p.Remove(RadioTapType.IEEE80211_RADIOTAP_ANTENNA);
                
                RadioPacket recreatedFrame = Packet.ParsePacket(rawCapture.LinkLayerType, p.Bytes) as RadioPacket;
                Assert.IsNotNull(recreatedFrame);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_FLAGS]);
                Assert.IsNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_RATE]);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_CHANNEL]);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL]);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE]);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY]);
                Assert.IsNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_ANTENNA]);
                Assert.IsNotNull(recreatedFrame[RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL]);
                MacFrame recreatedMacFrame = p.PayloadPacket as MacFrame;
                Assert.IsNotNull(recreatedMacFrame);
                Assert.IsTrue(recreatedMacFrame.AppendFcs);
                Assert.IsTrue(recreatedMacFrame.FCSValid);
            }
            
            [Test]
            public void UnhandledRadioTapField()
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bs = new BinaryWriter(ms);
                bs.Write((byte)0x0); //version
                bs.Write((byte)0x0); //pad
                bs.Write((ushort) 0x0010); //length
                bs.Write((uint) 0x80000002); //present 1 (wth flags field)
                bs.Write((uint) 0x00010000); //present 2 (with unhandled field)
                bs.Write((ushort) 0x0010); //Flags field (FCS included flag set)
                bs.Write((ushort) 0x1234); //a made up field that we want to keep even though we dont know what it is
                
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_plus_radiotap_header.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();
    
                RadioPacket anotherRadioPacket = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                bs.Write(anotherRadioPacket.PayloadPacket.Bytes);
                
                var radioTap = ms.ToArray();
                
                RadioPacket p = Packet.ParsePacket(LinkLayers.Ieee80211_Radio, radioTap) as RadioPacket;
                Assert.AreEqual(16, p.Length);
                p.Add(new TsftRadioTapField(0x123456789));
                RadioPacket finalFrame = Packet.ParsePacket(LinkLayers.Ieee80211_Radio, p.Bytes) as RadioPacket;
                
                Assert.AreEqual(24, finalFrame.Length);
                Assert.AreEqual(0x1234, EndianBitConverter.Little.ToUInt16(finalFrame.Bytes, finalFrame.Length - 2));
            }
            
            [Test]
            public void TestContainsField ()
            {
                var dev = new CaptureFileReaderDevice("../../CaptureFiles/80211_plus_radiotap_header.pcap");
                dev.Open();
                var rawCapture = dev.GetNextPacket();
                dev.Close();

                RadioPacket p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as RadioPacket;
                Assert.IsNotNull(p);
                Assert.IsTrue(p.Contains(RadioTapType.IEEE80211_RADIOTAP_FLAGS));
                Assert.IsFalse(p.Contains(RadioTapType.IEEE80211_RADIOTAP_FHSS));
            }
        } 
    }
}
