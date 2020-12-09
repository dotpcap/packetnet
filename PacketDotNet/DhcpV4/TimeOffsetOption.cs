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
    public class TimeOffsetOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOffsetOption" /> class.
        /// </summary>
        /// <param name="timeOffset">The time offset.</param>
        public TimeOffsetOption(TimeSpan timeOffset) : base(DhcpV4OptionType.TimeOffset)
        {
            TimeOffset = timeOffset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeOffsetOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public TimeOffsetOption(byte[] buffer, int offset) : base(DhcpV4OptionType.TimeOffset)
        {
            if (offset + 4 < buffer.Length)
                TimeOffset = TimeSpan.FromSeconds(EndianBitConverter.Big.ToUInt32(buffer, offset));
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Convert.ToInt32(TimeOffset.TotalSeconds));

        /// <inheritdoc />
        public override int Length => 4;

        /// <summary>
        /// Gets or sets the time offset.
        /// </summary>
        public TimeSpan TimeOffset { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Time Offset: {TimeOffset}";
        }
    }
}