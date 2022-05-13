/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Net;
using System.Net.Sockets;

namespace PacketDotNet.Ndp
{
    public class NdpLinkPrefixInformationOption : NdpOption
    {
        /// <summary>The offset (in bytes) of the Ext field</summary>
        internal const int ExtOffset = PayloadOffset + 1;

        /// <summary>The offset (in bytes) of the PreferredLifetime field</summary>
        internal const int PreferredLifetimeOffset = 8;

        /// <summary>The offset (in bytes) of the Prefix field</summary>
        internal const int PrefixOffset = 16;

        /// <summary>The offset (in bytes) of the ValidLifetime field</summary>
        internal const int ValidLifetimeOffset = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NdpLinkPrefixInformationOption" /> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public NdpLinkPrefixInformationOption(byte[] bytes, int offset, int length) : base(bytes, offset, length)
        { }

        /// <summary>
        /// Gets or sets the autonomous address configuration flag.
        /// </summary>
        public bool AutonomousAddressConfiguration
        {
            get => (OptionData.Bytes[OptionData.Offset + ExtOffset] & 0b_0100_0000) != 0;
            set
            {
                if (value)
                    OptionData.Bytes[OptionData.Offset + ExtOffset] |= 0b_0100_0000;
                else
                    OptionData.Bytes[OptionData.Offset + ExtOffset] = (byte)(OptionData.Bytes[OptionData.Offset + ExtOffset] & ~0b_0100_0000);
            }
        }

        /// <summary>
        /// Gets or sets the on-link flag.
        /// </summary>
        public bool OnLink
        {
            get => (OptionData.Bytes[OptionData.Offset + ExtOffset] & 0b_1000_0000) != 0;
            set
            {
                if (value)
                    OptionData.Bytes[OptionData.Offset + ExtOffset] |= 0b_1000_0000;
                else
                    OptionData.Bytes[OptionData.Offset + ExtOffset] = (byte)(OptionData.Bytes[OptionData.Offset + ExtOffset] & ~0b_1000_0000);
            }
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public IPAddress Prefix
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetworkV6, OptionData.Offset + PrefixOffset, OptionData.Bytes);
            set
            {
                var address = value.GetAddressBytes();

                for (var i = 0; i < address.Length; i++)
                    OptionData.Bytes[OptionData.Offset + PrefixOffset + i] = address[i];
            }
        }

        /// <summary>
        /// Gets or sets the link layer address.
        /// </summary>
        public byte PrefixLength
        {
            get => OptionData.Bytes[OptionData.Offset + PayloadOffset];
            set => OptionData.Bytes[OptionData.Offset + PayloadOffset] = value;
        }
    }
}