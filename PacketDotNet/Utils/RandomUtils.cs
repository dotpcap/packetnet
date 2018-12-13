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

using System;
using System.Collections.Generic;
using System.Net;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Random utility methods
    /// </summary>
    public class RandomUtils
    {
        /// <summary>
        /// Generate a random ip address
        /// </summary>
        /// <param name="version">
        /// A <see cref="IPVersion" />
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.IPAddress" />
        /// </returns>
        public static IPAddress GetIPAddress(IPVersion version)
        {
            var rnd = new Random();
            Byte[] randomAddressBytes;

            if (version == IPVersion.IPv4)
            {
                randomAddressBytes = new Byte[IPv4Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            }
            else if (version == IPVersion.IPv6)
            {
                randomAddressBytes = new Byte[IPv6Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            }
            else
            {
                throw new InvalidOperationException("Unknown version of " + version);
            }

            return new IPAddress(randomAddressBytes);
        }

        /// <summary>
        /// Get the length of the longest string in a list of strings
        /// </summary>
        /// <param name="stringsList">
        /// A <see cref="T:List{System.String}" />
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32" />
        /// </returns>
        public static Int32 LongestStringLength(List<String> stringsList)
        {
            var longest = "";

            foreach (var l in stringsList)
            {
                if (l.Length > longest.Length)
                {
                    longest = l;
                }
            }

            return longest.Length;
        }
    }
}