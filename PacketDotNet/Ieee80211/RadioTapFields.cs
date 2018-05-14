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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Channel field
    /// </summary>
    public class ChannelRadioTapField : RadioTapField
    {
        /// <summary>
        /// Channel flags
        /// </summary>
        public RadioTapChannelFlags Flags;

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
        public ChannelRadioTapField(UInt16 frequencyMhz, RadioTapChannelFlags flags)
        {
            FrequencyMHz = frequencyMhz;
            Channel = ChannelFromFrequencyMHz(FrequencyMHz);
            Flags = flags;
        }

        /// <summary>
        /// Channel number derived from frequency
        /// </summary>
        public Int32 Channel { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Channel;

        /// <summary>
        /// Frequency in MHz
        /// </summary>
        public UInt16 FrequencyMHz { get; set; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 4;

        /// <summary>
        /// Convert a frequency to a channel
        /// </summary>
        /// <remarks>
        /// There is some overlap between the 802.11b/g channel numbers and the 802.11a channel numbers. This means that while a particular frequncy will only
        /// ever map to single channel number the same channel number may be returned for more than one frequency. At present this affects channel numbers 8 and 12.
        /// </remarks>
        /// <param name="frequencyMHz">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32" />
        /// </returns>
        public static Int32 ChannelFromFrequencyMHz(Int32 frequencyMHz)
        {
            switch (frequencyMHz)
            {
                //802.11 bg channel numbers
                case 2412:
                    return 1;
                case 2417:
                    return 2;
                case 2422:
                    return 3;
                case 2427:
                    return 4;
                case 2432:
                    return 5;
                case 2437:
                    return 6;
                case 2442:
                    return 7;
                case 2447:
                    return 8;
                case 2452:
                    return 9;
                case 2457:
                    return 10;
                case 2462:
                    return 11;
                case 2467:
                    return 12;
                case 2472:
                    return 13;
                case 2484:
                    return 14;
                //802.11 a channel numbers
                case 4920:
                    return 240;
                case 4940:
                    return 244;
                case 4960:
                    return 248;
                case 4980:
                    return 252;
                case 5040:
                    return 8;
                case 5060:
                    return 12;
                case 5080:
                    return 16;
                case 5170:
                    return 34;
                case 5180:
                    return 36;
                case 5190:
                    return 38;
                case 5200:
                    return 40;
                case 5210:
                    return 42;
                case 5220:
                    return 44;
                case 5230:
                    return 46;
                case 5240:
                    return 48;
                case 5260:
                    return 52;
                case 5280:
                    return 56;
                case 5300:
                    return 60;
                case 5320:
                    return 64;
                case 5500:
                    return 100;
                case 5520:
                    return 104;
                case 5540:
                    return 108;
                case 5560:
                    return 112;
                case 5580:
                    return 116;
                case 5600:
                    return 120;
                case 5620:
                    return 124;
                case 5640:
                    return 128;
                case 5660:
                    return 132;
                case 5680:
                    return 136;
                case 5700:
                    return 140;
                case 5745:
                    return 149;
                case 5765:
                    return 153;
                case 5785:
                    return 157;
                case 5805:
                    return 161;
                case 5825:
                    return 165;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            EndianBitConverter.Little.CopyBytes(FrequencyMHz, dest, offset);
            EndianBitConverter.Little.CopyBytes((UInt16) Flags, dest, offset + 2);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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

            ChannelHoppingSet = (Byte) (u16 & 0xff);
            Pattern = (Byte) ((u16 >> 8) & 0xff);
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
        public FhssRadioTapField(Byte channelHoppingSet, Byte pattern)
        {
            ChannelHoppingSet = channelHoppingSet;
            Pattern = pattern;
        }

        /// <summary>
        /// Hop set
        /// </summary>
        public Byte ChannelHoppingSet { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Fhss;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 2;

        /// <summary>
        /// Hop pattern
        /// </summary>
        public Byte Pattern { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
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
        public override String ToString()
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
        public override UInt16 Length => 1;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) Flags;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public RateRadioTapField(Double rateMbps)
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
        public override UInt16 Length => 1;

        /// <summary>
        /// Rate in Mbps
        /// </summary>
        public Double RateMbps { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) (RateMbps / 0.5);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbAntennaSignalRadioTapField(Byte signalStrengthdB)
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
        public override UInt16 Length => 1;

        /// <summary>
        /// Signal strength in dB
        /// </summary>
        public Byte SignalStrengthdB { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = SignalStrengthdB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbAntennaNoiseRadioTapField(Byte antennaNoisedB)
        {
            AntennaNoisedB = antennaNoisedB;
        }

        /// <summary>
        /// Antenna noise in dB
        /// </summary>
        public Byte AntennaNoisedB { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbAntennaNoise;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = AntennaNoisedB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public AntennaRadioTapField(Byte antenna)
        {
            Antenna = antenna;
        }

        /// <summary>
        /// Antenna number
        /// </summary>
        public Byte Antenna { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Antenna;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = Antenna;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbmAntennaSignalRadioTapField(SByte antennaSignalDbm)
        {
            AntennaSignalDbm = antennaSignalDbm;
        }

        /// <summary>
        /// Antenna signal in dBm
        /// </summary>
        public SByte AntennaSignalDbm { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaSignal;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) AntennaSignalDbm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbmAntennaNoiseRadioTapField(SByte antennaNoisedBm)
        {
            AntennaNoisedBm = antennaNoisedBm;
        }

        /// <summary>
        /// Antenna noise in dBm
        /// </summary>
        public SByte AntennaNoisedBm { get; set; }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaNoise;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) AntennaNoisedBm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public LockQualityRadioTapField(UInt16 signalQuality)
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
        public override UInt16 Length => 2;

        /// <summary>
        /// Signal quality
        /// </summary>
        public UInt16 SignalQuality { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            EndianBitConverter.Little.CopyBytes(SignalQuality, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public TsftRadioTapField(UInt64 timestampUsec)
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
        public override UInt16 Length => 8;

        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public UInt64 TimestampUsec { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            EndianBitConverter.Little.CopyBytes(TimestampUsec, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public RxFlagsRadioTapField(Boolean plcpCrcCheckFailed)
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
        public override UInt16 Length => 2;

        /// <summary>
        /// Gets or sets a value indicating whether the frame failed the PLCP CRC check.
        /// </summary>
        /// <value>
        /// <c>true</c> if the PLCP CRC check failed; otherwise, <c>false</c>.
        /// </value>
        public Boolean PlcpCrcCheckFailed { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            var flags = (UInt16) (PlcpCrcCheckFailed ? 0x2 : 0x0);
            EndianBitConverter.Little.CopyBytes(flags, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public TxAttenuationRadioTapField(Int32 txPower)
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
        public override UInt16 Length => 2;

        /// <summary>
        /// Transmit power
        /// </summary>
        public Int32 TxPower { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            var absValue = (UInt16) Math.Abs(TxPower);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbTxAttenuationRadioTapField(Int32 txPowerdB)
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
        public override UInt16 Length => 2;

        /// <summary>
        /// Transmit power
        /// </summary>
        public Int32 TxPowerdB { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            var absValue = (UInt16) Math.Abs(TxPowerdB);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public DbmTxPowerRadioTapField(SByte txPowerdBm)
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
        public override UInt16 Length => 1;

        /// <summary>
        /// Tx power in dBm
        /// </summary>
        public SByte TxPowerdBm { get; set; }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) TxPowerdBm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
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
        public abstract UInt16 Length { get; }

        /// <summary>
        /// Parse a radio tap field, indicated by bitIndex, from a given BinaryReader
        /// </summary>
        /// <param name="bitIndex">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        /// <returns>
        /// A <see cref="RadioTapField" />
        /// </returns>
        public static RadioTapField Parse(Int32 bitIndex, BinaryReader br)
        {
            var type = (RadioTapType) bitIndex;
            switch (type)
            {
                case RadioTapType.Flags:
                    return new FlagsRadioTapField(br);
                case RadioTapType.Rate:
                    return new RateRadioTapField(br);
                case RadioTapType.DbAntennaSignal:
                    return new DbAntennaSignalRadioTapField(br);
                case RadioTapType.DbAntennaNoise:
                    return new DbAntennaNoiseRadioTapField(br);
                case RadioTapType.Antenna:
                    return new AntennaRadioTapField(br);
                case RadioTapType.DbmAntennaSignal:
                    return new DbmAntennaSignalRadioTapField(br);
                case RadioTapType.DbmAntennaNoise:
                    return new DbmAntennaNoiseRadioTapField(br);
                case RadioTapType.Channel:
                    return new ChannelRadioTapField(br);
                case RadioTapType.Fhss:
                    return new FhssRadioTapField(br);
                case RadioTapType.LockQuality:
                    return new LockQualityRadioTapField(br);
                case RadioTapType.TxAttenuation:
                    return new TxAttenuationRadioTapField(br);
                case RadioTapType.DbTxAttenuation:
                    return new DbTxAttenuationRadioTapField(br);
                case RadioTapType.DbmTxPower:
                    return new DbmTxPowerRadioTapField(br);
                case RadioTapType.Tsft:
                    return new TsftRadioTapField(br);
                case RadioTapType.RxFlags:
                    return new RxFlagsRadioTapField(br);
                default:
                    //the RadioTap fields are extendable so there may be some we dont know about
                    return null;
            }
        }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public abstract void CopyTo(Byte[] dest, Int32 offset);
    }
}