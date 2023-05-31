/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using PacketDotNet.Utils;

namespace PacketDotNet.Ndp;

    public abstract class NdpOption
    {
        /// <summary>The offset (in bytes) of the Length field</summary>
        internal const int LengthFieldOffset = 1;

        /// <summary>The offset (in bytes) of the Payload field</summary>
        internal const int PayloadOffset = 2;

        /// <summary>The offset (in bytes) of the Type field</summary>
        internal const int TypeFieldOffset = 0;

        // stores the data/length/offset of the option
        protected readonly ByteArraySegment OptionData;

        protected NdpOption(byte[] bytes, int offset, int length)
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
        /// Gets or sets the length of the option.
        /// </summary>
        public virtual byte Length
        {
            get => OptionData.Bytes[OptionData.Offset + LengthFieldOffset];
            set => OptionData.Bytes[OptionData.Offset + LengthFieldOffset] = value;
        }

        /// <summary>
        /// Gets or sets the option type.
        /// </summary>
        public OptionTypes Type
        {
            get => (OptionTypes)OptionData.Bytes[OptionData.Offset + TypeFieldOffset];
            set => OptionData.Bytes[OptionData.Offset + TypeFieldOffset] = (byte)value;
        }
    }