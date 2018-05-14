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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Additional TOS-specific information  for backward compatibility
    /// with previous versions of the OSPF specification
    /// </summary>
    public struct TOSMetric
    {
        ///<summary>The number of bytes a TOS metric occupy</summary>
        public static readonly Int32 TOSMetricLength = 4;

        /// <summary>
        /// IP Type of Service that this metric refers to.
        /// </summary>
        public Byte TOS;

        /// <summary>
        /// TOS-specific metric information.
        /// </summary>
        public UInt32 Metric;

        /// <summary>
        /// Gets the bytes that make up this packet.
        /// </summary>
        /// <value>Packet bytes</value>
        public Byte[] Bytes
        {
            get
            {
                Byte[] b = new Byte[TOSMetricLength];
                EndianBitConverter.Big.CopyBytes(Metric, b, 0);
                b[0] = TOS;
                return b;
            }
        }
    }

    /// <summary>
    /// Link state request, send by the LSR packets
    /// </summary>
    public class LinkStateRequest
    {
        /// <summary>
        /// Size of LinkStateRequest in bytes
        /// </summary>
        public static readonly Int32 Length = 12;

        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkStateRequest()
        {
            Byte[] b = new Byte[Length];
            header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset abd length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public LinkStateRequest(Byte[] packet, Int32 offset, Int32 length)
        {
            header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSR.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + LinkStateRequestFields.AdvertisingRouterPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + LinkStateRequestFields.AdvertisingRouterPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual Byte[] Bytes => header.ActualBytes();

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSR.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + LinkStateRequestFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + LinkStateRequestFields.LinkStateIdPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The type of the request
        /// </summary>
        public LSAType LSType
        {
            get => (LSAType) EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + LinkStateRequestFields.LSTypePosition);
            set => EndianBitConverter.Big.CopyBytes((UInt32) value, header.Bytes, header.Offset + LinkStateRequestFields.LSTypePosition);
        }
    }


    /// <summary>
    /// The LSA header. All LSAs begin with a common 20 byte header.  This header contains
    /// enough information to uniquely identify the LSA (LS type, Link State
    /// ID, and Advertising Router). See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class LSA
    {
        /// <summary>
        /// The I pv4 bytes count.
        /// </summary>
        public const Int32 IPv4BytesCount = 4;

        /// <summary>
        /// The length of the network mask.
        /// </summary>
        public const Int32 NetworkMaskLength = 4;

        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LSA()
        {
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderLength];
            header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public LSA(Byte[] packet, Int32 offset, Int32 length)
        {
            header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSA.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + LSAFields.AdvertisingRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + LSAFields.AdvertisingRouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual Byte[] Bytes => header.ActualBytes();

        /// <summary>
        /// The Fletcher checksum of the complete contents of the LSA,
        /// including the LSA header but excluding the LS age field.
        /// </summary>
        public UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + LSAFields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + LSAFields.ChecksumPosition);
        }

        /// <summary>
        /// The length in bytes of the LSA.  This includes the 20 byte LSA
        /// header.
        /// </summary>
        public UInt16 Length
        {
            get => EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + LSAFields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + LSAFields.PacketLengthPosition);
        }

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSA.  The contents of this field
        /// depend on the LSA's LS type.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + LSAFields.LinkStateIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + LSAFields.LinkStateIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The time in seconds since the LSA was originated.
        /// </summary>
        public UInt16 LSAge
        {
            get => EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + LSAFields.LSAgePosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + LSAFields.LSAgePosition);
        }

        /// <summary>
        /// Detects old or duplicate LSAs.  Successive instances of an LSA
        /// are given successive LS sequence numbers.
        /// </summary>
        public UInt32 LSSequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + LSAFields.LSSequenceNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + LSAFields.LSSequenceNumberPosition);
        }

        ///<summary>
        ///The type of the LSA.  Each LSA type has a separate advertisement format.
        ///</summary>
        public LSAType LSType
        {
            get => (LSAType) header.Bytes[header.Offset + LSAFields.LSTypePosition];
            set => header.Bytes[header.Offset + LSAFields.LSTypePosition] = (Byte) value;
        }

        /// <summary>
        /// The optional capabilities supported by the described portion of the routing domain.
        /// </summary>
        public Byte Options
        {
            get => header.Bytes[header.Offset + LSAFields.OptionsPosition];
            set => header.Bytes[header.Offset + LSAFields.OptionsPosition] = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.LSA" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.LSA" />.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("LSA Type {0}, Checksum {1:X2}\n", LSType, Checksum);
            return builder.ToString();
        }
    }

    /// <summary>
    /// Router link contained in a RouterLSA.
    /// </summary>
    public class RouterLink
    {
        /// <summary>
        /// The length of the router link.
        /// </summary>
        public static readonly Int32 RouterLinkLength = 12;

        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLink()
        {
            Byte[] b = new Byte[RouterLinkLength];
            header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs router link from a list of TOS metrics
        /// </summary>
        public RouterLink(List<TOSMetric> metrics)
        {
            Int32 length = RouterLinkLength + metrics.Count * TOSMetric.TOSMetricLength;
            Int32 offset = RouterLinkFields.AdditionalMetricsPosition;
            Byte[] b = new Byte[length];

            foreach (TOSMetric m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
            }

            header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public RouterLink(Byte[] packet, Int32 offset, Int32 length)
        {
            header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// bytes representation
        /// </summary>
        public Byte[] Bytes => header.Bytes;

        /// <summary>
        /// Value again depends on the link's Type field. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public IPAddress LinkData
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + RouterLinkFields.LinkDataPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + RouterLinkFields.LinkDataPosition,
                           address.Length);
            }
        }


        /// <summary>
        /// Identifies the object that this router link connects to.  Value
        /// depends on the link's Type.
        /// </summary>
        public IPAddress LinkID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + RouterLinkFields.LinkIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + RouterLinkFields.LinkIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The cost of using this router link.
        /// </summary>
        public UInt16 Metric
        {
            get => EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + RouterLinkFields.MetricPosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + RouterLinkFields.MetricPosition);
        }

        /// <summary>
        /// List of TOS metrics, contained in this LSA. Deprecated by RFC 4915
        /// </summary>
        public List<TOSMetric> TOSMetrics
        {
            get
            {
                List<TOSMetric> metrics = new List<TOSMetric>();

                for (Int32 i = 0; i < TOSNumber; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + RouterLinkFields.AdditionalMetricsPosition + i * TOSMetric.TOSMetricLength);
                    TOSMetric m = new TOSMetric();
                    m.TOS = (Byte) ((metric & 0xFF000000) >> 3);
                    m.Metric = metric & 0x00FFFFFF;
                    metrics.Add(m);
                }

                return metrics;
            }
        }

        /// <summary>
        /// The number of different TOS metrics given for this link, not
        /// counting the required link metric
        /// </summary>
        public Byte TOSNumber
        {
            get => header.Bytes[header.Offset + RouterLinkFields.TOSNumberPosition];
            set => header.Bytes[header.Offset + RouterLinkFields.TOSNumberPosition] = value;
        }

        /// <summary>
        /// A quick description of the router link. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public Byte Type
        {
            get => header.Bytes[header.Offset + RouterLinkFields.TypePosition];
            set => header.Bytes[header.Offset + RouterLinkFields.TypePosition] = value;
        }
    }

    /// <summary>
    /// Router-LSAs are the Type 1 LSAs. The LSA describes the state and cost of
    /// the router's links (i.e., interfaces) to the area.
    /// </summary>
    public class RouterLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Router;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLSA()
        {
            Byte[] b = new Byte[RouterLSAFields.RouterLinksStart];
            header = new ByteArraySegment(b);
            LSType = lsaType;
            LinkNumber = 0;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Router LSA with a list of router links
        /// </summary>
        public RouterLSA(List<RouterLink> links)
        {
            Int32 length = 0;
            Int32 offset = RouterLSAFields.RouterLinksStart;
            foreach (RouterLink l in links)
            {
                length += l.Bytes.Length;
            }

            length += RouterLSAFields.RouterLinksStart;

            Byte[] b = new Byte[length];
            header = new ByteArraySegment(b);
            foreach (RouterLink l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, l.Bytes.Length);
                offset += l.Bytes.Length;
            }

            LSType = lsaType;
            LinkNumber = (UInt16) links.Count;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public RouterLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// When set, the router is an area border router
        /// </summary>
        public Int32 bBit
        {
            get
            {
                Byte flags = (Byte) (header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] & 1);
                return flags;
            }
            set => header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] |= (Byte) (value & 1);
        }

        /// <summary>
        /// When set, the router is an AS boundary router
        /// </summary>
        public Int32 eBit
        {
            get
            {
                Byte flags = (Byte) ((header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] >> 1) & 1);
                return flags;
            }
            set => header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] |= (Byte) ((value & 1) << 1);
        }

        /// <summary>
        /// The number of the contained links in this RouterLSA
        /// </summary>
        public UInt16 LinkNumber
        {
            get => EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + RouterLSAFields.LinkNumberPosition);

            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + RouterLSAFields.LinkNumberPosition);
        }

        /// <summary>
        /// The contained router links in this RouterLSA
        /// </summary>
        /// <see cref="PacketDotNet.RouterLink" />
        public List<RouterLink> RouterLinks
        {
            get
            {
                List<RouterLink> ret = new List<RouterLink>();

                Int32 offset = header.Offset + RouterLSAFields.RouterLinksStart;

                for (Int32 i = 0; i < LinkNumber; i++)
                {
                    RouterLink l = new RouterLink(header.Bytes, offset, RouterLink.RouterLinkLength);
                    ret.Add(l);
                    offset += RouterLink.RouterLinkLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// When set, the router is an endpoint of one or more fully
        /// adjacent virtual links having the described area as Transit area
        /// </summary>
        public Int32 vBit
        {
            get
            {
                Byte flags = (Byte) ((header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] >> 2) & 1);
                return flags;
            }
            set => header.Bytes[header.Offset + RouterLSAFields.RouterOptionsPosition] |= (Byte) ((value & 1) << 2);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.RouterLSA" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.RouterLSA" />.</returns>
        public override String ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("Router LSA, ");
            ret.AppendFormat("V: {0}, E: {1}, B: {2}, ", vBit, eBit, bBit);
            ret.AppendFormat("links#: {0}", LinkNumber);
            return ret.ToString();
        }
    }


    /// <summary>
    /// Network-LSAs are the Type 2 LSAs. The LSA describes all routers
    /// attached to the network, including the Designated Router itself.
    /// </summary>
    public class NetworkLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Network;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NetworkLSA()
        {
            Byte[] b = new Byte[NetworkLSAFields.AttachedRouterPosition];
            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Network LSA with a list of attached routers
        /// </summary>
        public NetworkLSA(List<IPAddress> routers)
        {
            Int32 length = NetworkLSAFields.AttachedRouterPosition + routers.Count * IPv4BytesCount;
            Int32 offset = NetworkLSAFields.AttachedRouterPosition;

            Byte[] b = new Byte[length];
            foreach (IPAddress ip in routers)
            {
                Array.Copy(ip.GetAddressBytes(), 0, b, offset, IPv4BytesCount);
                offset += IPv4BytesCount;
            }

            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public NetworkLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The Router IDs of each of the routers attached to the network.
        /// Actually, only those routers that are fully adjacent to the
        /// Designated Router are listed.  The Designated Router includes
        /// itself in this list.  The number of routers included can be
        /// deduced from the LSA header's length field.
        /// </summary>
        public List<IPAddress> AttachedRouters
        {
            get
            {
                List<IPAddress> ret = new List<IPAddress>();
                Int32 routerCount = Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength;
                if (routerCount % IPv4BytesCount != 0)
                {
                    throw new Exception("Mallformed NetworkLSA - routerCount should be aligned to 4");
                }

                routerCount /= IPv4BytesCount;

                for (Int32 i = 0; i < routerCount; i++)
                {
                    Byte[] adr = new Byte[IPv4BytesCount];
                    Array.Copy(header.Bytes, header.Offset + NetworkLSAFields.AttachedRouterPosition + i * IPv4BytesCount, adr, 0, IPv4BytesCount);
                    IPAddress ip = new IPAddress(adr);
                    ret.Add(ip);
                }

                return ret;
            }
        }

        /// <summary>
        /// The IP address mask for the network.  For example, a class A
        /// network would have the mask 0xff000000 (255.0.0.0).
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + NetworkLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + NetworkLSAFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }

    /// <summary>
    /// Summary-LSAs are the Type 3 and 4 LSAs.  These LSAs are originated
    /// by area border routers. Summary-LSAs describe inter-area
    /// destinations. Type 3 summary-LSAs are used when the destination is an IP network,
    /// Type 4 - an AS boundary router.
    /// </summary>
    public class SummaryLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Summary;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryLSA()
        {
            Byte[] b = new Byte[SummaryLSAFields.TOSMetricPosition];
            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Summary LSA with a list of TOS metrics
        /// </summary>
        public SummaryLSA(List<TOSMetric> metrics)
        {
            Int32 length = SummaryLSAFields.TOSMetricPosition + metrics.Count * TOSMetric.TOSMetricLength;
            Int32 offset = SummaryLSAFields.TOSMetricPosition;
            Byte[] b = new Byte[length];

            foreach (TOSMetric m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
            }

            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public SummaryLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The cost of this route.  Expressed in the same units as the interface costs in the router-LSAs.
        /// </summary>
        public UInt32 Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + SummaryLSAFields.MetricPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var theValue = value & 0x00FFFFFF;
                EndianBitConverter.Big.CopyBytes(theValue, header.Bytes, header.Offset + SummaryLSAFields.MetricPosition);
            }
        }

        /// <summary>
        /// For Type 3 summary-LSAs, this indicates the destination
        /// network's IP address mask. This field is not meaningful
        /// and must be zero for Type 4 summary-LSAs.
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + SummaryLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + SummaryLSAFields.NetworkMaskPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Additional TOS-specific information  for backward compatibility
        /// with previous versions of the OSPF specification
        /// </summary>
        public List<TOSMetric> TOSMetrics
        {
            get
            {
                List<TOSMetric> ret = new List<TOSMetric>();

                if ((Length - SummaryLSAFields.TOSMetricPosition) % TOSMetric.TOSMetricLength != 0)
                {
                    throw new Exception("Malformed summary LSA - bad TOSMetrics size");
                }

                Int32 tosCnt = (Length - SummaryLSAFields.TOSMetricPosition) / TOSMetric.TOSMetricLength;

                for (Int32 i = 0; i < tosCnt; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + SummaryLSAFields.TOSMetricPosition + i * TOSMetric.TOSMetricLength);
                    TOSMetric m = new TOSMetric();
                    m.TOS = (Byte) ((metric & 0xFF000000) >> 24);
                    m.Metric = metric & 0x00FFFFFF;
                    ret.Add(m);
                }

                return ret;
            }
        }
    }

    /// <summary>
    /// Describes a particular external destination
    /// </summary>
    public class ASExternalLink
    {
        /// <summary>
        /// The length.
        /// </summary>
        public static readonly Int32 Length = 12;

        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLink()
        {
            Byte[] b = new Byte[Length];
            header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public ASExternalLink(Byte[] packet, Int32 offset, Int32 length)
        {
            header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// Bytes representation
        /// </summary>
        public Byte[] Bytes => header.Bytes;

        /// <summary>
        /// The type of external metric.  If bit E is set, the metric
        /// specified is a Type 2 external metric.
        /// </summary>
        public Byte eBit
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                return (Byte) ((val >> 31) & 0xFF);
            }
            set
            {
                UInt32 original = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                UInt32 val = (UInt32) ((value & 1) << 31) | original;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// A 32-bit field attached to each external route.  This is not used by the OSPF protocol itself.
        /// </summary>
        public UInt32 ExternalRouteTag
        {
            get => EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
            set => EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
        }

        /// <summary>
        /// Data traffic for the advertised destination will be forwarded to this address.
        /// </summary>
        public IPAddress ForwardingAddress
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.ForwardingAddressPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + ASExternalLinkFields.ForwardingAddressPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The cost of this route.  Interpretation depends on the external
        /// type indication (bit E above).
        /// </summary>
        public UInt32 Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                UInt32 original = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                var val = value & 0x00FFFFFF | original;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// The Type of Service that the following fields concern.
        /// </summary>
        public Byte TOS
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                return (Byte) ((val >> 24) & 0x7F);
            }
            set
            {
                UInt32 original = EndianBitConverter.Big.ToUInt32(header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
                var val = (Byte) ((value & 0x7F) << 24) | original;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }
    }

    /// <summary>
    /// AS-external-LSAs are the Type 5 LSAs.  These LSAs are originated by
    /// AS boundary routers, and describe destinations external to the AS.
    /// </summary>
    public class ASExternalLSA : LSA
    {
        const Int32 ASExternalLinkLength = 12;

        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.ASExternal;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLSA()
        {
            Byte[] b = new Byte[ASExternalLSAFields.MetricPosition];
            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an ASExternalLSA with a list of ASExternalLinks
        /// </summary>
        public ASExternalLSA(List<ASExternalLink> links)
        {
            Int32 length = ASExternalLSAFields.MetricPosition + ASExternalLink.Length * links.Count;
            Int32 offset = ASExternalLSAFields.MetricPosition;
            Byte[] b = new Byte[length];

            foreach (ASExternalLink l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, ASExternalLink.Length);
                offset += ASExternalLink.Length;
            }

            header = new ByteArraySegment(b);
            LSType = lsaType;
            Length = (UInt16) header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public ASExternalLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The list of the AS-External-Links contained in this OSPF packet.
        /// </summary>
        public List<ASExternalLink> ASExternalLinks
        {
            get
            {
                Int32 linkCnt = (Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength) / ASExternalLinkLength;
                List<ASExternalLink> ret = new List<ASExternalLink>(linkCnt);
                for (Int32 i = 0; i < linkCnt; i++)
                {
                    ASExternalLink l = new ASExternalLink(header.Bytes,
                                                          header.Offset + ASExternalLSAFields.MetricPosition + i * ASExternalLink.Length,
                                                          ASExternalLink.Length);
                    ret.Add(l);
                }

                return ret;
            }
        }

        /// <summary>
        /// The IP address mask for the advertised destination.
        /// E.g. class A network - 255.0.0.0
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + ASExternalLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           header.Bytes,
                           header.Offset + ASExternalLSAFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }
}