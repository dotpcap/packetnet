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