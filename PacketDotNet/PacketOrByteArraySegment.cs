/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System.Text;
using System.IO;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Encapsulates and ensures that we have either a Packet OR a ByteArraySegment, but not both.
    /// </summary>
    public sealed class PacketOrByteArraySegment
    {
        private ByteArraySegment _byteArraySegment;

        private Packet _packet;

        /// <summary>
        /// Gets or sets the byte array segment.
        /// </summary>
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
        /// Whether or not this container contains a packet, a byte[] or neither.
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
        /// Appends either the byte array or the packet, if non-null, to the <see cref="MemoryStream" />.
        /// </summary>
        /// <param name="memoryStream">
        /// A <see cref="MemoryStream" />
        /// </param>
        public void AppendToMemoryStream(MemoryStream memoryStream)
        {
            if (Packet != null)
            {
                var bytes = Packet.Bytes;
                memoryStream.Write(bytes, 0, bytes.Length);
            }
            else if (ByteArraySegment != null)
            {
                foreach (var b in ByteArraySegment)
                    memoryStream.WriteByte(b);
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            if (Type == PayloadType.Bytes)
            {
                buffer.AppendFormat("ByteArraySegment: [" + ByteArraySegment + "]");
            } else
            {
                buffer.AppendFormat("Packet: [" + Packet + "]");
            }

            return buffer.ToString();
        }
    }
}