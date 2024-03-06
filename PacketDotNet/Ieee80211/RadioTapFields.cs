/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Ieee80211;


    /// <summary>
    /// The presence of this field indicates that the frame was received or transmitted using the VHT PHY (Wi-Fi 5 / ieee802.11ac).
    /// </summary>
    public class VeryHighThroughputRadioTapField : RadioTapField
    {
        public RadioTapVhtKnown Known { get; set; }
        public RadioTapVhtFlags Flags { get; set; }
        public RadioTapVhtBandwidth Bandwidth { get; set; }
        public RadioTapVhtMcsNss McsNss1 { get; set; }
        public RadioTapVhtMcsNss McsNss2 { get; set; }
        public RadioTapVhtMcsNss McsNss3 { get; set; }
        public RadioTapVhtMcsNss McsNss4 { get; set; }

        /// <summary>
        /// The coding for a user is only valid if the NSS (in the mcs_nss field) for that user is nonzero.
        /// </summary>
        public RadioTapVhtCoding Coding { get; set; }
        public byte GroupId { get; set; }
        public ushort PartialAid { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public VeryHighThroughputRadioTapField(BinaryReader br)
        {
            Known = (RadioTapVhtKnown)br.ReadUInt16();
            Flags = (RadioTapVhtFlags)br.ReadByte();
            Bandwidth = (RadioTapVhtBandwidth)br.ReadByte();
            McsNss1 = (RadioTapVhtMcsNss)br.ReadByte();
            McsNss2 = (RadioTapVhtMcsNss)br.ReadByte();
            McsNss3 = (RadioTapVhtMcsNss)br.ReadByte();
            McsNss4 = (RadioTapVhtMcsNss)br.ReadByte();
            Coding = (RadioTapVhtCoding)br.ReadByte();
            GroupId = br.ReadByte();
            PartialAid = br.ReadUInt16();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="VeryHighThroughputRadioTapField" /> class.
        /// </summary>
        public VeryHighThroughputRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VeryHighThroughputRadioTapField" /> class.
        /// </summary>
        public VeryHighThroughputRadioTapField(
            RadioTapVhtKnown known, 
            RadioTapVhtFlags flags, 
            RadioTapVhtBandwidth bandwidth,
            RadioTapVhtMcsNss mcsNss1,
            RadioTapVhtMcsNss mcsNss2,
            RadioTapVhtMcsNss mcsNss3,
            RadioTapVhtMcsNss mcsNss4,
            RadioTapVhtCoding coding,
            byte groupId,
            ushort partialAid)
        {
            Known = known;
            Flags = flags;
            Bandwidth = bandwidth;
            McsNss1 = mcsNss1;
            McsNss2 = mcsNss2;
            McsNss3 = mcsNss3;
            McsNss4 = mcsNss4;
            Coding = coding;
            GroupId = groupId;
            PartialAid = partialAid;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.VeryHighThroughput;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 12;

        /// <summary>
        /// 
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => 2;


        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes((ushort)Known, dest, offset);
            EndianBitConverter.Little.CopyBytes((byte)Flags, dest, offset + 2);
            EndianBitConverter.Little.CopyBytes((byte)Bandwidth, dest, offset + 3);
            EndianBitConverter.Little.CopyBytes((byte)McsNss1, dest, offset + 4);
            EndianBitConverter.Little.CopyBytes((byte)McsNss2, dest, offset + 5);
            EndianBitConverter.Little.CopyBytes((byte)McsNss3, dest, offset + 6);
            EndianBitConverter.Little.CopyBytes((byte)McsNss4, dest, offset + 7);
            EndianBitConverter.Little.CopyBytes((byte)Coding, dest, offset + 8);
            EndianBitConverter.Little.CopyBytes(GroupId, dest, offset + 9);
            EndianBitConverter.Little.CopyBytes(PartialAid, dest, offset + 10);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"Known {Known}, Flags {Flags}, Bandwidth {Bandwidth}, McsNss1 {McsNss1}, McsNss2 {McsNss2}, McsNss3 {McsNss3}, McsNss4 {McsNss4}, Coding {Coding}, GroupId {GroupId}, PartialAid {PartialAid}";
        }
    }

    /// <summary>
    /// The presence of this field indicates that the frame was received or transmitted using the HE PHY (Wi-Fi 6 / ieee802.11ax).
    /// </summary>
    public class HighEfficiencyRadioTapField : RadioTapField
    {
        public RadioTapHighEfficiencyData1 Data1 { get; set; }
        public RadioTapHighEfficiencyData2 Data2 { get; set; }
        public ushort Data3 { get; set; }
        public ushort Data4 { get; set; }
        public ushort Data5 { get; set; }
        public ushort Data6 { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public HighEfficiencyRadioTapField(BinaryReader br)
        {
            Data1 = (RadioTapHighEfficiencyData1)br.ReadUInt16();
            Data2 = (RadioTapHighEfficiencyData2)br.ReadUInt16();
            Data3 = br.ReadUInt16();
            Data4 = br.ReadUInt16();
            Data5 = br.ReadUInt16();
            Data6 = br.ReadUInt16();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HighEfficiencyRadioTapField" /> class.
        /// </summary>
        public HighEfficiencyRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighEfficiencyRadioTapField" /> class.
        /// </summary>
        public HighEfficiencyRadioTapField(RadioTapHighEfficiencyData1 data1, RadioTapHighEfficiencyData2 data2, ushort data3, ushort data4, ushort data5, ushort data6)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;
            Data5 = data5;
            Data6 = data6;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.HighEfficiency;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 12;

        /// <summary>
        /// 
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => 2;


        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes((ushort)Data1, dest, offset);
            EndianBitConverter.Little.CopyBytes((ushort)Data2, dest, offset + 2);
            EndianBitConverter.Little.CopyBytes(Data3, dest, offset + 4);
            EndianBitConverter.Little.CopyBytes(Data4, dest, offset + 6);
            EndianBitConverter.Little.CopyBytes(Data5, dest, offset + 8);
            EndianBitConverter.Little.CopyBytes(Data6, dest, offset + 10);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"Data1 {Data1}, Data2 {Data2}, Data3 {Data3}, Data4 {Data4}, Data5 {Data5}, Data6 {Data6}";
        }
    }

    public class McsRadioTapField : RadioTapField
    {
        /// <summary>
        /// Indicates which information is known
        /// </summary>
        public RadioTapMcsKnown Known { get; set; }

        /// <summary>
        /// Indicates which information is known
        /// </summary>
        public RadioTapMcsFlags Flags { get; set; }

        public byte Mcs { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public McsRadioTapField(BinaryReader br)
        {
            Known = (RadioTapMcsKnown)br.ReadByte();
            Flags = (RadioTapMcsFlags)br.ReadByte();
            Mcs = br.ReadByte();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="McsRadioTapField" /> class.
        /// </summary>
        public McsRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="McsRadioTapField" /> class.
        /// <param name="known">Known information</param>
        /// <param name="flags">MCS Flags</param>
        /// <param name="mcs">Known information</param>
        /// </summary>
        public McsRadioTapField(RadioTapMcsKnown known, RadioTapMcsFlags flags, byte mcs)
        {
            Known = known;
            Flags = flags;
            Mcs = mcs;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Mcs;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 3;

        /// <summary>
        /// 
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => 1;


        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes((byte)Known, dest, offset);
            EndianBitConverter.Little.CopyBytes((byte)Flags, dest, offset + 1);
            EndianBitConverter.Little.CopyBytes((byte)Mcs, dest, offset + 2);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"Known {Known}, Flags {Flags}, Mcs {Mcs}";
        }
    }

    /// <summary>
    /// Channel field
    /// </summary>
    public class ChannelRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public ChannelRadioTapField(BinaryReader br)
        {
            FrequencyMHz = br.ReadUInt16();
            Channel = ChannelFromFrequencyMHz(FrequencyMHz);
            Flags = (RadioTapChannelFlags) br.ReadUInt16();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelRadioTapField" /> class.
        /// </summary>
        public ChannelRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelRadioTapField" /> class.
        /// </summary>
        /// <param name="frequencyMhz">Tx/Rx Frequency in MHz.</param>
        /// <param name="flags">Flags.</param>
        public ChannelRadioTapField(ushort frequencyMhz, RadioTapChannelFlags flags)
        {
            FrequencyMHz = frequencyMhz;
            Channel = ChannelFromFrequencyMHz(FrequencyMHz);
            Flags = flags;
        }

        /// <summary>
        /// Channel number derived from frequency
        /// </summary>
        public int Channel { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Channel;

        /// <summary>
        /// Gets the channel flags.
        /// </summary>
        public RadioTapChannelFlags Flags { get; }

        /// <summary>
        /// Frequency in MHz
        /// </summary>
        public ushort FrequencyMHz { get; set; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 4;

        /// <summary>
        /// Note that the alignment of the channel field is two bytes as it is comprised of two 2 byte fields
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => 2;

        /// <summary>
        /// Convert a frequency to a channel
        /// </summary>
        /// <remarks>
        /// There is some overlap between the 802.11b/g channel numbers and the 802.11a channel numbers. This means that while a particular frequncy will only
        /// ever map to single channel number the same channel number may be returned for more than one frequency. At present this affects channel numbers 8 and 12.
        /// </remarks>
        /// <param name="frequencyMHz">
        /// A <see cref="int" />
        /// </param>
        /// <returns>
        /// A <see cref="int" />
        /// </returns>
        public static int ChannelFromFrequencyMHz(int frequencyMHz)
        {
            switch (frequencyMHz)
            {
                //802.11 bg channel numbers
                case 2412:
                {
                    return 1;
                }
                case 2417:
                {
                    return 2;
                }
                case 2422:
                {
                    return 3;
                }
                case 2427:
                {
                    return 4;
                }
                case 2432:
                {
                    return 5;
                }
                case 2437:
                {
                    return 6;
                }
                case 2442:
                {
                    return 7;
                }
                case 2447:
                {
                    return 8;
                }
                case 2452:
                {
                    return 9;
                }
                case 2457:
                {
                    return 10;
                }
                case 2462:
                {
                    return 11;
                }
                case 2467:
                {
                    return 12;
                }
                case 2472:
                {
                    return 13;
                }
                case 2484:
                {
                    return 14;
                }
                //802.11 a channel numbers
                case 4920:
                {
                    return 240;
                }
                case 4940:
                {
                    return 244;
                }
                case 4960:
                {
                    return 248;
                }
                case 4980:
                {
                    return 252;
                }
                case 5040:
                {
                    return 8;
                }
                case 5060:
                {
                    return 12;
                }
                case 5080:
                {
                    return 16;
                }
                case 5170:
                {
                    return 34;
                }
                case 5180:
                {
                    return 36;
                }
                case 5190:
                {
                    return 38;
                }
                case 5200:
                {
                    return 40;
                }
                case 5210:
                {
                    return 42;
                }
                case 5220:
                {
                    return 44;
                }
                case 5230:
                {
                    return 46;
                }
                case 5240:
                {
                    return 48;
                }
                case 5260:
                {
                    return 52;
                }
                case 5280:
                {
                    return 56;
                }
                case 5300:
                {
                    return 60;
                }
                case 5320:
                {
                    return 64;
                }
                case 5500:
                {
                    return 100;
                }
                case 5520:
                {
                    return 104;
                }
                case 5540:
                {
                    return 108;
                }
                case 5560:
                {
                    return 112;
                }
                case 5580:
                {
                    return 116;
                }
                case 5600:
                {
                    return 120;
                }
                case 5620:
                {
                    return 124;
                }
                case 5640:
                {
                    return 128;
                }
                case 5660:
                {
                    return 132;
                }
                case 5680:
                {
                    return 136;
                }
                case 5700:
                {
                    return 140;
                }
                case 5745:
                {
                    return 149;
                }
                case 5765:
                {
                    return 153;
                }
                case 5785:
                {
                    return 157;
                }
                case 5805:
                {
                    return 161;
                }
                case 5825:
                {
                    return 165;
                }
                default:
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes(FrequencyMHz, dest, offset);
            EndianBitConverter.Little.CopyBytes((ushort) Flags, dest, offset + 2);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"FrequencyMHz {FrequencyMHz}, Channel {Channel}, Flags {Flags}";
        }
    }

    /// <summary>
    /// Fhss radio tap field
    /// </summary>
    public class FhssRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public FhssRadioTapField(BinaryReader br)
        {
            var u16 = br.ReadUInt16();

            ChannelHoppingSet = (byte) (u16 & 0xff);
            Pattern = (byte) ((u16 >> 8) & 0xff);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhssRadioTapField" /> class.
        /// </summary>
        public FhssRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FhssRadioTapField" /> class.
        /// </summary>
        /// <param name='channelHoppingSet'>
        /// Channel hopping set.
        /// </param>
        /// <param name='pattern'>
        /// Channel hopping pattern.
        /// </param>
        public FhssRadioTapField(byte channelHoppingSet, byte pattern)
        {
            ChannelHoppingSet = channelHoppingSet;
            Pattern = pattern;
        }

        /// <summary>
        /// Hop set
        /// </summary>
        public byte ChannelHoppingSet { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Fhss;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 2;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Hop pattern
        /// </summary>
        public byte Pattern { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = ChannelHoppingSet;
            dest[offset + 1] = Pattern;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"ChannelHoppingSet {ChannelHoppingSet}, Pattern {Pattern}";
        }
    }

    /// <summary>
    /// Radio tap flags
    /// </summary>
    public class FlagsRadioTapField : RadioTapField
    {
        /// <summary>
        /// Flags set
        /// </summary>
        public RadioTapFlags Flags;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public FlagsRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            Flags = (RadioTapFlags) u8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsRadioTapField" /> class.
        /// </summary>
        public FlagsRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsRadioTapField" /> class.
        /// </summary>
        /// <param name='flags'>
        /// Flags.
        /// </param>
        public FlagsRadioTapField(RadioTapFlags flags)
        {
            Flags = flags;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Flags;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) Flags;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"Flags {Flags}";
        }
    }

    /// <summary>
    /// Rate field
    /// </summary>
    public class RateRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public RateRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            RateMbps = 0.5 * (u8 & 0x7f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateRadioTapField" /> class.
        /// </summary>
        public RateRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateRadioTapField" /> class.
        /// </summary>
        /// <param name='rateMbps'>
        /// Rate mbps.
        /// </param>
        public RateRadioTapField(double rateMbps)
        {
            RateMbps = rateMbps;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Rate;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Rate in Mbps
        /// </summary>
        public double RateMbps { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) (RateMbps / 0.5);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"RateMbps {RateMbps}";
        }
    }

    /// <summary>
    /// Db antenna signal
    /// </summary>
    public class DbAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbAntennaSignalRadioTapField(BinaryReader br)
        {
            SignalStrengthdB = br.ReadByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAntennaSignalRadioTapField" /> class.
        /// </summary>
        public DbAntennaSignalRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAntennaSignalRadioTapField" /> class.
        /// </summary>
        /// <param name='signalStrengthdB'>
        /// Signal strength in dB
        /// </param>
        public DbAntennaSignalRadioTapField(byte signalStrengthdB)
        {
            SignalStrengthdB = signalStrengthdB;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbAntennaSignal;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Signal strength in dB
        /// </summary>
        public byte SignalStrengthdB { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = SignalStrengthdB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"SignalStrengthdB {SignalStrengthdB}";
        }
    }

    /// <summary>
    /// Antenna noise in dB
    /// </summary>
    public class DbAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedB = br.ReadByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAntennaNoiseRadioTapField" /> class.
        /// </summary>
        public DbAntennaNoiseRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbAntennaNoiseRadioTapField" /> class.
        /// </summary>
        /// <param name='antennaNoisedB'>
        /// Antenna signal noise in dB.
        /// </param>
        public DbAntennaNoiseRadioTapField(byte antennaNoisedB)
        {
            AntennaNoisedB = antennaNoisedB;
        }

        /// <summary>
        /// Antenna noise in dB
        /// </summary>
        public byte AntennaNoisedB { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbAntennaNoise;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = AntennaNoisedB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"AntennaNoisedB {AntennaNoisedB}";
        }
    }

    /// <summary>
    /// Antenna field
    /// </summary>
    public class AntennaRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public AntennaRadioTapField(BinaryReader br)
        {
            Antenna = br.ReadByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntennaRadioTapField" /> class.
        /// </summary>
        public AntennaRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntennaRadioTapField" /> class.
        /// </summary>
        /// <param name='antenna'>
        /// Antenna index of the Rx/Tx antenna for this packet. The first antenna is antenna 0.
        /// </param>
        public AntennaRadioTapField(byte antenna)
        {
            Antenna = antenna;
        }

        /// <summary>
        /// Antenna number
        /// </summary>
        public byte Antenna { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Antenna;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = Antenna;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"Antenna {Antenna}";
        }
    }

    /// <summary>
    /// Antenna signal in dBm
    /// </summary>
    public class DbmAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbmAntennaSignalRadioTapField(BinaryReader br)
        {
            AntennaSignalDbm = br.ReadSByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmAntennaSignalRadioTapField" /> class.
        /// </summary>
        public DbmAntennaSignalRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmAntennaSignalRadioTapField" /> class.
        /// </summary>
        /// <param name='antennaSignalDbm'>
        /// Antenna signal power in dB.
        /// </param>
        public DbmAntennaSignalRadioTapField(sbyte antennaSignalDbm)
        {
            AntennaSignalDbm = antennaSignalDbm;
        }

        /// <summary>
        /// Antenna signal in dBm
        /// </summary>
        public sbyte AntennaSignalDbm { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaSignal;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) AntennaSignalDbm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"AntennaSignalDbm {AntennaSignalDbm}";
        }
    }

    /// <summary>
    /// Antenna noise in dBm
    /// </summary>
    public class DbmAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbmAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedBm = br.ReadSByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmAntennaNoiseRadioTapField" /> class.
        /// </summary>
        public DbmAntennaNoiseRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmAntennaNoiseRadioTapField" /> class.
        /// </summary>
        /// <param name='antennaNoisedBm'>
        /// Antenna noise in dBm.
        /// </param>
        public DbmAntennaNoiseRadioTapField(sbyte antennaNoisedBm)
        {
            AntennaNoisedBm = antennaNoisedBm;
        }

        /// <summary>
        /// Antenna noise in dBm
        /// </summary>
        public sbyte AntennaNoisedBm { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaNoise;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) AntennaNoisedBm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"AntennaNoisedBm {AntennaNoisedBm}";
        }
    }

    /// <summary>
    /// Lock quality
    /// </summary>
    public class LockQualityRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public LockQualityRadioTapField(BinaryReader br)
        {
            SignalQuality = br.ReadUInt16();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockQualityRadioTapField" /> class.
        /// </summary>
        public LockQualityRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockQualityRadioTapField" /> class.
        /// </summary>
        /// <param name='signalQuality'>
        /// Signal quality.
        /// </param>
        public LockQualityRadioTapField(ushort signalQuality)
        {
            SignalQuality = signalQuality;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.LockQuality;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 2;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Signal quality
        /// </summary>
        public ushort SignalQuality { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes(SignalQuality, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"SignalQuality {SignalQuality}";
        }
    }

    /// <summary>
    /// Tsft radio tap field
    /// </summary>
    public class TsftRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public TsftRadioTapField(BinaryReader br)
        {
            TimestampUsec = br.ReadUInt64();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsftRadioTapField" /> class.
        /// </summary>
        public TsftRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TsftRadioTapField" /> class.
        /// </summary>
        /// <param name='timestampUsec'>
        /// Value in microseconds of the Time Synchronization Function timer
        /// </param>
        public TsftRadioTapField(ulong timestampUsec)
        {
            TimestampUsec = timestampUsec;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Tsft;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 8;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public ulong TimestampUsec { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes(TimestampUsec, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"TimestampUsec {TimestampUsec}";
        }
    }

    /// <summary>
    /// Contains properties about the received from.
    /// </summary>
    public class RxFlagsRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public RxFlagsRadioTapField(BinaryReader br)
        {
            var flags = br.ReadUInt16();
            PlcpCrcCheckFailed = (flags & 0x2) == 0x2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxFlagsRadioTapField" /> class.
        /// </summary>
        public RxFlagsRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxFlagsRadioTapField" /> class.
        /// </summary>
        /// <param name='plcpCrcCheckFailed'>
        /// PLCP CRC check failed.
        /// </param>
        public RxFlagsRadioTapField(bool plcpCrcCheckFailed)
        {
            PlcpCrcCheckFailed = plcpCrcCheckFailed;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.RxFlags;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 2;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Gets or sets a value indicating whether the frame failed the PLCP CRC check.
        /// </summary>
        /// <value>
        /// <c>true</c> if the PLCP CRC check failed; otherwise, <c>false</c>.
        /// </value>
        public bool PlcpCrcCheckFailed { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            var flags = (ushort) (PlcpCrcCheckFailed ? 0x2 : 0x0);
            EndianBitConverter.Little.CopyBytes(flags, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"PlcpCrcCheckFailed {PlcpCrcCheckFailed}";
        }
    }

    /// <summary>
    /// Transmit power expressed as unitless distance from max
    /// power set at factory calibration.  0 is max power.
    /// Monotonically nondecreasing with lower power levels.
    /// </summary>
    public class TxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public TxAttenuationRadioTapField(BinaryReader br)
        {
            TxPower = -br.ReadUInt16();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TxAttenuationRadioTapField" /> class.
        /// </summary>
        public TxAttenuationRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TxAttenuationRadioTapField" /> class.
        /// </summary>
        /// <param name='txPower'>
        /// Transmit power expressed as unitless distance from max power set at factory calibration. 0 is max power.
        /// </param>
        public TxAttenuationRadioTapField(int txPower)
        {
            TxPower = txPower;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.TxAttenuation;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 2;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Transmit power
        /// </summary>
        public int TxPower { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            var absValue = (ushort) Math.Abs(TxPower);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"TxPower {TxPower}";
        }
    }

    /// <summary>
    /// Transmit power expressed as decibel distance from max power
    /// set at factory calibration.  0 is max power.  Monotonically
    /// nondecreasing with lower power levels.
    /// </summary>
    public class DbTxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbTxAttenuationRadioTapField(BinaryReader br)
        {
            TxPowerdB = -br.ReadUInt16();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTxAttenuationRadioTapField" /> class.
        /// </summary>
        public DbTxAttenuationRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTxAttenuationRadioTapField" /> class.
        /// </summary>
        /// <param name='txPowerdB'>
        /// Transmit power expressed as decibel distance from max power set at factory calibration. 0 is max power.
        /// </param>
        public DbTxAttenuationRadioTapField(int txPowerdB)
        {
            TxPowerdB = txPowerdB;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbTxAttenuation;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 2;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Transmit power
        /// </summary>
        public int TxPowerdB { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            var absValue = (ushort) Math.Abs(TxPowerdB);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"TxPowerdB {TxPowerdB}";
        }
    }

    /// <summary>
    /// Transmit power expressed as dBm (decibels from a 1 milliwatt
    /// reference). This is the absolute power level measured at
    /// the antenna port.
    /// </summary>
    public class DbmTxPowerRadioTapField : RadioTapField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        public DbmTxPowerRadioTapField(BinaryReader br)
        {
            TxPowerdBm = br.ReadSByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmTxPowerRadioTapField" /> class.
        /// </summary>
        public DbmTxPowerRadioTapField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbmTxPowerRadioTapField" /> class.
        /// </summary>
        /// <param name='txPowerdBm'>
        /// Transmit power expressed as dBm (decibels from a 1 milliwatt reference).
        /// </param>
        public DbmTxPowerRadioTapField(sbyte txPowerdBm)
        {
            TxPowerdBm = txPowerdBm;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmTxPower;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Gets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public override ushort Alignment => Length;

        /// <summary>
        /// Tx power in dBm
        /// </summary>
        public sbyte TxPowerdBm { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) TxPowerdBm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"TxPowerdBm {TxPowerdBm}";
        }
    }

    /// <summary>
    /// Abstract class for all radio tap fields
    /// </summary>
    public abstract class RadioTapField
    {
        /// <summary>Type of the field</summary>
        public abstract RadioTapType FieldType { get; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public abstract ushort Length { get; }

        /// <summary>
        /// Gets the alignment length for this field.
        /// </summary>
        /// <value>The alignment.</value>
        public abstract ushort Alignment { get; }

        /// <summary>
        /// Based on the radiotap bitfield determine the fieldAlignment
        /// </summary>
        /// <returns><c>true</c>, if field was found, <c>false</c> otherwise.</returns>
        /// <param name="bitIndex">Bit index.</param>
        /// <param name="fieldAlignment">Field length.</param>
        public static bool FieldAlignment(int bitIndex, out ushort fieldAlignment)
        {
            // leverage the existing Parse() routine with a dummy buffer to decode a
            // RadioTapField instance so we can retrieve its length
            var emptyBuffer = new byte[32];
            var memoryStream = new MemoryStream(emptyBuffer);
            var binaryReader = new BinaryReader(memoryStream);

            var field = Parse(bitIndex, binaryReader);
            if (field != null)
            {
                fieldAlignment = field.Alignment;
                return true;
            }

            fieldAlignment = 0;
            return false;
        }

        /// <summary>
        /// Parse a radio tap field, indicated by bitIndex, from a given BinaryReader
        /// </summary>
        /// <param name="bitIndex">
        /// A <see cref="int" />
        /// </param>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        /// <returns>
        /// A <see cref="RadioTapField" />
        /// </returns>
        public static RadioTapField Parse(int bitIndex, BinaryReader br)
        {
            var type = (RadioTapType) bitIndex;
            switch (type)
            {
                case RadioTapType.Flags:
                {
                    return new FlagsRadioTapField(br);
                }
                case RadioTapType.Rate:
                {
                    return new RateRadioTapField(br);
                }
                case RadioTapType.DbAntennaSignal:
                {
                    return new DbAntennaSignalRadioTapField(br);
                }
                case RadioTapType.DbAntennaNoise:
                {
                    return new DbAntennaNoiseRadioTapField(br);
                }
                case RadioTapType.Antenna:
                {
                    return new AntennaRadioTapField(br);
                }
                case RadioTapType.DbmAntennaSignal:
                {
                    return new DbmAntennaSignalRadioTapField(br);
                }
                case RadioTapType.DbmAntennaNoise:
                {
                    return new DbmAntennaNoiseRadioTapField(br);
                }
                case RadioTapType.Channel:
                {
                    return new ChannelRadioTapField(br);
                }
                case RadioTapType.Fhss:
                {
                    return new FhssRadioTapField(br);
                }
                case RadioTapType.LockQuality:
                {
                    return new LockQualityRadioTapField(br);
                }
                case RadioTapType.TxAttenuation:
                {
                    return new TxAttenuationRadioTapField(br);
                }
                case RadioTapType.DbTxAttenuation:
                {
                    return new DbTxAttenuationRadioTapField(br);
                }
                case RadioTapType.DbmTxPower:
                {
                    return new DbmTxPowerRadioTapField(br);
                }
                case RadioTapType.Tsft:
                {
                    return new TsftRadioTapField(br);
                }
                case RadioTapType.RxFlags:
                {
                    return new RxFlagsRadioTapField(br);
                }
                case RadioTapType.Mcs:
                {
                    return new McsRadioTapField(br);
                }
                case RadioTapType.HighEfficiency:
                {
                    return new HighEfficiencyRadioTapField(br);
                }
                case RadioTapType.VeryHighThroughput:
                {
                    return new VeryHighThroughputRadioTapField(br);
                }
                default:
                {
                    //the RadioTap fields are extendable so there may be some we dont know about
                    return null;
                }
            }
        }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public abstract void CopyTo(byte[] dest, int offset);
    }