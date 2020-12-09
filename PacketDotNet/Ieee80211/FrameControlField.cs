/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Every 802.11 frame has a control field that contains information about the frame including
    /// the 802.11 protocol version, frame type, and various indicators, such as whether WEP is on,
    /// power management is active.
    /// </summary>
    public partial class FrameControlField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameControlField" /> class.
        /// </summary>
        public FrameControlField()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="ushort" />
        /// </param>
        public FrameControlField(ushort field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public ushort Field { get; set; }

        /// <summary>
        /// Is set to 1 when the frame is received from the Distribution System (DS)
        /// </summary>
        public bool FromDS
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
                    Field &= unchecked((ushort) ~(1 << 0x1));
                }
            }
        }

        /// <summary>
        /// Indicates that there are more frames buffered for this station
        /// </summary>
        public bool MoreData
        {
            get => ((Field >> 5) & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 1 << 0x5;
                }
                else
                {
                    Field &= unchecked((ushort) ~(1 << 0x5));
                }
            }
        }

        /// <summary>
        /// More Fragment is set to 1 when there are more fragments belonging to the same
        /// frame following the current fragment
        /// </summary>
        public bool MoreFragments
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
                    Field &= unchecked((ushort) ~(1 << 0x2));
                }
            }
        }

        /// <summary>
        /// Bit is set when the "strict ordering" delivery method is employed. Frames and
        /// fragments are not always sent in order as it causes a transmission performance penalty.
        /// </summary>
        public bool Order
        {
            get => ((Field >> 0x7) & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 1 << 0x7;
                }
                else
                {
                    Field &= unchecked((ushort) ~(1 << 0x7));
                }
            }
        }

        /// <summary>
        /// Indicates the power management mode that the station will be in after the transmission of the frame
        /// </summary>
        public bool PowerManagement
        {
            get => ((Field >> 4) & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 1 << 0x4;
                }
                else
                {
                    Field &= unchecked((ushort) ~(1 << 0x4));
                }
            }
        }

        /// <summary>
        /// Indicates whether the frame body is encrypted with one of several encryption standards
        /// </summary>
        public bool Protected
        {
            get => ((Field >> 6) & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 1 << 0x6;
                }
                else
                {
                    Field &= unchecked((ushort) ~(1 << 0x6));
                }
            }
        }

        /// <summary>
        /// Protocol version
        /// </summary>
        public byte ProtocolVersion
        {
            get => (byte) ((Field >> 0x8) & 0x3);
            set
            {
                if (value > 3)
                {
                    throw new ArgumentException("Invalid protocol version value. Value must be in the range 0-3.");
                }

                //unset the two bits before setting them to the value
                Field &= unchecked((ushort) ~0x0300);
                Field |= (ushort) (value << 0x8);
            }
        }

        /// <summary>
        /// Indicates that this fragment is a retransmission of a previously transmitted fragment.
        /// (For receiver to recognize duplicate transmissions of frames)
        /// </summary>
        public bool Retry
        {
            get => ((Field >> 3) & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 1 << 0x3;
                }
                else
                {
                    Field &= unchecked((ushort) ~(1 << 0x3));
                }
            }
        }

        /// <summary>
        /// Helps to identify the type of WLAN frame, control data and management are
        /// the various frame types defined in IEEE 802.11
        /// </summary>
        public FrameSubTypes SubType
        {
            get
            {
                var typeAndSubtype = Field >> 8; //get rid of the flags
                var type = ((typeAndSubtype & 0x0C) << 2) | (typeAndSubtype >> 4);
                return (FrameSubTypes) type;
            }
            set
            {
                var val = (uint) value;
                var typeAndSubtype = ((val & 0x0F) << 4) | ((val >> 4) << 2);
                //shift it into the right position in the field
                typeAndSubtype = typeAndSubtype << 0x8;
                //Unset all the bits related to the type and subtype
                Field &= 0x03FF;
                //Set the type bits
                Field |= (ushort) typeAndSubtype;
            }
        }

        /// <summary>
        /// Is set to 1 when the frame is sent to Distribution System (DS)
        /// </summary>
        public bool ToDS
        {
            get => (Field & 0x1) == 1;
            set
            {
                if (value)
                {
                    Field |= 0x1;
                }
                else
                {
                    Field &= unchecked((ushort) ~0x1);
                }
            }
        }

        /// <summary>
        /// Gets the type of the frame.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public FrameTypes Type
        {
            get
            {
                var typeAndSubtype = Field >> 8; //get rid of the flags
                var type = (typeAndSubtype & 0xC) >> 2;
                return (FrameTypes) type;
            }
        }

        /// <summary>
        /// Indicates that the frame body is encrypted according to the WEP (wired equivalent privacy) algorithm
        /// </summary>
        [Obsolete("This property is obsolete. Use Protected instead.", false)]
        public bool Wep
        {
            get => Protected;
            set => Protected = value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="FrameControlField" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents the current <see cref="FrameControlField" />.
        /// </returns>
        public override string ToString()
        {
            var flags = new List<string>
            {
                SubType.ToString()
            };

            if (ToDS)
            {
                flags.Add("ToDS");
            }

            if (FromDS)
            {
                flags.Add("FromDS");
            }

            if (Retry)
            {
                flags.Add("Retry");
            }

            if (PowerManagement)
            {
                flags.Add("PowerManagement");
            }

            if (MoreData)
            {
                flags.Add("MoreData");
            }

            if (Protected)
            {
                flags.Add("Wep");
            }

            if (Order)
            {
                flags.Add("Order");
            }

            return String.Join(" ", flags.ToArray());
        }
    }
}