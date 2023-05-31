/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Net;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

    public class ServerIdOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerIdOption" /> class.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        public ServerIdOption(IPAddress serverId) : base(DhcpV4OptionType.DHCPServerId)
        {
            ServerId = serverId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerIdOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public ServerIdOption(byte[] buffer, int offset) : base(DhcpV4OptionType.DHCPServerId)
        {
            ServerId = offset + IPv4Fields.AddressLength < buffer.Length ? IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, buffer) : IPAddress.Any;
        }

        /// <inheritdoc />
        public override byte[] Data => ServerId.GetAddressBytes();

        /// <inheritdoc />
        public override int Length => IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public IPAddress ServerId { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Server Id: {ServerId}";
        }
    }