/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2018 Steven Haufe <haufes@hotmail.com>
 */

using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

// ReSharper disable InconsistentNaming

namespace PacketDotNet;

    public class IPv6FragmentationExtensionHeader : IPv6ExtensionHeader
    {
        /// <inheritdoc />
        public IPv6FragmentationExtensionHeader(ProtocolType header, ByteArraySegment byteArraySegment) : base(header, byteArraySegment)
        { }

        /// <summary>
        /// Gets or sets the offset, in 8-octet units, relative to the start of the fragmentable part of the original packet.
        /// </summary>
        public int FragmentOffset
        {
            get => (FragmentOffsetReservedMore >> 3) & 0x1FFF;
            set
            {
                // read the original value
                var @field = (ushort) FragmentOffsetReservedMore;

                // mask in the new field
                @field = (ushort) ((@field & 0x7) | ((ushort) value << 3) & 0xFFF8);

                // write the updated value back
                FragmentOffsetReservedMore = (short) @field;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating  where more fragments follow.
        /// </summary>
        public bool More
        {
            get => (FragmentOffsetReservedMore & 1) != 0;
            set
            {
                // read the original value
                var @field = (ushort) FragmentOffsetReservedMore;

                // mask in the new field
                @field = (ushort) ((@field & 0xFFFE) | (value ? 1 : 0) & 0x1);

                // write the updated value back
                FragmentOffsetReservedMore = (short) @field;
            }
        }

        private short FragmentOffsetReservedMore
        {
            get => EndianBitConverter.Big.ToInt16(ByteArraySegment.Bytes,
                                                  ByteArraySegment.Offset + IPv6Fields.FragmentOffsetPosition);
            set => EndianBitConverter.Big.CopyBytes(value, ByteArraySegment.Bytes, ByteArraySegment.Offset + IPv6Fields.FragmentOffsetPosition);
        }
    }

    public class IPv6ExtensionHeader
    {
        protected ByteArraySegment ByteArraySegment;
        private ByteArraySegment _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPv6ExtensionHeader" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public IPv6ExtensionHeader(ProtocolType header, ByteArraySegment byteArraySegment)
        {
            Header = header;
            ByteArraySegment = byteArraySegment;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public ProtocolType Header { get; }

        /// <summary>
        /// Gets or sets the length of the header extension in 8-octets (bytes) units, not including the first 8 octets.
        /// </summary>
        public int HeaderExtensionLength
        {
            get
            {
                if (Header == ProtocolType.IPv6FragmentHeader)
                    return 0;


                return ByteArraySegment.Bytes[ByteArraySegment.Offset + IPv6Fields.HeaderExtensionLengthPosition];
            }
            set
            {
                if (Header == ProtocolType.IPv6FragmentHeader)
                    return;


                ByteArraySegment.Bytes[ByteArraySegment.Offset + IPv6Fields.HeaderExtensionLengthPosition] = (byte) value;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public ushort Length => (ushort) ((HeaderExtensionLength + 1) * 8);

        /// <summary>
        /// Gets or sets the next header.
        /// </summary>
        public ProtocolType NextHeader
        {
            get => (ProtocolType) ByteArraySegment.Bytes[ByteArraySegment.Offset];
            set => ByteArraySegment.Bytes[ByteArraySegment.Offset] = (byte) value;
        }

        /// <summary>
        /// Gets the payload of the extension header.
        /// </summary>
        public ByteArraySegment Payload => _data ??= new ByteArraySegment(ByteArraySegment.Bytes, ByteArraySegment.Offset + IPv6Fields.HeaderExtensionDataPosition, Length - IPv6Fields.HeaderExtensionDataPosition);
    }