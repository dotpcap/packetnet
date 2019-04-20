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
using PacketDotNet.Utils;

// ReSharper disable InconsistentNaming
namespace PacketDotNet
{
    [Serializable]
    public class IPv6ExtensionHeader
    {
        protected ByteArraySegment _encapsulatedBytes;
        private ByteArraySegment _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPv6ExtensionHeader" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public IPv6ExtensionHeader(IPProtocolType header, ByteArraySegment byteArraySegment)
        {
            Header = header;
            _encapsulatedBytes = byteArraySegment;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public IPProtocolType Header { get; }

        /// <summary>
        /// Gets or sets the length of the header extension in 8-octets (bytes) units, not including the first 8 octets.
        /// </summary>
        public int HeaderExtensionLength
        {
            get
            {
                if (Header == IPProtocolType.FRAGMENT)
                    return 0;


                return _encapsulatedBytes.Bytes[_encapsulatedBytes.Offset + IPv6Fields.HeaderExtensionLengthPosition];
            }
            set
            {
                if (Header == IPProtocolType.FRAGMENT)
                    return;


                _encapsulatedBytes.Bytes[_encapsulatedBytes.Offset + IPv6Fields.HeaderExtensionLengthPosition] = (byte) value;
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public ushort Length => (ushort) ((HeaderExtensionLength + 1) * 8);

        /// <summary>
        /// Gets or sets the next header.
        /// </summary>
        public IPProtocolType NextHeader
        {
            get => (IPProtocolType) _encapsulatedBytes.Bytes[_encapsulatedBytes.Offset];
            set => _encapsulatedBytes.Bytes[_encapsulatedBytes.Offset] = (byte) value;
        }

        /// <summary>
        /// Gets the payload of the extension header.
        /// </summary>
        public ByteArraySegment Payload =>
            _data ?? (_data = new ByteArraySegment(_encapsulatedBytes.Bytes, _encapsulatedBytes.Offset + IPv6Fields.HeaderExtensionDataPosition, Length - IPv6Fields.HeaderExtensionDataPosition));
    }
}