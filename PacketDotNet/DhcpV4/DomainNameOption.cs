/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

    public class DomainNameOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameOption" /> class.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        public DomainNameOption(string domainName) : base(DhcpV4OptionType.DomainName)
        {
            DomainName = domainName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public DomainNameOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.DomainName)
        {
            DomainName = Encoding.ASCII.GetString(buffer, Convert.ToInt32(offset), optionLength);
        }

        /// <inheritdoc />
        public override byte[] Data => Encoding.ASCII.GetBytes(DomainName);

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        public string DomainName { get; set; }

        /// <inheritdoc />
        public override int Length => DomainName.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return "Domain Name: " + DomainName;
        }
    }