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
        public BlockAcknowledgmentControlField(UInt16 field)
        {
            Field = field;
        }

        /// <summary>
        /// True if the frame is using a compressed acknowledgement bitmap.
        /// Newer standards used a compressed bitmap reducing its size
        /// </summary>
        public Boolean CompressedBitmap
        {
            get => ((Field >> 2) & 0x1) == 1;

            set
            {
                if (value)
                {
                    Field |= 1 << 0x2;
                }
                else
                {
                    Field &= unchecked((UInt16) ~(1 << 0x2));
                }
            }
        }

        /// <summary>
        /// Gets or sets the field. This provides direct access to the bytes that back all the other properties in the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UInt16 Field { get; set; }

        /// <summary>
        /// True if the acknowledgement can ack multi traffic ids
        /// </summary>
        public Boolean MultiTid
        {
            get => ((Field >> 1) & 0x1) == 1;

            set
            {
                if (value)
                {
                    Field |= 1 << 0x1;
                }
                else
                {
                    Field &= unchecked((UInt16) ~(1 << 0x1));
                }
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
                {
                    Field |= 0x1;
                }
                else
                {
                    Field &= unchecked((UInt16) ~0x1);
                }
            }
        }

        /// <summary>
        /// The traffic id being ack'd
        /// </summary>
        public Byte Tid
        {
            get => (Byte) (Field >> 12);

            set
            {
                Field &= 0x0FFF;
                Field |= (UInt16) (value << 12);
            }
        }
    }
}