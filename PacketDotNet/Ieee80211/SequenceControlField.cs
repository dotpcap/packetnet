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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The Sequence control field occurs in management and data frames and is used to
    /// relate together fragmented payloads carried in multiple 802.11 frames.
    /// </summary>
    public class SequenceControlField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.SequenceControlField" /> class.
        /// </summary>
        public SequenceControlField()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="ushort" />
        /// </param>
        public SequenceControlField(UInt16 field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field that backs all the other properties in the class.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UInt16 Field { get; set; }

        /// <summary>
        /// Gets or sets the fragment number.
        /// </summary>
        /// <value>
        /// The fragment number.
        /// </value>
        public Byte FragmentNumber
        {
            get => (Byte) (Field & 0x000F);

            set
            {
                Field &= unchecked((UInt16) ~0xF);
                Field |= (UInt16) (value & 0x0F);
            }
        }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public Int16 SequenceNumber
        {
            get => (Int16) (Field >> 4);

            set
            {
                //Use the & mask to make sure we only overwrite the sequence number part of the field
                Field &= 0xF;
                Field |= (UInt16) (value << 4);
            }
        }
    }
}