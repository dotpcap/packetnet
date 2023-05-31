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

    public class SubnetMaskOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubnetMaskOption" /> class.
        /// </summary>
        /// <param name="subnetMask">The subnet mask.</param>
        public SubnetMaskOption(IPAddress subnetMask) : base(DhcpV4OptionType.SubnetMask)
        {
            SubnetMask = subnetMask;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubnetMaskOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public SubnetMaskOption(byte[] buffer, int offset) : base(DhcpV4OptionType.SubnetMask)
        {
            SubnetMask = offset + IPv4Fields.AddressLength < buffer.Length ? IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, buffer) : IPAddress.Any;
        }

        /// <inheritdoc />
        public override byte[] Data => SubnetMask.GetAddressBytes();

        /// <inheritdoc />
        public override int Length => IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the subnet mask.
        /// </summary>
        public IPAddress SubnetMask { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Subnet Mask: {SubnetMask}";
        }
    }