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
 *  This file is licensed under the Apache License, Version 2.0.
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