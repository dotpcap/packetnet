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
        /// A <see cref="IpVersion"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.IPAddress"/>
        /// </returns>
        public static System.Net.IPAddress GetIPAddress(IpVersion version)
        {
            var rnd = new Random();
            byte[] randomAddressBytes;

            if(version == IpVersion.IPv4)
            {
                randomAddressBytes = new byte[IPv4Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            } else if(version == IpVersion.IPv6)
            {
                randomAddressBytes = new byte[IPv6Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            } else
            {
                throw new System.InvalidOperationException("Unknown version of " + version);
            }

            return new System.Net.IPAddress(randomAddressBytes);
        }

        /// <summary>
        /// Get the length of the longest string in a list of strings
        /// </summary>
        /// <param name="stringsList">
        /// A <see cref="List<System.String>"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static int LongestStringLength(List<string> stringsList)
        {
            string longest="";

            foreach(string L in stringsList)
            {
                if (L.Length > longest.Length)
                {
                    longest = L;
                }
            }
            return longest.Length;
        }
    }
}
