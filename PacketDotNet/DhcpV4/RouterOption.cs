/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

    public class RouterOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouterOption" /> class.
        /// </summary>
        /// <param name="routers">The routers.</param>
        public RouterOption(params IPAddress[] routers) : base(DhcpV4OptionType.Router)
        {
            Routers = routers.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public RouterOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.Router)
        {
            Routers = new List<IPAddress>();

            for (int i = 0; i < optionLength; i += 4)
            {
                if (offset + i + 4 < buffer.Length)
                    Routers.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset + i, buffer));
            }
        }

        /// <inheritdoc />
        public override byte[] Data
        {
            get
            {
                var bytes = new byte[Length];
                for (int i = 0; i < Routers.Count; i++)
                    Array.Copy(Routers[i].GetAddressBytes(), 0, bytes, i * IPv4Fields.AddressLength, IPv4Fields.AddressLength);

                return bytes;
            }
        }

        /// <inheritdoc />
        public override int Length => Routers.Count * IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the routers.
        /// </summary>
        public List<IPAddress> Routers { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Routers: {String.Join(", ", Routers)}";
        }
    }