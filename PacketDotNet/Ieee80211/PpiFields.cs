#region Header

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
 * Copyright 2011 David Thedens <dthedens@metageek.net>
 */

#endregion Header
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class Ppi802_3 : PpiField
        {
            [Flags]
            public enum StandardFlags : uint
            {
                FcsPresent = 1
            }
            
            [Flags]
            public enum ErrorFlags : uint
            {
                InvalidFcs = 1,
                SequenceError = 2,
                SymbolError = 4,
                DataError = 8
            }
            
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.Ppi802_3; }
            }
            
            public override int Length { get {return 8; } }
            
            public StandardFlags Flags { get; set; }
            
            public ErrorFlags Errors { get; set; }
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write((uint)Flags);
                    writer.Write((uint)Errors);
                    return ms.ToArray();
                }
            }

        #endregion Properties

        #region Constructors

            public Ppi802_3(BinaryReader br)
            {
                Flags = (StandardFlags) br.ReadUInt32();
                Errors = (ErrorFlags) br.ReadUInt32();
            }
            
            public Ppi802_3(StandardFlags Flags, ErrorFlags Errors)
            {
                this.Flags = Flags;
                this.Errors = Errors;
            }
            
            public Ppi802_3()
            {
             
            }

        #endregion Constructors
        }

        public class PpiAggregation : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiAggregation;}
            }
            
            public override int Length { get {return 4; } }

            public uint InterfaceId { get; set; }
            
            public override byte[] Bytes
            {
                get
                {
                    return BitConverter.GetBytes(InterfaceId);
                }
            }
            
        #endregion Properties

        #region Constructors

            public PpiAggregation (BinaryReader br)
            {
                InterfaceId = br.ReadUInt32();
            }
   
            public PpiAggregation(uint InterfaceId)
            {
                this.InterfaceId = InterfaceId;
            }
            
            public PpiAggregation ()
            {

            }
            
        #endregion Constructors
        }

        public class PpiCaptureInfo : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiCaptureInfo;}
            }
            
            public override int Length { get { return 0; } }
            
            public override byte[] Bytes
            {
                get
                {
                    return new byte[0];
                }
            }

        #endregion Properties

        #region Constructors

            public PpiCaptureInfo (BinaryReader br)
            {
            }
            
            public PpiCaptureInfo()
            {
             
            }

        #endregion Constructors
        }

        public class PpiCommon : PpiField
        {
            [Flags]
            public enum CommonFlags : ushort
            {
                FcsIncludedInFrame = 0x1,
                TimerSynchFunctionInUse = 0x2,
                FailedFcsCheck = 0x4,
                PhysicalError = 0x8
            }
         
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiCommon;}
            }
            
            public override int Length { get { return 20; } }
            
            public RadioTapChannelFlags ChannelFlags
            {
                get;
                set;
            }
            
            public UInt16 ChannelFrequency
            {
                get;
                set;
            }

            public CommonFlags Flags
            {
                get;
                set;
            }

            public double Rate
            {
                get;
                set;
            }

            public UInt64 TSFTimer
            {
                get;
                set;
            }
            
            public Byte FhssHopset
            {
                get;
                set;
            }
            
            public Byte FhssPattern
            {
                get;
                set;
            }
   
            
            public SByte AntennaSignalPower
            {
                get;
                set;
            }
            
            public SByte AntennaSignalNoise
            {
                get;
                set;
            }
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    
                    writer.Write(TSFTimer);
                    writer.Write((ushort)Flags);
                    writer.Write((ushort)(Rate * 2));
                    writer.Write(ChannelFrequency);
                    writer.Write((ushort)ChannelFlags);
                    writer.Write(FhssHopset);
                    writer.Write(FhssPattern);
                    writer.Write(AntennaSignalPower);
                    writer.Write(AntennaSignalNoise);
                    
                    return ms.ToArray();
                }
            }
            
        #endregion Properties

        #region Constructors

            public PpiCommon (BinaryReader br)
            {
                TSFTimer = br.ReadUInt64 ();
                Flags = (CommonFlags)br.ReadUInt16 ();
                Rate = 0.5f * br.ReadUInt16 ();
                ChannelFrequency = br.ReadUInt16 ();
                ChannelFlags = (RadioTapChannelFlags) br.ReadUInt16 ();
                FhssHopset = br.ReadByte ();
                FhssPattern = br.ReadByte ();
                AntennaSignalPower = br.ReadSByte();
                AntennaSignalNoise = br.ReadSByte();
            }
   
            public PpiCommon ()
            {
                AntennaSignalPower = -128;
                AntennaSignalNoise = -128;
            }
            
        #endregion Constructors
        }
        

        public class PpiException : Exception
        {
        #region Constructors

            internal PpiException ()
            : base()
            {
            }

            internal PpiException (string msg)
            : base(msg)
            {
            }

        #endregion Constructors
        }

        /// <summary>
        /// Abstract class for all PPI fields
        /// </summary>
        public abstract class PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public abstract PpiFieldType FieldType
            {
                get;
            }
   
            public abstract int Length { get; }
            
            public abstract byte[] Bytes { get; }
            
        #endregion Properties

        #region Public Methods

            /// <summary>
            /// Parse a PPI indicated by type, from a given BinaryReader
            /// </summary>
            /// <param name="fieldType">
            /// A <see cref="System.Int32"/>
            /// </param>
            /// <param name="br">
            /// A <see cref="BinaryReader"/>
            /// </param>
            /// <returns>
            /// A <see cref="PpiField"/>
            /// </returns>
            public static PpiField Parse (int fieldType, BinaryReader br, ushort fieldLength)
            {
                var type = (PpiFieldType)fieldType;
                switch (type)
                {
                case PpiFieldType.PpiReserved0:
                    return new PpiUnknown (fieldType, br, fieldLength);
                case PpiFieldType.PpiReserved1:
                    return new PpiUnknown (fieldType, br, fieldLength);
                case PpiFieldType.PpiCommon:
                    return new PpiCommon (br);
                case PpiFieldType.PpiMacExtensions:
                    return new PpiMacExtensions (br);
                case PpiFieldType.PpiMacPhy:
                    return new PpiMacPhy (br);
                case PpiFieldType.PpiSpectrum:
                    return new PpiSpectrum (br);
                case PpiFieldType.PpiProcessInfo:
                    return new PpiProcessInfo (br);
                case PpiFieldType.PpiCaptureInfo:
                    return new PpiCaptureInfo (br);
                case PpiFieldType.PpiAggregation:
                    return new PpiAggregation (br);
                case PpiFieldType.Ppi802_3:
                    return new Ppi802_3 (br);
                default:
                    return new PpiUnknown (fieldType, br, fieldLength);
                }
            }

        #endregion Public Methods
        }

        public class PpiMacExtensions : PpiField
        {
            public enum ExtensionFlags : uint
            {
                GreenField = 0x1,
                HtIndicator = 0x2,
                RxSgi = 0x4,
                DuplicateRx = 0x8,
                Aggregate = 0x10,
                MoreAggregates = 0x20,
                AggregateDelimiterCrc = 0x40
            }
            
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiMacExtensions;}
            }
   
            public override int Length { get { return 12; } }
            
            public ExtensionFlags Flags { get; set; }
            
            public uint AMpdu { get; set; }
            
            public byte DelimiterCount { get; set; }
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    
                    writer.Write((uint) Flags);
                    writer.Write(AMpdu);
                    writer.Write(DelimiterCount);
                    writer.Write(new byte[3]);
                    
                    return ms.ToArray();
                }
            }
            
        #endregion Properties

        #region Constructors

            public PpiMacExtensions (BinaryReader br)
            {
                Flags = (ExtensionFlags) br.ReadUInt32();
                AMpdu = br.ReadUInt32();
                DelimiterCount = br.ReadByte();
            }
            
            public PpiMacExtensions ()
            {
             
            }

        #endregion Constructors
        }

        public class PpiMacPhy : PpiField
        {
            public enum ExtensionFlags : uint
            {
                GreenField = 0x1,
                HtIndicator = 0x2,
                RxSgi = 0x4,
                DuplicateRx = 0x8,
                Aggregate = 0x10,
                MoreAggregates = 0x20,
                AggregateDelimiterCrc = 0x40
            }
            
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiMacPhy;}
            }
            
            public override int Length { get { return 48; } }
            
            public ExtensionFlags Flags { get; set; }
            
            public uint AMpdu { get; set; }
            
            public byte DelimiterCount { get; set; }
            
            public byte ModulationCodingScheme { get; set; }
            
            public byte SpatialStreamCount { get; set; }
            
            public byte RssiCombined { get; set; }
            
            public byte RssiAntenna0Control { get; set; }
            
            public byte RssiAntenna1Control { get; set; }
            
            public byte RssiAntenna2Control { get; set; }
            
            public byte RssiAntenna3Control { get; set; }
            
            public byte RssiAntenna0Ext { get; set; }
            
            public byte RssiAntenna1Ext { get; set; }
            
            public byte RssiAntenna2Ext { get; set; }
            
            public byte RssiAntenna3Ext { get; set; }
            
            public ushort ExtensionChannelFrequency { get; set; }
            
            public RadioTapChannelFlags ChannelFlags { get; set; }
            
            public byte DBmAntenna0SignalPower { get; set; }
            
            public byte DBmAntenna0SignalNoise { get; set; }
            
            public byte DBmAntenna1SignalPower { get; set; }
            
            public byte DBmAntenna1SignalNoise { get; set; }
            
            public byte DBmAntenna2SignalPower { get; set; }
            
            public byte DBmAntenna2SignalNoise { get; set; }
            
            public byte DBmAntenna3SignalPower { get; set; }
            
            public byte DBmAntenna3SignalNoise { get; set; }
            
            public uint ErrorVectorMagnitude0 { get; set; }
            
            public uint ErrorVectorMagnitude1 { get; set; }
            
            public uint ErrorVectorMagnitude2 { get; set; }
            
            public uint ErrorVectorMagnitude3 { get; set; }
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    
                    writer.Write(AMpdu);
                    writer.Write(DelimiterCount);
                    writer.Write(ModulationCodingScheme);
                    writer.Write(SpatialStreamCount);
                    writer.Write(RssiCombined);
                    writer.Write(RssiAntenna0Control);
                    writer.Write(RssiAntenna1Control);
                    writer.Write(RssiAntenna2Control);
                    writer.Write(RssiAntenna3Control);
                    writer.Write(RssiAntenna0Ext);
                    writer.Write(RssiAntenna1Ext);
                    writer.Write(RssiAntenna2Ext);
                    writer.Write(RssiAntenna3Ext);
                    writer.Write(ExtensionChannelFrequency);
                    writer.Write((ushort)ChannelFlags);
                    writer.Write(DBmAntenna0SignalPower);
                    writer.Write(DBmAntenna0SignalNoise);
                    writer.Write(DBmAntenna1SignalPower);
                    writer.Write(DBmAntenna1SignalNoise);
                    writer.Write(DBmAntenna2SignalPower);
                    writer.Write(DBmAntenna2SignalNoise);
                    writer.Write(DBmAntenna3SignalPower);
                    writer.Write(DBmAntenna3SignalNoise);
                    writer.Write(ErrorVectorMagnitude0);
                    writer.Write(ErrorVectorMagnitude1);
                    writer.Write(ErrorVectorMagnitude2);
                    writer.Write(ErrorVectorMagnitude3);
                    
                    return ms.ToArray();
                }
            }
            
        #endregion Properties

        #region Constructors

            public PpiMacPhy (BinaryReader br)
            {
                AMpdu = br.ReadUInt32();
                DelimiterCount = br.ReadByte();
                ModulationCodingScheme = br.ReadByte();
                SpatialStreamCount = br.ReadByte();
                RssiCombined = br.ReadByte();
                RssiAntenna0Control = br.ReadByte();
                RssiAntenna1Control = br.ReadByte();
                RssiAntenna2Control = br.ReadByte();
                RssiAntenna3Control = br.ReadByte();
                RssiAntenna0Ext = br.ReadByte();
                RssiAntenna1Ext = br.ReadByte();
                RssiAntenna2Ext = br.ReadByte();
                RssiAntenna3Ext = br.ReadByte();
                ExtensionChannelFrequency = br.ReadUInt16();
                ChannelFlags = (RadioTapChannelFlags) br.ReadUInt16();
                DBmAntenna0SignalPower = br.ReadByte();
                DBmAntenna0SignalNoise = br.ReadByte();
                DBmAntenna1SignalPower = br.ReadByte();
                DBmAntenna1SignalNoise = br.ReadByte();
                DBmAntenna2SignalPower = br.ReadByte();
                DBmAntenna2SignalNoise = br.ReadByte();
                DBmAntenna3SignalPower = br.ReadByte();
                DBmAntenna3SignalNoise = br.ReadByte();
                ErrorVectorMagnitude0 = br.ReadUInt32();
                ErrorVectorMagnitude1 = br.ReadUInt32();
                ErrorVectorMagnitude2 = br.ReadUInt32();
                ErrorVectorMagnitude3 = br.ReadUInt32();
            }
            
            public PpiMacPhy ()
            {
                
            }

        #endregion Constructors
        }

        public class PpiProcessInfo : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiProcessInfo;}
            }
            
            public override int Length
            {
                get
                {
                    var processLength = (String.IsNullOrEmpty(ProcessPath)) ? 0 : Encoding.UTF8.GetByteCount(ProcessPath);
                    var userLength = (String.IsNullOrEmpty(UserName)) ? 0 : Encoding.UTF8.GetByteCount(UserName);
                    var groupLength = (String.IsNullOrEmpty(GroupName)) ? 0 : Encoding.UTF8.GetByteCount(GroupName);
                    return 19 + processLength + userLength + groupLength;
                }
            }
            
            public uint ProcessId { get; set; }
            
            public uint ThreadId { get; set; }
                        
            public String ProcessPath { get; set; }
            
            public uint UserId { get; set; }
            
            public String UserName { get; set; }
            
            public uint GroupId { get; set; }
            
            public String GroupName { get; set; }
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    
                    writer.Write(ProcessId);
                    writer.Write(ThreadId);
                    
                    var pathBytes = Encoding.UTF8.GetBytes(ProcessPath ?? String.Empty);
                    writer.Write((byte)pathBytes.Length);
                    writer.Write(pathBytes);
                    
                    writer.Write(UserId);
                    
                    var userBytes = Encoding.UTF8.GetBytes(UserName ?? String.Empty);
                    writer.Write((byte)userBytes.Length);
                    writer.Write(userBytes);
                    
                    writer.Write(GroupId);
                    
                    var groupBytes = Encoding.UTF8.GetBytes(GroupName ?? String.Empty);
                    writer.Write((byte)groupBytes.Length);
                    writer.Write(groupBytes);
                        
                    return ms.ToArray();
                }
            }

        #endregion Properties

        #region Constructors

            public PpiProcessInfo (BinaryReader br)
            {
                
                ProcessId = br.ReadUInt32();
                ThreadId = br.ReadUInt32();
                
                var pathLength = br.ReadByte();
                ProcessPath = Encoding.UTF8.GetString(br.ReadBytes(pathLength));
                
                UserId = br.ReadUInt32();
                
                var userLength = br.ReadByte();
                UserName = Encoding.UTF8.GetString(br.ReadBytes(userLength));
                
                GroupId = br.ReadUInt32();
                
                var groupLength = br.ReadByte();
                GroupName = Encoding.UTF8.GetString(br.ReadBytes(groupLength));
            }
            
            public PpiProcessInfo ()
            {
                
            }

        #endregion Constructors
        }

        public class PpiUnknown : PpiField
        {
            private PpiFieldType fieldType;
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType{ get { return fieldType; } } 
            
            public override int Length { get { return Bytes.Length; } }
   
            public override byte[] Bytes { get { return UnknownBytes; } }
            
            public byte[] UnknownBytes { get; set; }
            
        #endregion Properties

        #region Constructors

            public PpiUnknown (int typeNumber, BinaryReader br, int length)
            {
                fieldType = (PpiFieldType) typeNumber;
                UnknownBytes = br.ReadBytes(length);
            }
   
            public PpiUnknown (int typeNumber)
            {
                fieldType = (PpiFieldType)typeNumber;
            }
            
            public PpiUnknown (int typeNumber, byte[] UnknownBytes)
            {
                fieldType = (PpiFieldType)typeNumber;
                this.UnknownBytes = UnknownBytes;
            }
            
        #endregion Constructors
        }

        public class PpiSpectrum : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiSpectrum;}
            }
            
            public override int Length { get { return 20 + SamplesData.Length; } }
   
            public uint StartingFrequency { get; set; }
            
            public uint Resolution { get; set; }
            
            public uint AmplitudeOffset { get; set; }
            
            public uint AmplitudeResolution { get; set; }
            
            public ushort MaximumRssi { get; set; }
            
            public byte[] SamplesData { get; set;}
            
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    
                    writer.Write(StartingFrequency);
                    writer.Write(Resolution);
                    writer.Write(AmplitudeOffset);
                    writer.Write(AmplitudeResolution);
                    writer.Write(MaximumRssi);
                    writer.Write((ushort) SamplesData.Length);
                    writer.Write(SamplesData);
                    
                    return ms.ToArray();
                }    
            }
            
        #endregion Properties

        #region Constructors

            public PpiSpectrum (BinaryReader br)
            {
                StartingFrequency = br.ReadUInt32();
                Resolution = br.ReadUInt32();
                AmplitudeOffset = br.ReadUInt32();
                AmplitudeResolution = br.ReadUInt32();
                MaximumRssi = br.ReadUInt16();
                var samplesLength = br.ReadUInt16();
                SamplesData = br.ReadBytes(samplesLength);
            }
            
            public PpiSpectrum ()
            {
             
            }
            
        #endregion Constructors
        }
    }
}