/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.DhcpV4
{
    public class MaxMessageSizeOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxMessageSizeOption" /> class.
        /// </summary>
        /// <param name="maxMessageSize">Maximum size of the message.</param>
        public MaxMessageSizeOption(ushort maxMessageSize) : base(DhcpV4OptionType.DHCPMaxMsgSize)
        {
            MaxMessageSize = maxMessageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxMessageSizeOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public MaxMessageSizeOption(byte[] buffer, int offset) : base(DhcpV4OptionType.DHCPMaxMsgSize)
        {
            if (offset + 2 < buffer.Length)
                MaxMessageSize = EndianBitConverter.Big.ToUInt16(buffer, offset);
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(MaxMessageSize);

        /// <inheritdoc />
        public override int Length => 2;

        /// <summary>
        /// Gets or sets the maximum size of the message.
        /// </summary>
        public ushort MaxMessageSize { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Max Message Size: {MaxMessageSize}";
        }
    }
}