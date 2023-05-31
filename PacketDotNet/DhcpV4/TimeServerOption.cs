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

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4;

    public class TimeServerOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerOption" /> class.
        /// </summary>
        /// <param name="timeServers">The time servers.</param>
        public TimeServerOption(List<IPAddress> timeServers) : base(DhcpV4OptionType.TimeServer)
        {
            TimeServers = timeServers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeServerOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer.</param>
        /// <param name="optionLength">The offset.</param>
        public TimeServerOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.TimeServer)
        {
            TimeServers = new List<IPAddress>();

            for (int i = 0; i < optionLength; i += 4)
            {
                if (offset + i + 4 < buffer.Length)
                    TimeServers.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset + i, buffer));
            }
        }

        /// <inheritdoc />
        public override byte[] Data
        {
            get
            {
                var bytes = new byte[Length];
                for (int i = 0; i < TimeServers.Count; i++)
                    Array.Copy(TimeServers[i].GetAddressBytes(), 0, bytes, i * IPv4Fields.AddressLength, IPv4Fields.AddressLength);

                return bytes;
            }
        }

        /// <inheritdoc />
        public override int Length => TimeServers.Count * IPv4Fields.AddressLength;

        /// <summary>
        /// Gets or sets the time servers.
        /// </summary>
        public List<IPAddress> TimeServers { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Time Servers: {String.Join(", ", TimeServers)}";
        }
    }