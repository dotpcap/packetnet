/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

namespace PacketDotNet.Ieee80211;

    /// <summary>
    /// The Sequence control field occurs in management and data frames and is used to
    /// relate together fragmented payloads carried in multiple 802.11 frames.
    /// </summary>
    public class SequenceControlField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceControlField" /> class.
        /// </summary>
        public SequenceControlField()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="ushort" />
        /// </param>
        public SequenceControlField(ushort field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field that backs all the other properties in the class.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public ushort Field { get; set; }

        /// <summary>
        /// Gets or sets the fragment number.
        /// </summary>
        /// <value>
        /// The fragment number.
        /// </value>
        public byte FragmentNumber
        {
            get => (byte) (Field & 0x000F);
            set
            {
                Field &= unchecked((ushort) ~0xF);
                Field |= (ushort) (value & 0x0F);
            }
        }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <value>
        /// The sequence number.
        /// </value>
        public short SequenceNumber
        {
            get => (short) (Field >> 4);
            set
            {
                //Use the & mask to make sure we only overwrite the sequence number part of the field
                Field &= 0xF;
                Field |= (ushort) (value << 4);
            }
        }
    }