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
 * Copyright 2018 Steven Haufe<haufes@hotmail.com>
 */

using System;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

// ReSharper disable InconsistentNaming

namespace PacketDotNet
{
    [Serializable]
    public class IPv6ExtensionHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPv6ExtensionHeader" /> class.
        /// </summary>
        /// <param name="bas">The bas.</param>
        public IPv6ExtensionHeader(ByteArraySegment bas)
        {
            Header = bas;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public UInt16 Length => (ushort) ((PayloadLength + 1) * 8);

        /// <summary>
        /// Gets or sets the next header.
        /// </summary>
        public IPProtocolType NextHeader
        {
            get => (IPProtocolType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                    Header.Offset);

            set => EndianBitConverter.Big.CopyBytes((Byte) value,
                                                    Header.Bytes,
                                                    Header.Offset);
        }

        /// <summary>
        /// Gets the options and padding.
        /// </summary>
        public ByteArraySegment OptionsAndPadding => new ByteArraySegment(Header.Bytes, Header.Offset + 16, PayloadLength - 8);

        /// <summary>
        /// Gets or sets the length of the payload.
        /// </summary>
        /// <value>
        /// The length of the payload.
        /// </value>
        public UInt16 PayloadLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv6Fields.PayloadLengthPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + IPv6Fields.PayloadLengthPosition);
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        protected ByteArraySegment Header { get; set; }
    }
}