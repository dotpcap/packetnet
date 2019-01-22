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
using System.Net.Sockets;
using System.Reflection;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
    [Serializable]
    public abstract class IPPacket : InternetPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
                if (value is TcpPacket)
                {
                    Protocol = IPProtocolType.TCP;
                }
                else if (value is UdpPacket)
                {
                    Protocol = IPProtocolType.UDP;
                }
                else if (value is ICMPv6Packet)
                {
                    Protocol = IPProtocolType.ICMPV6;
                }
                else if (value is ICMPv4Packet)
                {
                    Protocol = IPProtocolType.ICMP;
                }
                else if (value is IGMPv2Packet)
                {
                    Protocol = IPProtocolType.IGMP;
                }
                else if (value is OSPFPacket)
                {
                    Protocol = IPProtocolType.OSPF;
                }
                else // NOTE: new checks go here
                {
                    Protocol = IPProtocolType.NONE;
                }

                // update the payload length based on the size
                // of the payload packet
                var newPayloadLength = (UInt16)base.PayloadPacket.BytesHighPerformance.Length;
                Log.DebugFormat("newPayloadLength {0}", newPayloadLength);
                PayloadLength = newPayloadLength;
            }
        }

        /// <value>
        /// The destination address
        /// </value>
        public abstract IPAddress DestinationAddress { get; set; }

        /// <value>
        /// The source address
        /// </value>
        public abstract IPAddress SourceAddress { get; set; }

        /// <value>
        /// The IP version
        /// </value>
        public abstract IPVersion Version { get; set; }

        /// <value>
        /// The protocol of the ip packet's payload
        /// Named 'Protocol' in IPv4
        /// Named 'NextHeader' in IPv6'
        /// </value>
        public abstract IPProtocolType Protocol { get; set; }

        /// <value>
        /// The protocol of the ip packet's payload
        /// Included along side Protocol for user convenience
        /// </value>
        public virtual IPProtocolType NextHeader
        {
            get => Protocol;
            set => Protocol = value;
        }

        /// <value>
        /// The number of hops remaining before this packet is discarded
        /// Named 'TimeToLive' in IPv4
        /// Named 'HopLimit' in IPv6
        /// </value>
        public abstract Int32 TimeToLive { get; set; }

        /// <value>
        /// The number of hops remaining for this packet
        /// Included along side of TimeToLive for user convenience
        /// </value>
        public virtual Int32 HopLimit
        {
            get => TimeToLive;
            set => TimeToLive = value;
        }

        /// <summary>
        /// ipv4 header length field, calculated for ipv6 packets
        /// NOTE: This field is the number of 32bit words in the ip header,
        /// ie. the number of bytes is 4x this value
        /// </summary>
        public abstract Int32 HeaderLength { get; set; }

        /// <summary>
        /// ipv4 total number of bytes in the ipv4 header + payload,
        /// ipv6 PayloadLength + IPv6Fields.HeaderLength
        /// </summary>
        public abstract Int32 TotalLength { get; set; }

        /// <summary>
        /// ipv6 payload length in bytes,
        /// calculate from ipv4.TotalLength - (ipv4.HeaderLength * 4)
        /// </summary>
        public abstract UInt16 PayloadLength { get; set; }

        /// <summary>
        /// Gets the pseudo ip header.
        /// </summary>
        /// <param name="originalHeaderLength">Length of the original header.</param>
        /// <returns><see cref="Byte" />s.</returns>
        internal abstract byte[] GetPseudoIPHeader(int originalHeaderLength);

        /// <summary>
        /// Convert an ip address from a byte[]
        /// </summary>
        /// <param name="ipType">
        /// A <see cref="System.Net.Sockets.AddressFamily" />
        /// </param>
        /// <param name="fieldOffset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <returns>
        /// A <see cref="System.Net.IPAddress" />
        /// </returns>
        public static IPAddress GetIPAddress
        (
            AddressFamily ipType,
            Int32 fieldOffset,
            Byte[] bytes)
        {
            switch (ipType)
            {
                case AddressFamily.InterNetwork:
                {
                    // IPv4: it's possible to avoid a copy by doing the same as IPAddress.
                    // --> m_Address = ((address[3] << 24 | address[2] <<16 | address[1] << 8| address[0]) & 0x0FFFFFFFF);
                    var address = ((bytes[3 + fieldOffset] << 24 | bytes[2 + fieldOffset] << 16 | bytes[1 + fieldOffset] << 8 | bytes[fieldOffset]) & 0x0FFFFFFFF);
                    return new IPAddress(address);
                }
                case AddressFamily.InterNetworkV6:
                {
                    // IPv6: not possible due to not accepting parameters for it.
                    var address = new Byte[IPv6Fields.AddressLength];
                    for (int i = 0; i < IPv6Fields.AddressLength; i++)
                        address[i] = bytes[fieldOffset + i];

                    return new IPAddress(address);
                }
                default:
                    throw new InvalidOperationException("ipType " + ipType + " unknown");
            }
        }
        
        /// <summary>
        /// Called by IPv4 and IPv6 packets to parse their packet payload
        /// </summary>
        /// <param name="payload">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="protocolType">
        /// A <see cref="IPProtocolType" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment" />
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes
        (
            ByteArraySegment payload,
            IPProtocolType protocolType,
            Packet parentPacket)
        {
            Log.DebugFormat("payload: {0}, ParentPacket.GetType() {1}",
                            payload,
                            parentPacket.GetType());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // if we are an ipv4 packet with a non-zero FragmentOffset we shouldn't attempt
            // to decode the content, it is a continuation of a previous packet so it won't
            // have the proper headers for its type, that was in the first packet fragment
            if (parentPacket is IPv4Packet ipv4Packet)
            {
                if (ipv4Packet.FragmentOffset > 0)
                {
                    payloadPacketOrData.ByteArraySegment = payload;
                    return payloadPacketOrData;
                }
            }

            switch (protocolType)
            {
                case IPProtocolType.TCP:
                    payloadPacketOrData.Packet = new TcpPacket(payload,
                                                               parentPacket);
                    break;
                case IPProtocolType.UDP:
                    payloadPacketOrData.Packet = new UdpPacket(payload,
                                                               parentPacket);
                    break;
                case IPProtocolType.ICMP:
                    payloadPacketOrData.Packet = new ICMPv4Packet(payload,
                                                                  parentPacket);
                    break;
                case IPProtocolType.ICMPV6:
                    payloadPacketOrData.Packet = new ICMPv6Packet(payload,
                                                                  parentPacket);
                    break;
                case IPProtocolType.IGMP:
                    payloadPacketOrData.Packet = new IGMPv2Packet(payload,
                                                                  parentPacket);
                    break;
                case IPProtocolType.OSPF:
                    payloadPacketOrData.Packet = OSPFPacket.ConstructOSPFPacket(payload.Bytes,
                                                                                payload.Offset);
                    break;
                case IPProtocolType.IPIP:
                    payloadPacketOrData.Packet = new IPv4Packet(payload,
                                                                parentPacket);
                    break;
                case IPProtocolType.IPV6:
                    payloadPacketOrData.Packet = new IPv6Packet(payload,
                                                                parentPacket);
                    break;
                case IPProtocolType.GRE:
                    payloadPacketOrData.Packet = new GREPacket(payload,
                                                               parentPacket);
                    break;

                // NOTE: new payload parsing entries go here
                default:
                    payloadPacketOrData.ByteArraySegment = payload;
                    break;
            }

            return payloadPacketOrData;
        }

        /// <summary>
        /// Generate a random packet of a specific ip version
        /// </summary>
        /// <param name="version">
        /// A <see cref="IPVersion" />
        /// </param>
        /// <returns>
        /// A <see cref="IPPacket" />
        /// </returns>
        public static IPPacket RandomPacket(IPVersion version)
        {
            Log.DebugFormat("version {0}", version);

            switch (version)
            {
                case IPVersion.IPv4:
                    return IPv4Packet.RandomPacket();
                case IPVersion.IPv6:
                    return IPv6Packet.RandomPacket();
            }

            throw new InvalidOperationException("Unknown version of " + version);
        }
    }
}