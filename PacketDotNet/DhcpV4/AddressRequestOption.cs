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
}