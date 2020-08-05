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
 *  This file is licensed under the Apache License, Version 2.0.
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