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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet.DhcpV4
{
    public class DomainNameServerOption : DhcpV4Option
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameServerOption" /> class.
        /// </summary>
        /// <param name="domainNameServers">The domain name servers.</param>
        public DomainNameServerOption(params IPAddress[] domainNameServers) : base(DhcpV4OptionType.DomainServer)
        {
            DomainNameServers = domainNameServers.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameServerOption" /> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="optionLength">Length of the option.</param>
        public DomainNameServerOption(byte[] buffer, int offset, int optionLength) : base(DhcpV4OptionType.DomainServer)
        {
            DomainNameServers = new List<IPAddress>();

            for (int i = 0; i < optionLength; i += 4)
            {
                if (offset + i + 4 < buffer.Length)
                    DomainNameServers.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset + i, buffer));
            }
        }

        /// <inheritdoc />
        public override byte[] Data
        {
            get
            {
                var bytes = new byte[Length];
                for (int i = 0; i < DomainNameServers.Count; i++)
                    Array.Copy(DomainNameServers[i].GetAddressBytes(), 0, bytes, i * IPv4Fields.AddressLength, IPv4Fields.AddressLength);

                return bytes;
            }
        }

        /// <summary>
        /// Gets or sets the domain name servers.
        /// </summary>
        public List<IPAddress> DomainNameServers { get; set; }

        /// <inheritdoc />
        public override int Length => DomainNameServers.Count * IPv4Fields.AddressLength;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Domain Name Servers: {String.Join(", ", DomainNameServers)}";
        }
    }
}