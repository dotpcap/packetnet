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

    public class AddressRequestOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRequestOption" /> class.
        /// </summary>
        /// <param name="requestedIP">The requested ip.</param>
        public AddressRequestOption(IPAddress requestedIP) : base(DhcpV4OptionType.AddressRequest)
        {
            RequestedIP = requestedIP;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressRequestOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public AddressRequestOption(byte[] buffer, int offset) : base(DhcpV4OptionType.AddressRequest)
        {
            RequestedIP = offset + IPv4Fields.AddressLength < buffer.Length ? IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, buffer) : IPAddress.Any;
        }

        /// <inheritdoc />
        public override byte[] Data => RequestedIP.GetAddressBytes();

        /// <inheritdoc />
        public override int Length => IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the requested ip.
        /// </summary>
        public IPAddress RequestedIP { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Requested IP Address: {RequestedIP}";
        }
    }