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
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public static String GetString
        (
            Byte[] Byte,
            Int32 offset,
            Int32 length)
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
        public static String PrintMACAddress(PhysicalAddress address)
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