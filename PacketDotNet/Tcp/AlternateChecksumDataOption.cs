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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Alternative Checksum Data
    /// Used as an extension to Alternative Checksum Response when the
    /// checksum is longer than the standard 16bit TCP Checksum field
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1146/
    /// </remarks>
    public class AlternateChecksumDataOption : TcpOption
    {
        // the offset (in bytes) of the Data Field
        private const int DataFieldOffset = 2;

        /// <summary>
        /// Creates an Alternate Checksum Data Option
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public AlternateChecksumDataOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The array of attached Checksum
        /// </summary>
        public byte[] Data
        {
            get
            {
                var data = new byte[Length - DataFieldOffset];
                Array.Copy(OptionData.Bytes, OptionData.Offset + DataFieldOffset, data, 0, data.Length);
                return data;
            }
            set => Array.Copy(value, 0, OptionData.Bytes, OptionData.Offset + DataFieldOffset, value.Length);
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": Data=0x" + Data + "]";
        }
    }
}