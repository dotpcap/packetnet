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

    public class TFTPServerNameOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TFTPServerNameOption" /> class.
        /// </summary>
        /// <param name="tftpServerName">Name of the TFTP server.</param>
        public TFTPServerNameOption(string tftpServerName) : base(DhcpV4OptionType.ServerName)
        {
            TFTPServerName = tftpServerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TFTPServerNameOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public TFTPServerNameOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.ServerName)
        {
            TFTPServerName = Encoding.ASCII.GetString(buffer, Convert.ToInt32(offset), optionLength);
        }

        /// <inheritdoc />
        public override byte[] Data => Encoding.ASCII.GetBytes(TFTPServerName);

        /// <inheritdoc />
        public override int Length => TFTPServerName.Length;

        /// <summary>
        /// Gets or sets the name of the TFTP server.
        /// </summary>
        public string TFTPServerName { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "TFTP Server: " + TFTPServerName;
        }
    }