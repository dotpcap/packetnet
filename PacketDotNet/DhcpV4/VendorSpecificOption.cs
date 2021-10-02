/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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
            Information = Array.Empty<byte>();
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