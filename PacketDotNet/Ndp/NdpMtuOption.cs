/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Ndp;

    public class NdpMtuOption : NdpOption
    {
        /// <summary>The offset (in bytes) of the MTU field</summary>
        internal const int MtuOffset = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NdpMtuOption" /> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public NdpMtuOption(byte[] bytes, int offset, int length) : base(bytes, offset, length)
        { }

        /// <summary>
        /// Gets or sets the MTU.
        /// </summary>
        public uint Mtu
        {
            get => EndianBitConverter.Big.ToUInt32(OptionData.Bytes,
                                                   OptionData.Offset + MtuOffset);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    OptionData.Bytes,
                                                    OptionData.Offset + MtuOffset);
        }
    }