/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PacketDotNet.Ndp;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    public class NdpRedirectMessagePacket : NdpPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdpRedirectMessagePacket" /> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public NdpRedirectMessagePacket(ByteArraySegment payload) : base(payload)
        { }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        public IPAddress DestinationAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetworkV6, Header.Offset + NdpFields.RedirectMessageDestinationAddressOffset, Header.Bytes);
            set
            {
                var address = value.GetAddressBytes();

                for (var i = 0; i < address.Length; i++)
                    Header.Bytes[Header.Offset + NdpFields.RedirectMessageDestinationAddressOffset + i] = address[i];
            }
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public List<NdpOption> OptionsCollection
        {
            get => ParseOptions(OptionsSegment);
            set
            {
                var optionsOffset = NdpFields.RedirectMessageOptionsOffset;

                foreach (var option in value)
                {
                    var optionBytes = option.Bytes;
                    Array.Copy(optionBytes, 0, Header.Bytes, Header.Offset + optionsOffset, optionBytes.Length);
                    optionsOffset += optionBytes.Length;
                }
            }
        }

        /// <summary>
        /// Gets the options as a <see cref="ByteArraySegment" />.
        /// </summary>
        public ByteArraySegment OptionsSegment
        {
            get
            {
                var optionsOffset = NdpFields.RedirectMessageOptionsOffset;
                var optionsLength = Header.Length - optionsOffset;

                return new ByteArraySegment(Header.Bytes, Header.Offset + optionsOffset, optionsLength);
            }
        }

        /// <summary>
        /// Gets or sets the target address.
        /// </summary>
        public IPAddress TargetAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetworkV6, Header.Offset + NdpFields.RedirectMessageTargetAddressOffset, Header.Bytes);
            set
            {
                var address = value.GetAddressBytes();

                for (var i = 0; i < address.Length; i++)
                    Header.Bytes[Header.Offset + NdpFields.RedirectMessageTargetAddressOffset + i] = address[i];
            }
        }
    }
}