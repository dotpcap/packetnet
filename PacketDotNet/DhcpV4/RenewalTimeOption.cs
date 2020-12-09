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
    public class RenewalTimeOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenewalTimeOption" /> class.
        /// </summary>
        /// <param name="renewalTime">The renewal time.</param>
        public RenewalTimeOption(TimeSpan renewalTime) : base(DhcpV4OptionType.RenewalTime)
        {
            RenewalTime = renewalTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenewalTimeOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        public RenewalTimeOption(byte[] buffer, int offset) : base(DhcpV4OptionType.RenewalTime)
        {
            if (offset + 4 < buffer.Length)
                RenewalTime = TimeSpan.FromSeconds(EndianBitConverter.Big.ToUInt32(buffer, offset));
        }

        /// <inheritdoc />
        public override byte[] Data => EndianBitConverter.Big.GetBytes(Convert.ToInt32(RenewalTime.TotalSeconds));

        /// <inheritdoc />
        public override int Length => 4;

        /// <summary>
        /// Gets or sets the renewal time.
        /// </summary>
        public TimeSpan RenewalTime { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Renewal Time: {RenewalTime}";
        }
    }
}