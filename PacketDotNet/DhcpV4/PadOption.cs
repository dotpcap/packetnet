/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class PadOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PadOption" /> class.
        /// </summary>
        public PadOption() : base(DhcpV4OptionType.Pad)
        {
            Pad = new byte[1];
        }

        /// <inheritdoc />
        public override byte[] Data => Pad;

        /// <inheritdoc />
        public override int Length => 1;

        /// <summary>
        /// Gets the pad.
        /// </summary>
        public byte[] Pad { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Pad: 0";
        }
    }
}