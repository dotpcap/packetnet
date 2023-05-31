/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace PacketDotNet.Ndp;

    public class NdpLinkLayerAddressOption : NdpOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdpLinkLayerAddressOption" /> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public NdpLinkLayerAddressOption(byte[] bytes, int offset, int length) : base(bytes, offset, length)
        { }

        /// <summary>
        /// Gets or sets the link layer address.
        /// </summary>
        public PhysicalAddress LinkLayerAddress
        {
            get
            {
                var hwAddress = new byte[EthernetFields.MacAddressLength];

                Unsafe.WriteUnaligned(ref hwAddress[0], Unsafe.As<byte, int>(ref OptionData.Bytes[OptionData.Offset + PayloadOffset]));
                Unsafe.WriteUnaligned(ref hwAddress[4], Unsafe.As<byte, short>(ref OptionData.Bytes[OptionData.Offset + PayloadOffset + 4]));

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                Unsafe.WriteUnaligned(ref OptionData.Bytes[OptionData.Offset + PayloadOffset], Unsafe.As<byte, int>(ref hwAddress[0]));
                Unsafe.WriteUnaligned(ref OptionData.Bytes[OptionData.Offset + PayloadOffset + 4], Unsafe.As<byte, short>(ref hwAddress[4]));
            }
        }
    }