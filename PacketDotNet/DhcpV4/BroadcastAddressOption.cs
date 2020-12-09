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

namespace PacketDotNet.DhcpV4
{
    public class BroadcastAddressOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastAddressOption" /> class.
        /// </summary>
        /// <param name="broadcastAddress">The broadcast address.</param>
        public BroadcastAddressOption(IPAddress broadcastAddress) : base(DhcpV4OptionType.BroadcastAddress)
        {
            BroadcastAddress = broadcastAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastAddressOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public BroadcastAddressOption(byte[] buffer, int offset) : base(DhcpV4OptionType.BroadcastAddress)
        {
            BroadcastAddress = offset + IPv4Fields.AddressLength < buffer.Length ? IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, buffer) : IPAddress.Any;
        }

        /// <summary>
        /// Gets or sets the broadcast address.
        /// </summary>
        public IPAddress BroadcastAddress { get; set; }

        /// <inheritdoc />
        public override byte[] Data => BroadcastAddress.GetAddressBytes();

        /// <inheritdoc />
        public override int Length => IPv4Fields.AddressLength;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Broadcast Address: {BroadcastAddress}";
        }
    }
}