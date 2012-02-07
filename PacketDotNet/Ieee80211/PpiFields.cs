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

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class Ppi802_3 : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.Ppi802_3; }
            }

        #endregion Properties

        #region Constructors

            public Ppi802_3 (BinaryReader br)
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

        #endregion Properties

        #region Constructors

            public PpiAggregation (BinaryReader br)
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

        #endregion Properties

        #region Constructors

            public PpiCaptureInfo (BinaryReader br)
            {
            }

        #endregion Constructors
        }

        public class PpiCommon : PpiField
        {
            [Flags]
            public enum CommonFlags
            {
                FcsIncludedInFrame = 0x1,
                TimerSynchFunctionInUse = 0x2,
                FailedFcsCheck = 0x4,
                PhysicalError = 0x8
            }
            
        #region Properties

            public UInt16 ChannelFlags
            {
                get;
                set;
            }

            public UInt16 ChannelFrequency
            {
                get;
                set;
            }

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiCommon;}
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
            
        #endregion Properties

        #region Constructors

            public PpiCommon (BinaryReader br)
            {
                TSFTimer = br.ReadUInt64 ();
                Flags = (CommonFlags)br.ReadUInt16 ();
                Rate = 0.5 * br.ReadUInt16 ();
                ChannelFrequency = br.ReadUInt16 ();
                ChannelFlags = br.ReadUInt16 ();
                FhssHopset = br.ReadByte ();
                FhssPattern = br.ReadByte ();
                AntennaSignalPower = br.ReadSByte();
                AntennaSignalNoise = br.ReadSByte();
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
            public static PpiField Parse (int fieldType, BinaryReader br)
            {
                var type = (PpiFieldType)fieldType;
                switch (type)
                {
                case PpiFieldType.PpiReserved0:
                    return new PpiReserved (br);
                case PpiFieldType.PpiReserved1:
                    return new PpiReserved (br);
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
                    return new PpiReserved (br);
                }
            }

        #endregion Public Methods
        }

        public class PpiMacExtensions : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiMacExtensions;}
            }

        #endregion Properties

        #region Constructors

            public PpiMacExtensions (BinaryReader br)
            {
            }

        #endregion Constructors
        }

        public class PpiMacPhy : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiMacPhy;}
            }

        #endregion Properties

        #region Constructors

            public PpiMacPhy (BinaryReader br)
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

        #endregion Properties

        #region Constructors

            public PpiProcessInfo (BinaryReader br)
            {
            }

        #endregion Constructors
        }

        public class PpiReserved : PpiField
        {
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType
            {
                get { return PpiFieldType.PpiReserved0;}
            }

        #endregion Properties

        #region Constructors

            public PpiReserved (BinaryReader br)
            {
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

        #endregion Properties

        #region Constructors

            public PpiSpectrum (BinaryReader br)
            {
            }

        #endregion Constructors
        }
    }
}