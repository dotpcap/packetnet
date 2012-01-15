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
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Length">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public static string GetString(byte[] Byte,
                                       int Offset,
                                       int Length)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = Offset; i < Offset + Length; i++)
            {
                sb.AppendFormat("[{0:x2}]", Byte[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a string from a Physical address in the format "xx:xx:xx:xx:xx:xx"
        /// </summary>
        /// <param name="address">
        /// A <see cref="PhysicalAddress"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public static string PrintMACAddress(PhysicalAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            string output = "";

            for(int i = 0; i < bytes.Length; i++)
            {
                output += bytes[i].ToString("x").PadLeft(2, '0') + ":";
            }
            return output.TrimEnd(':');
        }
    }
}
