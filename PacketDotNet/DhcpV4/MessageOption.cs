/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

    public class MessageOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOption" /> class.
        /// </summary>
        /// <param name="dhcpMessage">The DHCP message.</param>
        public MessageOption(string dhcpMessage) : base(DhcpV4OptionType.DHCPMessage)
        {
            DhcpMessage = dhcpMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public MessageOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.DHCPMessage)
        {
            DhcpMessage = Encoding.ASCII.GetString(buffer, Convert.ToInt32(offset), optionLength);
        }

        /// <inheritdoc />
        public override byte[] Data => Encoding.ASCII.GetBytes(DhcpMessage);

        /// <summary>
        /// Gets or sets the DHCP message.
        /// </summary>
        public string DhcpMessage { get; set; }

        /// <inheritdoc />
        public override int Length => DhcpMessage.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return "DHCP Message: " + DhcpMessage;
        }
    }