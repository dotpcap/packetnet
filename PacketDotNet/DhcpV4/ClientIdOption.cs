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
using System.Linq;
using System.Net.NetworkInformation;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class ClientIdOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public ClientIdOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.ClientId)
        {
            if (offset < buffer.Length)
                HardwareType = (DhcpV4HardwareType) buffer[offset];

            if (optionLength > 0 && offset + optionLength - 1 < buffer.Length)
            {
                var address = new byte[optionLength - 1];
                Array.Copy(buffer, offset + 1, address, 0, address.Length);
                HardwareAddress = new PhysicalAddress(address);
            }
            else
                HardwareAddress = PhysicalAddress.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientIdOption" /> class.
        /// </summary>
        /// <param name="hardwareAddress">The hardware address.</param>
        public ClientIdOption(PhysicalAddress hardwareAddress) : base(DhcpV4OptionType.ClientId)
        {
            HardwareAddress = hardwareAddress;
        }

        /// <inheritdoc />
        public override byte[] Data => new[] { (byte) HardwareType }.Concat(HardwareAddress.GetAddressBytes()).ToArray();

        /// <summary>
        /// Gets or sets the hardware address.
        /// </summary>
        public PhysicalAddress HardwareAddress { get; set; }

        /// <summary>
        /// Gets or sets the type of the hardware.
        /// </summary>
        public DhcpV4HardwareType HardwareType { get; set; }

        /// <inheritdoc />
        public override int Length => 1 + HardwareAddress.GetAddressBytes().Length;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Client Id: {HardwareType} - {HardwareAddress}";
        }
    }
}