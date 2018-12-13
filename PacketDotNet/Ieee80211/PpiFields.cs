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
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Abstract class for all PPI fields
    /// </summary>
    public abstract class PpiFields
    {
        #region Public Methods

        /// <summary>
        /// Parse a PPI indicated by type, from a given BinaryReader
        /// </summary>
        /// <param name="fieldType">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="br">
        /// A <see cref="BinaryReader" />
        /// </param>
        /// <param name="fieldLength">
        /// The maximum number of bytes that the field to be parsed can encompass.
        /// </param>
        /// <returns>
        /// A <see cref="PpiFields" />
        /// </returns>
        public static PpiFields Parse(Int32 fieldType, BinaryReader br, UInt16 fieldLength)
        {
            var type = (PpiFieldType) fieldType;
            switch (type)
            {
                case PpiFieldType.PpiReserved0:
                    return new PpiUnknown(fieldType, br, fieldLength);
                case PpiFieldType.PpiReserved1:
                    return new PpiUnknown(fieldType, br, fieldLength);
                case PpiFieldType.PpiCommon:
                    return new PpiCommon(br);
                case PpiFieldType.PpiMacExtensions:
                    return new PpiMacExtensions(br);
                case PpiFieldType.PpiMacPhy:
                    return new PpiMacPhy(br);
                case PpiFieldType.PpiSpectrum:
                    return new PpiSpectrum(br);
                case PpiFieldType.PpiProcessInfo:
                    return new PpiProcessInfo(br);
                case PpiFieldType.PpiCaptureInfo:
                    return new PpiCaptureInfo();
                case PpiFieldType.PpiAggregation:
                    return new PpiAggregation(br);
                case PpiFieldType.Ppi802_3:
                    return new Ppi8023(br);
                default:
                    return new PpiUnknown(fieldType, br, fieldLength);
            }
        }

        #endregion Public Methods


        #region Properties

        /// <summary>Type of the field</summary>
        public abstract PpiFieldType FieldType { get; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public abstract Int32 Length { get; }

        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public abstract Byte[] Bytes { get; }

        #endregion Properties
    }
}