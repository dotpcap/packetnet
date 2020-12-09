/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class HostNameOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostNameOption" /> class.
        /// </summary>
        /// <param name="hostName">The hostName.</param>
        public HostNameOption(string hostName) : base(DhcpV4OptionType.HostName)
        {
            HostName = hostName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostNameOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public HostNameOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.HostName)
        {
            HostName = Encoding.ASCII.GetString(buffer, offset, optionLength);
        }

        /// <inheritdoc />
        public override byte[] Data => Encoding.ASCII.GetBytes(HostName);

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        public string HostName { get; set; }

        /// <inheritdoc />
        public override int Length => HostName.Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return "HostName: " + HostName;
        }
    }
}