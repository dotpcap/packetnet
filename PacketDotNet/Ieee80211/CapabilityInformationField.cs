/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Capability information field.
    /// </summary>
    public class CapabilityInformationField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapabilityInformationField" /> class.
        /// </summary>
        public CapabilityInformationField()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="ushort" />
        /// </param>
        public CapabilityInformationField(ushort field)
        {
            Field = field;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> cf pollable.
        /// </summary>
        /// <value>
        /// <c>true</c> if cf pollable; otherwise, <c>false</c>.
        /// </value>
        public bool CfPollable
        {
            get => GetBitFieldValue(2);
            set => SetBitFieldValue(2, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> cf poll request.
        /// </summary>
        /// <value>
        /// <c>true</c> if cf poll request; otherwise, <c>false</c>.
        /// </value>
        public bool CfPollRequest
        {
            get => GetBitFieldValue(3);
            set => SetBitFieldValue(3, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> channel agility.
        /// </summary>
        /// <value>
        /// <c>true</c> if channel agility; otherwise, <c>false</c>.
        /// </value>
        public bool ChannelAgility
        {
            get => GetBitFieldValue(7);
            set => SetBitFieldValue(7, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> dss ofdm.
        /// </summary>
        /// <value>
        /// <c>true</c> if dss ofdm; otherwise, <c>false</c>.
        /// </value>
        public bool DssOfdm
        {
            get => GetBitFieldValue(13);
            set => SetBitFieldValue(13, value);
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public ushort Field { get; set; }

        /// <summary>
        /// Is set to 1 when the beacon frame is representing an ESS (as opposed to an IBSS)
        /// This field and IsIbss should be mutually exclusive
        /// </summary>
        public bool IsEss
        {
            get => GetBitFieldValue(0);
            set => SetBitFieldValue(0, value);
        }

        /// <summary>
        /// Is set to 1 when the beacon frame is representing an IBSS (as opposed to an ESS)
        /// This field and IsEss should be mutually exclusive
        /// </summary>
        public bool IsIbss
        {
            get => GetBitFieldValue(1);
            set => SetBitFieldValue(1, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> is PBCC.
        /// </summary>
        /// <value>
        /// <c>true</c> if PBCC; otherwise, <c>false</c>.
        /// </value>
        public bool Pbcc
        {
            get => GetBitFieldValue(6);
            set => SetBitFieldValue(6, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> is privacy.
        /// </summary>
        /// <value>
        /// <c>true</c> if privacy; otherwise, <c>false</c>.
        /// </value>
        public bool Privacy
        {
            get => GetBitFieldValue(4);
            set => SetBitFieldValue(4, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> short preamble.
        /// </summary>
        /// <value>
        /// <c>true</c> if short preamble; otherwise, <c>false</c>.
        /// </value>
        public bool ShortPreamble
        {
            get => GetBitFieldValue(5);
            set => SetBitFieldValue(5, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="CapabilityInformationField" /> short time slot.
        /// </summary>
        /// <value>
        /// <c>true</c> if short time slot; otherwise, <c>false</c>.
        /// </value>
        public bool ShortTimeSlot
        {
            get => GetBitFieldValue(10);
            set => SetBitFieldValue(10, value);
        }

        /// <summary>
        /// Returns true if the bit is set false if not.
        /// </summary>
        /// <param name="index">0 indexed position of the bit</param>
        private bool GetBitFieldValue(ushort index)
        {
            return ((Field >> index) & 0x1) == 1;
        }

        private void SetBitFieldValue(ushort index, bool value)
        {
            if (value)
            {
                Field |= unchecked((ushort) (1 << index));
            }
            else
            {
                Field &= unchecked((ushort) ~(1 << index));
            }
        }
    }
}