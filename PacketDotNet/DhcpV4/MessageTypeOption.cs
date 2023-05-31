/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

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