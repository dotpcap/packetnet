/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using PacketDotNet.Utils;

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// An End Of LLDPDU Tlv
    /// </summary>
    public class EndOfLldpduTlv : Tlv
    {
        /// <summary>
        /// Parses bytes into an End Of LLDPDU Tlv
        /// </summary>
        /// <param name="bytes">
        /// TLV bytes
        /// </param>
        /// <param name="offset">
        /// The End Of LLDPDU TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public EndOfLldpduTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Creates an End Of LLDPDU Tlv
        /// </summary>
        public EndOfLldpduTlv()
        {
            var bytes = new byte[TlvTypeLength.TypeLengthLength];
            var length = bytes.Length;
            Data = new ByteArraySegment(bytes, 0, length);

            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Convert this LLDPDU TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return "[EndOfLldpdu]";
        }
    }
}