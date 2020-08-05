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
using System.Linq;

namespace PacketDotNet.DhcpV4
{
    public class VendorSpecificOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VendorSpecificOption" /> class.
        /// </summary>
        public VendorSpecificOption() : base(DhcpV4OptionType.VendorSpecific)
        {
            Information = new byte[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorSpecificOption" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public VendorSpecificOption(byte[] data) : base(DhcpV4OptionType.VendorSpecific)
        {
            Information = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorSpecificOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="optionLength">Length of the option.</param>
        public VendorSpecificOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.VendorSpecific)
        {
            Information = new byte[optionLength];

            for (int i = 0; i < optionLength; i++)
                Information[i] = buffer[offset + i];
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public override byte[] Data => Information;

        /// <summary>
        /// Gets or sets the vendor specific information.
        /// </summary>
        public byte[] Information { get; set; }

        /// <inheritdoc />
        public override int Length => Information.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Vendor Specific: {String.Join(",", Information.Select(x => x.ToString("X2")))}";
        }
    }
}