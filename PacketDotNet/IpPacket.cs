/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Net;

namespace Packet.Net
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
    public abstract class IpPacket : InternetPacket
    {
        public abstract IPAddress DestinationAddress
        {
            get;
            set;
        }

        public abstract IPAddress SourceAddress
        {
            get;
            set;
        }

        /// <value>
        /// The IP version, 4 for IPv4, 6 for IPv6 etc 
        /// </value>
        public abstract int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a pseudo ip header to a given packet. Used to generate the full
        /// byte array required to generate a udp or tcp checksum.
        /// </summary>
        /// <param name="origHeader">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte"/>
        /// </returns>
        internal abstract byte[] AttachPseudoIPHeader(byte[] origHeader);

        public static System.Net.IPAddress GetIPAddress(System.Net.Sockets.AddressFamily ipType, int fieldOffset, byte[] bytes)
        {
            byte[] address;
            if(ipType == System.Net.Sockets.AddressFamily.InterNetwork) // ipv4
            {
                address = new byte[IPv4Fields.AddressLength];
            } else if(ipType == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                address = new byte[IPv6Fields.AddressLength];
            } else
            {
                throw new System.InvalidOperationException("ipType " + ipType + " unknown");
            }

            System.Array.Copy(bytes, fieldOffset,
                              address, 0, address.Length);

            return new System.Net.IPAddress(address);
        }
    }
}
