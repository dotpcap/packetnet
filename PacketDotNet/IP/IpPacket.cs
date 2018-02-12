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

using System;
using System.Net;
using PacketDotNet.GRE;
using PacketDotNet.ICMP;
using PacketDotNet.IGMP;
using PacketDotNet.OSPF;
using PacketDotNet.Tcp;
using PacketDotNet.Udp;
using PacketDotNet.Utils;

namespace PacketDotNet.IP
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
    [Serializable]
    public abstract class IpPacket : InternetPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// The default time to live value for Ip packets being constructed
        /// </summary>
        protected Int32 DefaultTimeToLive = 64;

        /// <value>
        /// Payload packet, overridden to set the NextHeader/Protocol based
        /// on the type of payload packet when the payload packet is set
        /// </value>
        public override Packet PayloadPacket
        {
            get => base.PayloadPacket;

            set
            {
                base.PayloadPacket = value;

                // set NextHeader (Protocol) based on the type of this packet
                switch (value)
                {
                    case TcpPacket _:
                        this.NextHeader = IPProtocolType.TCP;
                        break;
                    case UdpPacket _:
                        this.NextHeader = IPProtocolType.UDP;
                        break;
                    case ICMPv6Packet _:
                        this.NextHeader = IPProtocolType.ICMPV6;
                        break;
                    case ICMPv4Packet _:
                        this.NextHeader = IPProtocolType.ICMP;
                        break;
                    case IGMPv2Packet _:
                        this.NextHeader = IPProtocolType.IGMP;
                        break;
                    case OSPFPacket _:
                        this.NextHeader = IPProtocolType.OSPF;
                        break;
                    default:
                        this.NextHeader = IPProtocolType.NONE;
                        break;
                }

                // update the payload length based on the size
                // of the payload packet
                var newPayloadLength = (UInt16)base.PayloadPacket.Bytes.Length;
                Log.DebugFormat("newPayloadLength {0}", newPayloadLength);
                this.PayloadLength = newPayloadLength;
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
            get => this.Protocol;
            set => this.Protocol = value;
        }

        /// <value>
        /// The number of hops remaining before this packet is discarded
        /// Named 'TimeToLive' in IPv4
        /// Named 'HopLimit' in IPv6
        /// </value>
        public abstract Int32 TimeToLive
        {
            get;
            set;
        }

        /// <value>
        /// The number of hops remaining for this packet
        /// Included along side of TimeToLive for user convienence
        /// </value>
        public virtual Int32 HopLimit
        {
            get => this.TimeToLive;
            set => this.TimeToLive = value;
        }

        /// <summary>
        /// ipv4 header length field, calculated for ipv6 packets
        /// NOTE: This field is the number of 32bit words in the ip header,
        ///       ie. the number of bytes is 4x this value
        /// </summary>
        public abstract Int32 HeaderLength
        {
            get; set;
        }

        /// <summary>
        /// ipv4 total number of bytes in the ipv4 header + payload,
        /// ipv6 PayloadLength + IPv6Fields.HeaderLength
        /// </summary>
        public abstract Int32 TotalLength
        {
            get; set;
        }

        /// <summary>
        /// ipv6 payload length in bytes,
        /// calculate from ipv4.TotalLength - (ipv4.HeaderLength * 4)
        /// </summary>
        public abstract UInt16 PayloadLength
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
        internal abstract Byte[] AttachPseudoIPHeader(Byte[] origHeader);

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
        public static IPAddress GetIPAddress(System.Net.Sockets.AddressFamily ipType,
                                                        Int32 fieldOffset,
                                                        Byte[] bytes)
        {
            Byte[] address;
            switch (ipType)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    address = new Byte[IPv4Fields.AddressLength];
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    address = new Byte[IPv6Fields.AddressLength];
                    break;
                default:
                    throw new InvalidOperationException("ipType " + ipType + " unknown");
            }

            Array.Copy(bytes, fieldOffset,
                              address, 0, address.Length);

            return new IPAddress(address);
        }

        /// <summary>
        /// IpPacket constructor
        /// </summary>
        protected IpPacket()
        {}

        /// <summary>
        /// Called by IPv4 and IPv6 packets to parse their packet payload
        /// </summary>
        /// <param name="payload">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="protocolType">
        /// A <see cref="IPProtocolType"/>
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment"/>
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment payload,
                                                                        IPProtocolType protocolType,
                                                                        Packet parentPacket)
        {
            Log.DebugFormat("payload: {0}, ParentPacket.GetType() {1}",
                            payload,
                            parentPacket.GetType());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // if we are an ipv4 packet with a non-zero FragementOffset we shouldn't attempt
            // to decode the content, it is a continuation of a previous packet so it won't
            // have the proper headers for its type, that was in the first packet fragment
            var ipv4Packet = parentPacket.Extract(typeof(IPv4Packet)) as IPv4Packet;
            if (ipv4Packet?.FragmentOffset > 0)
            {
                payloadPacketOrData.TheByteArraySegment = payload;
                return payloadPacketOrData;
            }

            switch(protocolType)
            {
            case IPProtocolType.TCP:
                payloadPacketOrData.ThePacket = new TcpPacket(payload,
                                                              parentPacket);
                break;
            case IPProtocolType.UDP:
                payloadPacketOrData.ThePacket = new UdpPacket(payload,
                                                              parentPacket);
                break;
            case IPProtocolType.ICMP:
                payloadPacketOrData.ThePacket = new ICMPv4Packet(payload,
                                                                 parentPacket);
                break;
            case IPProtocolType.ICMPV6:
                payloadPacketOrData.ThePacket = new ICMPv6Packet(payload,
                                                                 parentPacket);
                break;
            case IPProtocolType.IGMP:
                payloadPacketOrData.ThePacket = new IGMPv2Packet(payload,
                                                                 parentPacket);
                break;
            case IPProtocolType.OSPF:
                payloadPacketOrData.ThePacket = OSPFPacket.ConstructOSPFPacket(payload.Bytes,
                                                                               payload.Offset);
                break;                    
            case IPProtocolType.IPIP:
                payloadPacketOrData.ThePacket = new IPv4Packet(payload,
                                                               parentPacket);
                break;
            case IPProtocolType.IPV6:
                payloadPacketOrData.ThePacket = new IPv6Packet(payload,
                                                               parentPacket);
                break;
            case IPProtocolType.GRE:
                payloadPacketOrData.ThePacket = new GREPacket(payload,
                                                                parentPacket);
                break;
                    
                case IPProtocolType.IP:
                case IPProtocolType.EGP:
                case IPProtocolType.PUP:
                case IPProtocolType.IDP:
                case IPProtocolType.TP:
                case IPProtocolType.ROUTING:
                case IPProtocolType.FRAGMENT:
                case IPProtocolType.RSVP:
                case IPProtocolType.ESP:
                case IPProtocolType.AH:
                case IPProtocolType.NONE:
                case IPProtocolType.DSTOPTS:
                case IPProtocolType.MTP:
                case IPProtocolType.ENCAP:
                case IPProtocolType.PIM:
                case IPProtocolType.COMP:
                case IPProtocolType.RAW:
                default:
                payloadPacketOrData.TheByteArraySegment = payload;
                break;
            }

            return payloadPacketOrData;
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
            Log.DebugFormat("version {0}", version);

            switch (version)
            {
                case IpVersion.IPv4:
                    return IPv4Packet.RandomPacket();
                case IpVersion.IPv6:
                    return IPv6Packet.RandomPacket();
                default:
                    throw new InvalidOperationException("Unknown version of " + version);
            }
        }
    }
}
