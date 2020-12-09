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
    public abstract class DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DhcpV4Option" /> class.
        /// </summary>
        /// <param name="optionType">Type of the option.</param>
        protected DhcpV4Option(DhcpV4OptionType optionType)
        {
            OptionType = optionType;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        public abstract byte[] Data { get; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the type of the option.
        /// </summary>
        public DhcpV4OptionType OptionType { get; }
    }
}