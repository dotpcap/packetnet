/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using PacketDotNet.Utils;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// Base class for IPv4 and IPv6 packets that exports the common
    /// functionality that both of these classes has in common
    /// </summary>
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
        protected int DefaultTimeToLive = 64;

        /// <summary>
        /// The destination address
        /// </summary>
        public abstract IPAddress DestinationAddress { get; set; }

        /// <summary>
        /// ipv4 header length field, calculated for ipv6 packets
        /// NOTE: This field is the number of 32bit words in the ip header,
        /// ie. the number of bytes is 4x this value
        /// </summary>
        public abstract int HeaderLength { get; set; }

        /// <summary>
        /// The number of hops remaining for this packet
        /// Included along side of TimeToLive for user convenience
        /// </summary>
        public virtual int HopLimit
        {
            get => TimeToLive;
            set => TimeToLive = value;
        }

        /// <summary>
        /// ipv6 payload length in bytes,
        /// calculate from ipv4.TotalLength - (ipv4.HeaderLength * 4)
        /// </summary>
        public abstract ushort PayloadLength { get; set; }

        /// <summary>
        /// Payload packet, overridden to set the NextHeader/Protocol based
        /// on the type of payload packet when the payload packet is set
        /// </summary>
        public override Packet PayloadPacket
        {
            get => base.PayloadPacket;
            set
            {
                base.PayloadPacket = value;

                // set NextHeader (Protocol) based on the type of this packet

                Protocol = value switch
                {
                    TcpPacket _ => ProtocolType.Tcp,
                    UdpPacket _ => ProtocolType.Udp,
                    IcmpV6Packet _ => ProtocolType.IcmpV6,
                    IcmpV4Packet _ => ProtocolType.Icmp,
                    IgmpV2Packet _ => ProtocolType.Igmp,
                    OspfPacket _ => ProtocolType.Ospf,
                    _ => ProtocolType.IPv6NoNextHeader
                };

                // update the payload length based on the size
                // of the payload packet
                var newPayloadLength = (ushort) base.PayloadPacket.BytesSegment.Length;
                Log.DebugFormat("newPayloadLength {0}", newPayloadLength);
                PayloadLength = newPayloadLength;
            }
        }

        /// <summary>
        /// The protocol of the ip packet's payload
        /// Named 'Protocol' in IPv4
        /// Named 'NextHeader' in IPv6'
        /// </summary>
        public abstract ProtocolType Protocol { get; set; }

        /// <summary>
        /// The source address
        /// </summary>
        public abstract IPAddress SourceAddress { get; set; }

        /// <summary>
        /// The number of hops remaining before this packet is discarded
        /// Named 'TimeToLive' in IPv4
        /// Named 'HopLimit' in IPv6
        /// </summary>
        public abstract int TimeToLive { get; set; }

        /// <summary>
        /// ipv4 total number of bytes in the ipv4 header + payload,
        /// ipv6 PayloadLength + IPv6Fields.HeaderLength
        /// </summary>
        public abstract int TotalLength { get; set; }

        /// <summary>
        /// The IP version
        /// </summary>
        public abstract IPVersion Version { get; set; }

        /// <summary>
        /// Gets the pseudo ip header.
        /// </summary>
        /// <param name="originalHeaderLength">Length of the original header.</param>
        /// <returns><see cref="byte" />s.</returns>
        internal abstract byte[] GetPseudoIPHeader(int originalHeaderLength);

        /// <summary>
        /// Converts an IP address from a <see cref="byte"/> array.
        /// </summary>
        /// <param name="ipType">A <see cref="AddressFamily" />.</param>
        /// <param name="fieldOffset">A <see cref="int" />.</param>
        /// <param name="bytes">A <see cref="byte" />.</param>
        /// <returns>A <see cref="IPAddress" />.</returns>
        public static IPAddress GetIPAddress
        (
            AddressFamily ipType,
            int fieldOffset,
            byte[] bytes)
        {
            switch (ipType)
            {
                case AddressFamily.InterNetwork:
                {
                    var address = Unsafe.As<byte, long>(ref bytes[fieldOffset]) & 0x00000000FFFFFFFF;
                    return new IPAddress(address);
                }
                case AddressFamily.InterNetworkV6:
                {
#if NETSTANDARD2_1_OR_GREATER
                    return new IPAddress(bytes.AsSpan(fieldOffset, IPv6Fields.AddressLength));
#else
                    var address = new byte[IPv6Fields.AddressLength];

                    Unsafe.WriteUnaligned(ref address[0], Unsafe.As<byte, long>(ref bytes[fieldOffset]));
                    Unsafe.WriteUnaligned(ref address[8], Unsafe.As<byte, long>(ref bytes[fieldOffset + 8]));

                    return new IPAddress(address);
#endif
                }
                default:
                {
                    ThrowHelper.ThrowInvalidAddressFamilyException(ipType);
                    return null;
                }
            }
        }

        /// <summary>
        /// Called by IPv4 and IPv6 packets to parse their packet payload
        /// </summary>
        /// <param name="payload">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="protocolType">
        /// A <see cref="ProtocolType" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment" />
        /// </returns>
        protected static PacketOrByteArraySegment ParseNextSegment
        (
            ByteArraySegment payload,
            ProtocolType protocolType,
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
            else if (parentPacket is IPv6Packet ipv6Packet)
            {
                foreach (var extensionHeader in ipv6Packet.ExtensionHeaders)
                {
                    if (extensionHeader is IPv6FragmentationExtensionHeader fragmentationExtensionHeader && fragmentationExtensionHeader.FragmentOffset > 0)
                    {
                        payloadPacketOrData.ByteArraySegment = payload;
                        return payloadPacketOrData;
                    }
                }
            }

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                {
                    payloadPacketOrData.Packet = new TcpPacket(payload, parentPacket);
                    break;
                }
                case ProtocolType.Udp:
                {
                    payloadPacketOrData.Packet = new UdpPacket(payload, parentPacket);
                    break;
                }
                case ProtocolType.Icmp:
                {
                    payloadPacketOrData.Packet = new IcmpV4Packet(payload, parentPacket);
                    break;
                }
                case ProtocolType.IcmpV6:
                {
                    payloadPacketOrData.Packet = new IcmpV6Packet(payload, parentPacket);
                    break;
                }
                case ProtocolType.Igmp:
                {
                    payloadPacketOrData.Packet = IgmpPacket.ConstructIgmpPacket(payload, parentPacket);
                    break;
                }
                case ProtocolType.Ospf:
                {
                    payloadPacketOrData.Packet = OspfPacket.ConstructOspfPacket(payload.Bytes, payload.Offset);
                    break;
                }
                case ProtocolType.IPv4:
                {
                    payloadPacketOrData.Packet = new IPv4Packet(payload, parentPacket);
                    break;
                }
                case ProtocolType.IPv6:
                {
                    payloadPacketOrData.Packet = new IPv6Packet(payload, parentPacket);
                    break;
                }
                case ProtocolType.Gre:
                {
                    payloadPacketOrData.Packet = new GrePacket(payload, parentPacket);
                    break;
                }
                // NOTE: new payload parsing entries go here
                default:
                {
                    payloadPacketOrData.ByteArraySegment = payload;
                    break;
                }
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
                {
                    return IPv4Packet.RandomPacket();
                }
                case IPVersion.IPv6:
                {
                    return IPv6Packet.RandomPacket();
                }
            }

            throw new InvalidOperationException("Unknown version of " + version);
        }
    }
}
