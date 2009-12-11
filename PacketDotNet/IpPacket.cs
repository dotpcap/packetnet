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

﻿using System;
using System.Net;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
    public abstract class IpPacket : InternetPacket
    {
        public abstract IPAddress DestinationAddress
        {
            get;
            set;
        }

        public abstract IPAddress SourceAddress
        {
            get;
            set;
        }

        /// <value>
        /// The IP version
        /// </value>
        public abstract IpVersion Version
        {
            get;
            set;
        }

        /// <value>
        /// The protocol of the ip packet's payload
        /// Named 'Protocol' in IPv4
        /// Named 'NextHeader' in IPv6'
        /// </value>
        public abstract IPProtocolType Protocol
        {
            get;
            set;
        }

        /// <value>
        /// The protocol of the ip packet's payload
        /// Included along side Protocol for user convienence
        /// </value>
        public IPProtocolType NextHeader
        {
            get { return Protocol; }
            set { Protocol = value; }
        }

        /// <value>
        /// The number of hops remaining before this packet is discarded
        /// Named 'TimeToLive' in IPv4
        /// Named 'HopLimit' in IPv6
        /// </value>
        public abstract int TimeToLive
        {
            get;
            set;
        }

        /// <value>
        /// The number of hops remaining for this packet
        /// Included along side of TimeToLive for user convienence
        /// </value>
        public int HopLimit
        {
            get { return TimeToLive; }
            set { TimeToLive = value; }
        }

        /// <summary>
        /// Adds a pseudo ip header to a given packet. Used to generate the full
        /// byte array required to generate a udp or tcp checksum.
        /// </summary>
        /// <param name="origHeader">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte"/>
        /// </returns>
        internal abstract byte[] AttachPseudoIPHeader(byte[] origHeader);

        public static System.Net.IPAddress GetIPAddress(System.Net.Sockets.AddressFamily ipType, int fieldOffset, byte[] bytes)
        {
            byte[] address;
            if(ipType == System.Net.Sockets.AddressFamily.InterNetwork) // ipv4
            {
                address = new byte[IPv4Fields.AddressLength];
            } else if(ipType == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                address = new byte[IPv6Fields.AddressLength];
            } else
            {
                throw new System.InvalidOperationException("ipType " + ipType + " unknown");
            }

            System.Array.Copy(bytes, fieldOffset,
                              address, 0, address.Length);

            return new System.Net.IPAddress(address);
        }

        /// <summary>
        /// IpPacket constructor 
        /// </summary>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public IpPacket(PosixTimeval Timeval) : base(Timeval)
        {}

        /// <summary>
        /// Called by IPv4 and IPv6 packets to parse their packet payload 
        /// </summary>
        /// <param name="Header">
        /// A <see cref="ByteArrayAndOffset"/>
        /// </param>
        /// <param name="ProtocolType">
        /// A <see cref="IPProtocolType"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArray"/>
        /// </returns>
        internal static PacketOrByteArray ParseEncapsulatedBytes(ByteArrayAndOffset Header,
                                               IPProtocolType ProtocolType,
                                               PosixTimeval Timeval)
        {
            // slice off the payload
            var payload = Header.EncapsulatedBytes();

            var payloadPacketOrData = new PacketOrByteArray();

            switch(ProtocolType)
            {
            case IPProtocolType.TCP:
                payloadPacketOrData.ThePacket = new TcpPacket(payload.Bytes, payload.Offset, Timeval);
                break;
            case IPProtocolType.UDP:
                payloadPacketOrData.ThePacket = new UdpPacket(payload.Bytes, payload.Offset, Timeval);
                break;
            /// NOTE: new payload parsing entries go here
            default:
                payloadPacketOrData.TheByteArray = payload;
                break;
            }

            return payloadPacketOrData;
        }
    }
}
