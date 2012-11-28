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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

﻿using System;
using System.Net;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
    public abstract class IpPacket : InternetPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// The default time to live value for Ip packets being constructed
        /// </summary>
        protected int DefaultTimeToLive = 64;

        /// <value>
        /// Payload packet, overridden to set the NextHeader/Protocol based
        /// on the type of payload packet when the payload packet is set
        /// </value>
        public override Packet PayloadPacket
        {
            get
            {
                return base.PayloadPacket;
            }

            set
            {
                base.PayloadPacket = value;

                // set NextHeader (Protocol) based on the type of this packet
                if(value is TcpPacket)
                {
                    NextHeader = IPProtocolType.TCP;
                } else if(value is UdpPacket)
                {
                    NextHeader = IPProtocolType.UDP;
                } else if(value is ICMPv6Packet)
                {
                    NextHeader = IPProtocolType.ICMPV6;
                } else if(value is ICMPv4Packet)
                {
                    NextHeader = IPProtocolType.ICMP;
                } else if(value is IGMPv2Packet)
                {
                    NextHeader = IPProtocolType.IGMP;
                } else // NOTE: new checks go here
                {
                    NextHeader = IPProtocolType.NONE;
                }

                // update the payload length based on the size
                // of the payload packet
                var newPayloadLength = (ushort)base.PayloadPacket.Bytes.Length;
                log.DebugFormat("newPayloadLength {0}", newPayloadLength);
                PayloadLength = newPayloadLength;
            }
        }

        /// <value>
        /// The destination address
        /// </value>
        public abstract IPAddress DestinationAddress
        {
            get;
            set;
        }

        /// <value>
        /// The source address
        /// </value>
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
        public virtual IPProtocolType NextHeader
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
        public virtual int HopLimit
        {
            get { return TimeToLive; }
            set { TimeToLive = value; }
        }

        /// <summary>
        /// ipv4 header length field, calculated for ipv6 packets
        /// NOTE: This field is the number of 32bit words in the ip header,
        ///       ie. the number of bytes is 4x this value
        /// </summary>
        public abstract int HeaderLength
        {
            get; set;
        }

        /// <summary>
        /// ipv4 total number of bytes in the ipv4 header + payload,
        /// ipv6 PayloadLength + IPv6Fields.HeaderLength
        /// </summary>
        public abstract int TotalLength
        {
            get; set;
        }

        /// <summary>
        /// ipv6 payload length in bytes,
        /// calculate from ipv4.TotalLength - (ipv4.HeaderLength * 4)
        /// </summary>
        public abstract ushort PayloadLength
        {
            get;
            set;
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

        /// <summary>
        /// Convert an ip address from a byte[]
        /// </summary>
        /// <param name="ipType">
        /// A <see cref="System.Net.Sockets.AddressFamily"/>
        /// </param>
        /// <param name="fieldOffset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.IPAddress"/>
        /// </returns>
        public static System.Net.IPAddress GetIPAddress(System.Net.Sockets.AddressFamily ipType,
                                                        int fieldOffset,
                                                        byte[] bytes)
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
        public IpPacket()
        {}

        /// <summary>
        /// Called by IPv4 and IPv6 packets to parse their packet payload
        /// </summary>
        /// <param name="payload">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="ProtocolType">
        /// A <see cref="IPProtocolType"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment"/>
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment payload,
                                                                        IPProtocolType ProtocolType,
                                                                        Packet ParentPacket)
        {
            log.DebugFormat("payload: {0}, ParentPacket.GetType() {1}",
                            payload,
                            ParentPacket.GetType());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch(ProtocolType)
            {
            case IPProtocolType.TCP:
                payloadPacketOrData.ThePacket = new TcpPacket(payload,
                                                              ParentPacket);
                break;
            case IPProtocolType.UDP:
                payloadPacketOrData.ThePacket = new UdpPacket(payload,
                                                              ParentPacket);
                break;
            case IPProtocolType.ICMP:
                payloadPacketOrData.ThePacket = new ICMPv4Packet(payload,
                                                                 ParentPacket);
                break;
            case IPProtocolType.ICMPV6:
                payloadPacketOrData.ThePacket = new ICMPv6Packet(payload,
                                                                 ParentPacket);
                break;
            case IPProtocolType.IGMP:
                payloadPacketOrData.ThePacket = new IGMPv2Packet(payload,
                                                                 ParentPacket);
                break;
            // NOTE: new payload parsing entries go here
            default:
                payloadPacketOrData.TheByteArraySegment = payload;
                break;
            }

            return payloadPacketOrData;
        }

        /// <summary>
        /// Returns the IpPacket inside of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="IpPacket"/>
        /// </returns>
        [Obsolete("Use Packet.Extract() instead")]
        public static IpPacket GetEncapsulated(Packet p)
        {
            log.Debug("");

            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    return (IpPacket)payload;
                }
            }

            return null;
        }

        /// <summary>
        /// Generate a random packet of a specific ip version
        /// </summary>
        /// <param name="version">
        /// A <see cref="IpVersion"/>
        /// </param>
        /// <returns>
        /// A <see cref="IpPacket"/>
        /// </returns>
        public static IpPacket RandomPacket(IpVersion version)
        {
            log.DebugFormat("version {0}", version);

            if(version == IpVersion.IPv4)
            {
                return IPv4Packet.RandomPacket();
            } else if(version == IpVersion.IPv6)
            {
                return IPv6Packet.RandomPacket();
            } else
            {
                throw new System.InvalidOperationException("Unknown version of " + version);
            }
        }
    }
}
