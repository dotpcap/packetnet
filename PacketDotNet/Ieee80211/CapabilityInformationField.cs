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
    ///     Capability information field.
    /// </summary>
    public class CapabilityInformationField
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> class.
        /// </summary>
        public CapabilityInformationField()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="field">
        ///     A <see cref="ushort" />
        /// </param>
        public CapabilityInformationField(UInt16 field)
        {
            this.Field = field;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> cf pollable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cf pollable; otherwise, <c>false</c>.
        /// </value>
        public Boolean CfPollable
        {
            get => this.GetBitFieldValue(2);

            set => this.SetBitFieldValue(2, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> cf poll request.
        /// </summary>
        /// <value>
        ///     <c>true</c> if cf poll request; otherwise, <c>false</c>.
        /// </value>
        public Boolean CfPollRequest
        {
            get => this.GetBitFieldValue(3);

            set => this.SetBitFieldValue(3, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> channel agility.
        /// </summary>
        /// <value>
        ///     <c>true</c> if channel agility; otherwise, <c>false</c>.
        /// </value>
        public Boolean ChannelAgility
        {
            get => this.GetBitFieldValue(7);

            set => this.SetBitFieldValue(7, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> dss ofdm.
        /// </summary>
        /// <value>
        ///     <c>true</c> if dss ofdm; otherwise, <c>false</c>.
        /// </value>
        public Boolean DssOfdm
        {
            get => this.GetBitFieldValue(13);

            set => this.SetBitFieldValue(13, value);
        }

        /// <summary>
        ///     Gets or sets the field.
        /// </summary>
        /// <value>
        ///     The field.
        /// </value>
        public UInt16 Field { get; set; }

        /// <summary>
        ///     Is set to 1 when the beacon frame is representing an ESS (as opposed to an IBSS)
        ///     This field and IsIbss should be mutually exclusive
        /// </summary>
        public Boolean IsEss
        {
            get => this.GetBitFieldValue(0);

            set => this.SetBitFieldValue(0, value);
        }

        /// <summary>
        ///     Is set to 1 when the beacon frame is representing an IBSS (as opposed to an ESS)
        ///     This field and IsEss should be mutually exclusive
        /// </summary>
        public Boolean IsIbss
        {
            get => this.GetBitFieldValue(1);

            set => this.SetBitFieldValue(1, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> is pbcc.
        /// </summary>
        /// <value>
        ///     <c>true</c> if pbcc; otherwise, <c>false</c>.
        /// </value>
        public Boolean Pbcc
        {
            get => this.GetBitFieldValue(6);

            set => this.SetBitFieldValue(6, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> is privacy.
        /// </summary>
        /// <value>
        ///     <c>true</c> if privacy; otherwise, <c>false</c>.
        /// </value>
        public Boolean Privacy
        {
            get => this.GetBitFieldValue(4);

            set => this.SetBitFieldValue(4, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> short preamble.
        /// </summary>
        /// <value>
        ///     <c>true</c> if short preamble; otherwise, <c>false</c>.
        /// </value>
        public Boolean ShortPreamble
        {
            get => this.GetBitFieldValue(5);

            set => this.SetBitFieldValue(5, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this
        ///     <see cref="PacketDotNet.Ieee80211.CapabilityInformationField" /> short time slot.
        /// </summary>
        /// <value>
        ///     <c>true</c> if short time slot; otherwise, <c>false</c>.
        /// </value>
        public Boolean ShortTimeSlot
        {
            get => this.GetBitFieldValue(10);

            set => this.SetBitFieldValue(10, value);
        }

        /// <summary>
        ///     Returns true if the bit is set false if not.
        /// </summary>
        /// <param name="index">0 indexed position of the bit</param>
        private Boolean GetBitFieldValue(UInt16 index)
        {
            return (((this.Field >> index) & 0x1) == 1) ? true : false;
        }

        private void SetBitFieldValue(UInt16 index, Boolean value)
        {
            if (value)
            {
                this.Field |= unchecked((UInt16) (1 << index));
            }
            else
            {
                this.Field &= unchecked((UInt16) ~(1 << index));
            }
        }
    }
}