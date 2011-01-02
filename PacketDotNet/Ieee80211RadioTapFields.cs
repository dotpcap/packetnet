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
using System.Collections.Generic;
using System.IO;

namespace PacketDotNet
{
    /// <summary>
    /// Channel field
    /// </summary>
    public class ChannelRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_CHANNEL; } }

        /// <summary>
        /// Frequency in MHz
        /// </summary>
        public UInt16 FrequencyMHz { get; set; }

        /// <summary>
        /// Channel number derived from frequency
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Channel flags
        /// </summary>
        public Ieee80211RadioTapChannelFlags Flags;

        /// <summary>
        /// Convert a frequency to a channel
        /// </summary>
        /// <param name="frequencyMHz">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static int ChannelFromFrequencyMHz(int frequencyMHz)
        {
            switch (frequencyMHz)
            {
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
                default:
                    throw new System.NotImplementedException("Unknown frequencyMHz " + frequencyMHz);
            };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public ChannelRadioTapField(BinaryReader br)
        {
            FrequencyMHz = br.ReadUInt16();
            Channel = ChannelFromFrequencyMHz(FrequencyMHz);
            Flags = (Ieee80211RadioTapChannelFlags)br.ReadUInt16();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrequencyMHz {0}, Channel {1}, Flags {2}",
                                 FrequencyMHz, Channel, Flags);
        }
    }

    /// <summary>
    /// Fhss radio tap field
    /// </summary>
    public class FhssRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_FHSS; } }

        /// <summary>
        /// Hop set
        /// </summary>
        public byte ChannelHoppingSet { get; set; }

        /// <summary>
        /// Hop pattern
        /// </summary>
        public byte Pattern { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public FhssRadioTapField(BinaryReader br)
        {
            var u16 = br.ReadUInt16();

            ChannelHoppingSet = (byte)(u16 & 0xff);
            Pattern = (byte)((u16 >> 8) & 0xff);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("ChannelHoppingSet {0}, Pattern {1}",
                                 ChannelHoppingSet, Pattern);
        }
    }

    /// <summary>
    /// Radio tap flags
    /// </summary>
    public class FlagsRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_FLAGS; } }

        /// <summary>
        /// Flags set
        /// </summary>
        public Ieee80211RadioTapFlags Flags;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public FlagsRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            Flags = (Ieee80211RadioTapFlags)u8;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("Flags {0}", Flags);
        }
    }

    /// <summary>
    /// Rate field
    /// </summary>
    public class RateRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_RATE; } }

        /// <summary>
        /// Rate in Mbps
        /// </summary>
        public double RateMbps { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public RateRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            RateMbps = (0.5 * (u8 & 0x7f));
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("RateMbps {0}", RateMbps);
        }
    }

    /// <summary>
    /// Db antenna signal
    /// </summary>
    public class DbAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL; } }

        /// <summary>
        /// Signal strength in dB
        /// </summary>
        public byte SignalStrengthdB { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbAntennaSignalRadioTapField(BinaryReader br)
        {
            SignalStrengthdB = br.ReadByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("SignalStrengthdB {0}", SignalStrengthdB);
        }
    }

    /// <summary>
    /// Antenna noise in dB
    /// </summary>
    public class DbAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_ANTNOISE; } }

        /// <summary>
        /// Antenna noise in dB
        /// </summary>
        public byte AntennaNoisedB { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedB = br.ReadByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaNoisedB {0}", AntennaNoisedB);
        }
    }

    /// <summary>
    /// Antenna field
    /// </summary>
    public class AntennaRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_ANTENNA; } }

        /// <summary>
        /// Antenna number
        /// </summary>
        public byte Antenna { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public AntennaRadioTapField(BinaryReader br)
        {
            Antenna = br.ReadByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("Antenna {0}", Antenna);
        }
    }

    /// <summary>
    /// Antenna signal in dBm
    /// </summary>
    public class DbmAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL; } }

        /// <summary>
        /// Antenna signal in dBm
        /// </summary>
        public sbyte AntennaSignalDbm { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmAntennaSignalRadioTapField(BinaryReader br)
        {
            AntennaSignalDbm = br.ReadSByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaSignalDbm {0}", AntennaSignalDbm);
        }
    }

    /// <summary>
    /// Antenna noise in dBm
    /// </summary>
    public class DbmAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE; } }

        /// <summary>
        /// Antenna noise in dBm
        /// </summary>
        public sbyte AntennaNoisedBm { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmAntennaNoiseRadioTapField(BinaryReader br)
        {
            AntennaNoisedBm = br.ReadSByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaNoisedBm {0}", AntennaNoisedBm);
        }
    }

    /// <summary>
    /// Lock quality
    /// </summary>
    public class LockQualityRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY; } }

        /// <summary>
        /// Signal quality
        /// </summary>
        public UInt16 SignalQuality { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public LockQualityRadioTapField(BinaryReader br)
        {
            SignalQuality = br.ReadUInt16();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("SignalQuality {0}", SignalQuality);
        }
    }

    /// <summary>
    /// Tsft radio tap field
    /// </summary>
    public class TsftRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_TSFT; } }

        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public UInt64 TimestampUsec { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public TsftRadioTapField(BinaryReader br)
        {
            TimestampUsec = br.ReadUInt64();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TimestampUsec {0}", TimestampUsec);
        }
    }

    /// <summary>
    /// Fcs field
    /// </summary>
    public class FcsRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_FCS; } }

        /// <summary>
        /// Frame check sequence
        /// </summary>
        public UInt32 FrameCheckSequence { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public FcsRadioTapField(BinaryReader br)
        {
            FrameCheckSequence = (UInt32)System.Net.IPAddress.HostToNetworkOrder(br.ReadInt32());
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrameCheckSequence {0}", FrameCheckSequence);
        }
    }

    /// <summary>
    /// Transmit power expressed as unitless distance from max
    /// power set at factory calibration.  0 is max power.
    /// Monotonically nondecreasing with lower power levels.
    /// </summary>
    public class TxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_TX_ATTENUATION; } }

        /// <summary>
        /// Transmit power
        /// </summary>
        public int TxPower { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public TxAttenuationRadioTapField(BinaryReader br)
        {
            TxPower = -(int)br.ReadUInt16();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TxPower {0}", TxPower);
        }
    }

    /// <summary>
    /// Transmit power expressed as decibel distance from max power
    /// set at factory calibration.  0 is max power.  Monotonically
    /// nondecreasing with lower power levels.
    /// </summary>
    public class DbTxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_TX_ATTENUATION; } }

        /// <summary>
        /// Transmit power 
        /// </summary>
        public int TxPowerdB { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbTxAttenuationRadioTapField(BinaryReader br)
        {
            TxPowerdB = -(int)br.ReadByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TxPowerdB {0}", TxPowerdB);
        }
    }

    /// <summary>
    /// Transmit power expressed as dBm (decibels from a 1 milliwatt
    /// reference). This is the absolute power level measured at
    /// the antenna port.
    /// </summary>
    public class DbmTxPowerRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override Ieee80211RadioTapType FieldType { get { return Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_TX_POWER; } }

        /// <summary>
        /// Tx power in dBm
        /// </summary>
        public sbyte TxPowerdBm { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmTxPowerRadioTapField(BinaryReader br)
        {
            TxPowerdBm = br.ReadSByte();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TxPowerdBm {0}", TxPowerdBm);
        }
    }

    /// <summary>
    /// Abstract class for all radio tap fields
    /// </summary>
    public abstract class RadioTapField
    {
        /// <summary>Type of the field</summary>
        public abstract Ieee80211RadioTapType FieldType
        { get; }

        /// <summary>
        /// Parse a radio tap field, indicated by bitIndex, from a given BinaryReader
        /// </summary>
        /// <param name="bitIndex">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        /// <returns>
        /// A <see cref="RadioTapField"/>
        /// </returns>
        public static RadioTapField Parse(int bitIndex, BinaryReader br)
        {
            var Type = (Ieee80211RadioTapType)bitIndex;
            switch (Type)
            {
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_FLAGS:
                    return new FlagsRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_RATE:
                    return new RateRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_ANTSIGNAL:
                    return new DbAntennaSignalRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_ANTNOISE:
                    return new DbAntennaNoiseRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_ANTENNA:
                    return new AntennaRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_ANTSIGNAL:
                    return new DbmAntennaSignalRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_ANTNOISE:
                    return new DbmAntennaNoiseRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_CHANNEL:
                    return new ChannelRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_FHSS:
                    return new FhssRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_LOCK_QUALITY:
                    return new LockQualityRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_TX_ATTENUATION:
                    return new TxAttenuationRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DB_TX_ATTENUATION:
                    return new DbTxAttenuationRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_DBM_TX_POWER:
                    return new DbmTxPowerRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_TSFT:
                    return new TsftRadioTapField(br);
                case Ieee80211RadioTapType.IEEE80211_RADIOTAP_FCS:
                    return new FcsRadioTapField(br);
                default:
                    throw new System.NotImplementedException("Unknown bitIndex of " + bitIndex);
            }
        }
    };
}
