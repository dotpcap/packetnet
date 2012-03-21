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
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Channel field
        /// </summary>
        public class ChannelRadioTapField : RadioTapField
        {
            /// <summary>Type of the field</summary>
            public override RadioTapType FieldType { get { return RadioTapType.Channel; } }
            
            public override ushort Length { get { return 4; } }

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
            public RadioTapChannelFlags Flags;

            /// <summary>
            /// Convert a frequency to a channel
            /// </summary>
            /// <remarks>There is some overlap between the 802.11b/g channel numbers and the 802.11a channel numbers. This means that while a particular frequncy will only
            /// ever map to single channel number the same channel number may be returned for more than one frequency. At present this affects channel numbers 8 and 12.</remarks>
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
                };
            }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                EndianBitConverter.Little.CopyBytes(FrequencyMHz, dest, offset);
                EndianBitConverter.Little.CopyBytes((UInt16)Flags, dest, offset + 2);
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
                Flags = (RadioTapChannelFlags)br.ReadUInt16();
            }
            
            public ChannelRadioTapField()
            {
             
            }
            
            public ChannelRadioTapField(UInt16 FrequencyMhz, RadioTapChannelFlags Flags)
            {
                this.FrequencyMHz = FrequencyMHz;
                this.Channel = ChannelFromFrequencyMHz(FrequencyMHz);
                this.Flags = Flags;
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
            public override RadioTapType FieldType { get { return RadioTapType.Fhss; } }
   
            public override ushort Length { get { return 2; } }
            
            /// <summary>
            /// Hop set
            /// </summary>
            public byte ChannelHoppingSet { get; set; }

            /// <summary>
            /// Hop pattern
            /// </summary>
            public byte Pattern { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = ChannelHoppingSet;
                dest[offset + 1] = Pattern;
            }

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
   
            public FhssRadioTapField()
            {
             
            }
            
            public FhssRadioTapField(byte ChannelHoppingSet, byte Pattern)
            {
                this.ChannelHoppingSet = ChannelHoppingSet;
                this.Pattern = Pattern;
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
            public override RadioTapType FieldType { get { return RadioTapType.Flags; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Flags set
            /// </summary>
            public RadioTapFlags Flags;
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = (byte) Flags;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="br">
            /// A <see cref="BinaryReader"/>
            /// </param>
            public FlagsRadioTapField(BinaryReader br)
            {
                var u8 = br.ReadByte();
                Flags = (RadioTapFlags)u8;
            }
   
            public FlagsRadioTapField()
            {
             
            }
            
            public FlagsRadioTapField(RadioTapFlags Flags)
            {
                this.Flags = Flags;
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
            public override RadioTapType FieldType { get { return RadioTapType.Rate; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Rate in Mbps
            /// </summary>
            public double RateMbps { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = (byte) (RateMbps / 0.5);
            }

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
            
            public RateRadioTapField ()
            {
             
            }
            
            public RateRadioTapField(double RateMbps)
            {
                this.RateMbps = RateMbps;             
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
            public override RadioTapType FieldType { get { return RadioTapType.DbAntennaSignal; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Signal strength in dB
            /// </summary>
            public byte SignalStrengthdB { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = SignalStrengthdB;
            }

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
            
            public DbAntennaSignalRadioTapField()
            {
             
            }
            
            public DbAntennaSignalRadioTapField (byte SignalStrengthdB)
            {
                this.SignalStrengthdB = SignalStrengthdB;
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
            public override RadioTapType FieldType { get { return RadioTapType.DbAntennaNoise; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Antenna noise in dB
            /// </summary>
            public byte AntennaNoisedB { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = AntennaNoisedB;
            }

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
            
            public DbAntennaNoiseRadioTapField()
            {
             
            }
            
            public DbAntennaNoiseRadioTapField(byte AntennaNoisedB)
            {
                this.AntennaNoisedB = AntennaNoisedB;
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
            public override RadioTapType FieldType { get { return RadioTapType.Antenna; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Antenna number
            /// </summary>
            public byte Antenna { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = Antenna;
            }

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
   
            public AntennaRadioTapField()
            {
             
            }
            
            public AntennaRadioTapField (byte Antenna)
            {
                this.Antenna = Antenna;
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
            public override RadioTapType FieldType { get { return RadioTapType.DbmAntennaSignal; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Antenna signal in dBm
            /// </summary>
            public sbyte AntennaSignalDbm { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = (byte)AntennaSignalDbm;
            }

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
            
            public DbmAntennaSignalRadioTapField()
            {
             
            }
            
            public DbmAntennaSignalRadioTapField (sbyte AntennaSignalDbm)
            {
                this.AntennaSignalDbm = AntennaSignalDbm;
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
            public override RadioTapType FieldType { get { return RadioTapType.DbmAntennaNoise; } }
   
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Antenna noise in dBm
            /// </summary>
            public sbyte AntennaNoisedBm { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = (byte)AntennaNoisedBm;
            }

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
            
            public DbmAntennaNoiseRadioTapField()
            {
             
            }
   
            public DbmAntennaNoiseRadioTapField (sbyte AntennaNoisedBm)
            {
                this.AntennaNoisedBm = AntennaNoisedBm;
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
            public override RadioTapType FieldType { get { return RadioTapType.LockQuality; } }
   
            public override ushort Length { get { return 2; } }
            
            /// <summary>
            /// Signal quality
            /// </summary>
            public UInt16 SignalQuality { get; set; }
   
            public override void CopyTo(byte[] dest, int offset)
            {
                EndianBitConverter.Little.CopyBytes(SignalQuality, dest, offset);
            }
            
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
            
            public LockQualityRadioTapField()
            {
             
            }
            
            public LockQualityRadioTapField(UInt16 SignalQuality)
            {
                this.SignalQuality = SignalQuality;
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
            public override RadioTapType FieldType { get { return RadioTapType.Tsft; } }
   
            public override ushort Length { get { return 8; } }
            
            /// <summary>
            /// Timestamp in microseconds
            /// </summary>
            public UInt64 TimestampUsec { get; set; }
            
            public override void CopyTo(byte[] dest, int offset)
            {
                EndianBitConverter.Little.CopyBytes(TimestampUsec, dest, offset);
            }

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
            
            public TsftRadioTapField()
            {
             
            }
            
            public TsftRadioTapField(UInt64 TimestampUsec)
            {
                this.TimestampUsec = TimestampUsec;
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


        public class RxFlagsRadioTapField : RadioTapField
        {
            /// <summary>Type of the field</summary>
            public override RadioTapType FieldType { get { return RadioTapType.RxFlags; } }
            
            public override ushort Length { get { return 2; } }
            
            public bool PlcpCrcCheckFailed {get; set;}
            
            public override void CopyTo(byte[] dest, int offset)
            {
                UInt16 flags = (UInt16)((PlcpCrcCheckFailed) ? 0x2 : 0x0);
                EndianBitConverter.Little.CopyBytes(flags, dest, offset);
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="br">
            /// A <see cref="BinaryReader"/>
            /// </param>
            public RxFlagsRadioTapField(BinaryReader br)
            {
                UInt16 flags = br.ReadUInt16();
                Console.WriteLine("RxFlagsRadioTapField {0}", flags);
                PlcpCrcCheckFailed = ((flags & 0x2) == 0x2);
            }
            
            public RxFlagsRadioTapField()
            {
             
            }
            
            public RxFlagsRadioTapField(bool PlcpCrcCheckFailed)
            {
                this.PlcpCrcCheckFailed = PlcpCrcCheckFailed;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("PlcpCrcCheckFailed {0}", PlcpCrcCheckFailed);
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
            public override RadioTapType FieldType { get { return RadioTapType.TxAttenuation; } }
   
            public override ushort Length { get { return 2; } }
            
            /// <summary>
            /// Transmit power
            /// </summary>
            public int TxPower { get; set; }
   
            public override void CopyTo(byte[] dest, int offset)
            {
                UInt16 absValue = (UInt16) Math.Abs(TxPower);
                EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
            }
            
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
            
            public TxAttenuationRadioTapField()
            {
             
            }
            
            public TxAttenuationRadioTapField (int TxPower)
            {
                this.TxPower = TxPower;
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
            public override RadioTapType FieldType { get { return RadioTapType.DbTxAttenuation; } }
   
            public override ushort Length { get { return 2; } }
            
            /// <summary>
            /// Transmit power 
            /// </summary>
            public int TxPowerdB { get; set; }
   
            public override void CopyTo(byte[] dest, int offset)
            {
                UInt16 absValue = (UInt16) Math.Abs(TxPowerdB);
                EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
            }
            
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="br">
            /// A <see cref="BinaryReader"/>
            /// </param>
            public DbTxAttenuationRadioTapField(BinaryReader br)
            {
                TxPowerdB = -(int)br.ReadUInt16();
            }
            
            public DbTxAttenuationRadioTapField()
            {
             
            }
            
            public DbTxAttenuationRadioTapField(int TxPowerdB)
            {
                this.TxPowerdB = TxPowerdB;
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
            public override RadioTapType FieldType { get { return RadioTapType.DbmTxPower; } }
            
            public override ushort Length { get { return 1; } }
            
            /// <summary>
            /// Tx power in dBm
            /// </summary>
            public sbyte TxPowerdBm { get; set; }
   
            public override void CopyTo(byte[] dest, int offset)
            {
                dest[offset] = (byte)TxPowerdBm;
            }
            
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
            
            public DbmTxPowerRadioTapField()
            {
             
            }
            
            public DbmTxPowerRadioTapField(sbyte TxPowerdBm)
            {
                this.TxPowerdBm = TxPowerdBm;
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
            public abstract RadioTapType FieldType
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
                var Type = (RadioTapType)bitIndex;
                switch (Type)
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
            
            public abstract ushort Length {get;}
            
            public abstract void CopyTo(byte[] dest, int offset);
        }; 
    }
}
