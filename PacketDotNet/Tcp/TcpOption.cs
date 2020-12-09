/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;
using PacketDotNet.Utils;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// A TCP Option
    /// </summary>
    public abstract class TcpOption
    {
        /// <summary>The length (in bytes) of the Kind field</summary>
        internal const int KindFieldLength = 1;

        /// <summary>The offset (in bytes) of the Kind Field</summary>
        internal const int KindFieldOffset = 0;

        /// <summary>The length (in bytes) of the Length field</summary>
        internal const int LengthFieldLength = 1;

        /// <summary>The offset (in bytes) of the Length field</summary>
        internal const int LengthFieldOffset = 1;

        // stores the data/length/offset of the option
        protected readonly ByteArraySegment OptionData;

        /// <summary>
        /// Creates an Option from a byte[]
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        protected TcpOption(byte[] bytes, int offset, int length)
        {
            OptionData = new ByteArraySegment(bytes, offset, length);
        }

        /// <summary>
        /// Gets or sets a the underlying bytes.
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                var bytes = new byte[OptionData.Length];
                Array.Copy(OptionData.Bytes, OptionData.Offset, bytes, 0, OptionData.Length);
                return bytes;
            }
            set
            {
                for (var i = 0; i < value.Length; i++)
                    OptionData.Bytes[OptionData.Offset + i] = value[i];
            }
        }

        /// <summary>
        /// Gets or sets the option kind.
        /// </summary>
        public OptionTypes Kind
        {
            get => (OptionTypes) OptionData.Bytes[OptionData.Offset + KindFieldOffset];
            set => OptionData.Bytes[OptionData.Offset + KindFieldOffset] = (byte) value;
        }

        /// <summary>
        /// Gets or sets the length of the option.
        /// </summary>
        public virtual byte Length
        {
            get => OptionData.Bytes[OptionData.Offset + LengthFieldOffset];
            set => OptionData.Bytes[OptionData.Offset + LengthFieldOffset] = value;
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + "]";
        }
    }
}