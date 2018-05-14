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
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Encapsulates and ensures that we have either a Packet OR
    /// a ByteArraySegment but not both
    /// </summary>
    [Serializable]
    public class PacketOrByteArraySegment
    {
        private ByteArraySegment _byteArraySegment;

        private Packet _packet;

        /// <summary>
        /// Gets or sets the byte array segment.
        /// </summary>
        /// <value>
        /// The byte array segment.
        /// </value>
        public ByteArraySegment ByteArraySegment
        {
            get => _byteArraySegment;

            set
            {
                _packet = null;
                _byteArraySegment = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet.
        /// </summary>
        /// <value>
        /// The packet.
        /// </value>
        public Packet Packet
        {
            get => _packet;

            set
            {
                _byteArraySegment = null;
                _packet = value;
            }
        }

        /// <value>
        /// Whether or not this container contains a packet, a byte[] or neither
        /// </value>
        public PayloadType Type
        {
            get
            {
                if (Packet != null)
                    return PayloadType.Packet;


                return ByteArraySegment != null ? PayloadType.Bytes : PayloadType.None;
            }
        }

        /// <summary>
        /// Appends to the MemoryStream either the byte[] represented by TheByteArray, or
        /// if ThePacket is non-null, the Packet.Bytes will be appended to the memory stream
        /// which will append ThePacket's header and any encapsulated packets it contains
        /// </summary>
        /// <param name="ms">
        /// A <see cref="MemoryStream" />
        /// </param>
        public void AppendToMemoryStream(MemoryStream ms)
        {
            if (Packet != null)
            {
                var bytes = Packet.Bytes;
                ms.Write(bytes, 0, bytes.Length);
            }
            else if (ByteArraySegment != null)
            {
                var bytes = ByteArraySegment.ActualBytes();
                ms.Write(bytes, 0, bytes.Length);
            }
        }
    }
}