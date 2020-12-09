/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  This file is licensed under the Apache License, Version 2.0.
 */

using System;
using PacketDotNet.Utils.Converters;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class AddressTimeOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressTimeOption" /> class.
        /// </summary>
        /// <param name="addressTime">The address time.</param>
        public AddressTimeOption(TimeSpan addressTime) : base(DhcpV4OptionType.AddressTime)
        {
            AddressTime = addressTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressTimeOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        public AddressTimeOption(byte[] buffer, int offset) : base(DhcpV4OptionType.AddressTime)
        {
            if (offset + 4 < buffer.Length)
                AddressTime = TimeSpan.FromSeconds(EndianBitConverter.Big.ToUInt32(buffer, offset));
        }

        /// <summary>
        /// Gets or sets the address time.
        /// </summary>
        public TimeSpan AddressTime { get; set; }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Convert.ToInt32(AddressTime.TotalSeconds));

        /// <inheritdoc />
        public override int Length => 4;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Address Time: {AddressTime}";
        }
    }
}