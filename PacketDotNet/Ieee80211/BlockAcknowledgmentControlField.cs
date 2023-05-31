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
    /// Block acknowledgment control field.
    /// </summary>
    public class BlockAcknowledgmentControlField
    {
        /// <summary>
        /// The available block acknowledgement policies.
        /// </summary>
        public enum AcknowledgementPolicy
        {
            /// <summary>
            /// The acknowledgement does not have to be sent immediately after the request
            /// </summary>
            Delayed = 0,

            /// <summary>
            /// The acknowledgement must be sent immediately after the request
            /// </summary>
            Immediate = 1
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAcknowledgmentControlField" /> class.
        /// </summary>
        public BlockAcknowledgmentControlField()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAcknowledgmentControlField" /> class.
        /// </summary>
        /// <param name='field'>
        /// Field.
        /// </param>
        public BlockAcknowledgmentControlField(ushort field)
        {
            Field = field;
        }

        /// <summary>
        /// True if the frame is using a compressed acknowledgement bitmap.
        /// Newer standards used a compressed bitmap reducing its size
        /// </summary>
        public bool CompressedBitmap
        {
            get => ((Field >> 2) & 0x1) == 1;
            set
            {
                if (value)
                    Field |= 1 << 0x2;
                else
                    Field &= unchecked((ushort) ~(1 << 0x2));
            }
        }

        /// <summary>
        /// Gets or sets the field. This provides direct access to the bytes that back all the other properties in the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public ushort Field { get; set; }

        /// <summary>
        /// True if the acknowledgement can ack multi traffic ids
        /// </summary>
        public bool MultiTid
        {
            get => ((Field >> 1) & 0x1) == 1;
            set
            {
                if (value)
                    Field |= 1 << 0x1;
                else
                    Field &= unchecked((ushort) ~(1 << 0x1));
            }
        }

        /// <summary>
        /// The block acknowledgement policy in use
        /// </summary>
        public AcknowledgementPolicy Policy
        {
            get => (AcknowledgementPolicy) (Field & 0x1);
            set
            {
                if (value == AcknowledgementPolicy.Immediate)
                    Field |= 0x1;
                else
                    Field &= unchecked((ushort) ~0x1);
            }
        }

        /// <summary>
        /// The traffic id being ack'd
        /// </summary>
        public byte Tid
        {
            get => (byte) (Field >> 12);
            set
            {
                Field &= 0x0FFF;
                Field |= (ushort) (value << 12);
            }
        }
    }