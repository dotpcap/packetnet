/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Net.NetworkInformation;
using System.Text;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Helper class that prints out an array of hex values
    /// </summary>
    public class HexPrinter
    {
        /// <summary>
        /// Create a string that contains the hex values of byte[] Byte in
        /// text form
        /// </summary>
        /// <param name="Byte">
        /// A <see cref="byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public static string GetString(byte[] Byte, int offset, int length)
        {
            var sb = new StringBuilder();

            for (var i = offset; i < offset + length; i++)
            {
                sb.AppendFormat("[{0:x2}]", Byte[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a string from a Physical address in the format "xx:xx:xx:xx:xx:xx"
        /// </summary>
        /// <param name="address">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public static string PrintMACAddress(PhysicalAddress address)
        {
            var bytes = address.GetAddressBytes();
            var output = "";

            foreach (var t in bytes)
            {
                output += t.ToString("x").PadLeft(2, '0') + ":";
            }

            return output.TrimEnd(':');
        }
    }
}