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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Capability information field.
        /// </summary>
        public class CapabilityInformationField
        {

            /// <summary>
            /// Is set to 1 when the beacon frame is representing an ESS (as opposed to an IBSS)
            /// 
            /// This field and IsIbss should be mutually exclusive
            /// </summary>
            public bool IsEss
            {
                get
                {
                    return GetBitFieldValue(0);
                }

                set
                {
                    SetBitFieldValue(0, value);
                }
            }

            /// <summary>
            /// Is set to 1 when the beacon frame is representing an IBSS (as opposed to an ESS)
            /// 
            /// This field and IsEss should be mutually exclusive
            /// </summary>
            public bool IsIbss
            {
                get
                {
                    return GetBitFieldValue(1);
                }

                set
                {
                    SetBitFieldValue(1, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> cf pollable.
            /// </summary>
            /// <value>
            /// <c>true</c> if cf pollable; otherwise, <c>false</c>.
            /// </value>
            public bool CfPollable
            {
                get
                {
                    return GetBitFieldValue(2);
                }

                set
                {
                    SetBitFieldValue(2, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> cf poll request.
            /// </summary>
            /// <value>
            /// <c>true</c> if cf poll request; otherwise, <c>false</c>.
            /// </value>
            public bool CfPollRequest
            {
                get
                {
                    return GetBitFieldValue(3);
                }

                set
                {
                    SetBitFieldValue(3, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> is privacy.
            /// </summary>
            /// <value>
            /// <c>true</c> if privacy; otherwise, <c>false</c>.
            /// </value>
            public bool Privacy
            {
                get
                {
                    return GetBitFieldValue(4);
                }

                set
                {
                    SetBitFieldValue(4, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> short preamble.
            /// </summary>
            /// <value>
            /// <c>true</c> if short preamble; otherwise, <c>false</c>.
            /// </value>
            public bool ShortPreamble
            {
                get
                {
                    return GetBitFieldValue(5);
                }

                set
                {
                    SetBitFieldValue(5, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> is pbcc.
            /// </summary>
            /// <value>
            /// <c>true</c> if pbcc; otherwise, <c>false</c>.
            /// </value>
            public bool Pbcc
            {
                get
                {
                    return GetBitFieldValue(6);
                }

                set
                {
                    SetBitFieldValue(6, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> channel agility.
            /// </summary>
            /// <value>
            /// <c>true</c> if channel agility; otherwise, <c>false</c>.
            /// </value>
            public bool ChannelAgility
            {
                get
                {
                    return GetBitFieldValue(7);
                }

                set
                {
                    SetBitFieldValue(7, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> short time slot.
            /// </summary>
            /// <value>
            /// <c>true</c> if short time slot; otherwise, <c>false</c>.
            /// </value>
            public bool ShortTimeSlot
            {
                get
                {
                    return GetBitFieldValue(10);
                }

                set
                {
                    SetBitFieldValue(10, value);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this
            /// <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> dss ofdm.
            /// </summary>
            /// <value>
            /// <c>true</c> if dss ofdm; otherwise, <c>false</c>.
            /// </value>
            public bool DssOfdm
            {
                get
                {
                    return GetBitFieldValue(13);
                }

                set
                {
                    SetBitFieldValue(13, value);
                }
            }

            /// <summary>
            /// Returns true if the bit is set false if not.
            /// </summary>
            /// <param name="index">0 indexed position of the bit</param>
            private bool GetBitFieldValue(ushort index)
            {
                return (((Field >> index) & 0x1) == 1) ? true : false;
            }

            private void SetBitFieldValue(ushort index, bool value)
            {
                if (value)
                {
                    Field |= unchecked((UInt16)(1 << index));
                }
                else
                {
                    Field &= unchecked((UInt16)~(1 << index));
                }
            }

            /// <summary>
            /// Gets or sets the field.
            /// </summary>
            /// <value>
            /// The field.
            /// </value>
            public UInt16 Field { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.CapabilityInformationField"/> class.
            /// </summary>
            public CapabilityInformationField()
            {

            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field">
            /// A <see cref="UInt16"/>
            /// </param>
            public CapabilityInformationField(UInt16 field)
            {
                this.Field = field;
            }
        } 
    }
}
