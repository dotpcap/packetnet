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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class NTPServersOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NTPServersOption" /> class.
        /// </summary>
        /// <param name="ntpServers">The NTP servers.</param>
        public NTPServersOption(List<IPAddress> ntpServers) : base(DhcpV4OptionType.NTPServers)
        {
            NTPServers = ntpServers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NTPServersOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public NTPServersOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.NTPServers)
        {
            NTPServers = new List<IPAddress>();

            for (int i = 0; i < optionLength; i += 4)
            {
                if (offset + i + 4 < buffer.Length)
                    NTPServers.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset + i, buffer));
            }
        }

        /// <inheritdoc />
        public override byte[] Data
        {
            get
            {
                var bytes = new byte[Length];
                for (int i = 0; i < NTPServers.Count; i++)
                    Array.Copy(NTPServers[i].GetAddressBytes(), 0, bytes, i * IPv4Fields.AddressLength, IPv4Fields.AddressLength);

                return bytes;
            }
        }

        /// <inheritdoc />
        public override int Length => NTPServers.Count * IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the NTP servers.
        /// </summary>
        public List<IPAddress> NTPServers { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"NTP Servers: {String.Join(", ", NTPServers)}";
        }
    }
}