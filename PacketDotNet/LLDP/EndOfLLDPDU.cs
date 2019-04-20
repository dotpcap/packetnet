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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// An End Of LLDPDU Tlv
    /// </summary>
    [Serializable]
    public class EndOfLLDPDU : Tlv
    {
        #region Constructors

        /// <summary>
        /// Parses bytes into an End Of LLDPDU Tlv
        /// </summary>
        /// <param name="bytes">
        /// Tlv bytes
        /// </param>
        /// <param name="offset">
        /// The End Of LLDPDU Tlv's offset from the
        /// origin of the LLDP
        /// </param>
        public EndOfLLDPDU(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Creates an End Of LLDPDU Tlv
        /// </summary>
        public EndOfLLDPDU()
        {
            var bytes = new byte[TLVTypeLength.TypeLengthLength];
            var offset = 0;
            var length = bytes.Length;
            TLVData = new ByteArraySegment(bytes, offset, length);

            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Convert this TTL Tlv to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return "[EndOfLLDPDU]";
        }

        #endregion
    }
}