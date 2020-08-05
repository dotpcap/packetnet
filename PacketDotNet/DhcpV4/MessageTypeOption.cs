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

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class MessageTypeOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTypeOption" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        public MessageTypeOption(DhcpV4MessageType messageType) : base(DhcpV4OptionType.DHCPMsgType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTypeOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public MessageTypeOption(byte[] buffer, int offset) : base(DhcpV4OptionType.DHCPMsgType)
        {
            if (offset < buffer.Length)
                MessageType = (DhcpV4MessageType) buffer[offset];
        }

        /// <inheritdoc />
        public override byte[] Data => new[] { (byte) MessageType };

        /// <inheritdoc />
        public override int Length => 1;

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        public DhcpV4MessageType MessageType { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"DHCP Message Type: {MessageType}";
        }
    }
}