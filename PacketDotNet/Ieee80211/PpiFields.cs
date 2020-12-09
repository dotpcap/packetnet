/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2011 David Thedens <dthedens@metageek.net>
 */

using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Abstract class for all PPI fields
    /// </summary>
    public abstract class PpiFields
    {
        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public abstract byte[] Bytes { get; }

        /// <summary>Type of the field</summary>
        public abstract PpiFieldType FieldType { get; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public abstract int Length { get; }

        /// <summary>
        /// Parse a PPI indicated by type, from a given BinaryReader
        /// </summary>
        /// <param name="fieldType">
        /// A <see cref="int" />
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
        public static PpiFields Parse(int fieldType, BinaryReader br, ushort fieldLength)
        {
            var type = (PpiFieldType) fieldType;
            switch (type)
            {
                case PpiFieldType.PpiReserved0:
                {
                    return new PpiUnknown(fieldType, br, fieldLength);
                }
                case PpiFieldType.PpiReserved1:
                {
                    return new PpiUnknown(fieldType, br, fieldLength);
                }
                case PpiFieldType.PpiCommon:
                {
                    return new PpiCommon(br);
                }
                case PpiFieldType.PpiMacExtensions:
                {
                    return new PpiMacExtensions(br);
                }
                case PpiFieldType.PpiMacPhy:
                {
                    return new PpiMacPhy(br);
                }
                case PpiFieldType.PpiSpectrum:
                {
                    return new PpiSpectrum(br);
                }
                case PpiFieldType.PpiProcessInfo:
                {
                    return new PpiProcessInfo(br);
                }
                case PpiFieldType.PpiCaptureInfo:
                {
                    return new PpiCaptureInfo();
                }
                case PpiFieldType.PpiAggregation:
                {
                    return new PpiAggregation(br);
                }
                case PpiFieldType.Ppi802_3:
                {
                    return new Ppi8023(br);
                }
                default:
                {
                    return new PpiUnknown(fieldType, br, fieldLength);
                }
            }
        }
    }
}