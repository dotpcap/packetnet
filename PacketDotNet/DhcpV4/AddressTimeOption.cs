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