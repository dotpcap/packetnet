/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using PacketDotNet.Utils.Converters;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class RebindingTimeOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RebindingTimeOption" /> class.
        /// </summary>
        /// <param name="rebindingTime">The rebinding time.</param>
        public RebindingTimeOption(TimeSpan rebindingTime) : base(DhcpV4OptionType.RebindingTime)
        {
            RebindingTime = rebindingTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RebindingTimeOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public RebindingTimeOption(byte[] buffer, int offset) : base(DhcpV4OptionType.RebindingTime)
        {
            if (offset + 4 < buffer.Length)
                RebindingTime = TimeSpan.FromSeconds(EndianBitConverter.Big.ToUInt32(buffer, offset));
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Convert.ToInt32(RebindingTime.TotalSeconds));

        /// <inheritdoc />
        public override int Length => 4;

        /// <summary>
        /// Gets or sets the rebinding time.
        /// </summary>
        public TimeSpan RebindingTime { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Rebinding Time: {RebindingTime}";
        }
    }
}