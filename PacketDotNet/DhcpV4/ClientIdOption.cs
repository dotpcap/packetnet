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