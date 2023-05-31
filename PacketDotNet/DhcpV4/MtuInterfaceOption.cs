/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.DhcpV4;

    public class MtuInterfaceOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MtuInterfaceOption" /> class.
        /// </summary>
        /// <param name="mtu">The MTU of the interface.</param>
        public MtuInterfaceOption(ushort mtu) : base(DhcpV4OptionType.MTUInterface)
        {
            Mtu = mtu;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MtuInterfaceOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public MtuInterfaceOption(byte[] buffer, int offset) : base(DhcpV4OptionType.MTUInterface)
        {
            if (offset + 2 < buffer.Length)
                Mtu = EndianBitConverter.Big.ToUInt16(buffer, offset);
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Mtu);

        /// <inheritdoc />
        public override int Length => 2;

        /// <summary>
        /// Gets or sets the MTU of the interface.
        /// </summary>
        public ushort Mtu { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"MTU Interface: {Mtu}";
        }
    }