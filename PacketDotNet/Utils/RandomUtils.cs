/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Net;

namespace PacketDotNet.Utils;

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
        /// A <see cref="IPAddress" />
        /// </returns>
        public static IPAddress GetIPAddress(IPVersion version)
        {
            var rnd = new Random();
            byte[] randomAddressBytes;

            if (version == IPVersion.IPv4)
            {
                randomAddressBytes = new byte[IPv4Fields.AddressLength];
                rnd.NextBytes(randomAddressBytes);
            }
            else if (version == IPVersion.IPv6)
            {
                randomAddressBytes = new byte[IPv6Fields.AddressLength];
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
        /// A <see cref="int" />
        /// </returns>
        public static int LongestStringLength(List<string> stringsList)
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