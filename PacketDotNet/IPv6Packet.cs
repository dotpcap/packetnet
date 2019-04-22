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
 * Copyright 2009 David Bond <mokon@mokon.net>
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// IPv6 packet
    /// References
    /// ----------
    /// http://tools.ietf.org/html/rfc2460
    /// http://en.wikipedia.org/wiki/IPv6
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public sealed class IPv6Packet : IPPacket
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

        /// <value>
        /// The version of the IP protocol. The '6' in IPv6 indicates the version of the protocol
        /// </value>
        public static IPVersion IPVersion = IPVersion.IPv6;

        private static readonly HashSet<ProtocolType> ExtensionHeaderTypes = new HashSet<ProtocolType>
        {
            ProtocolType.IPv6HopByHopOptions,
            ProtocolType.IPv6DestinationOptions,
            ProtocolType.IPv6RoutingHeader,
            ProtocolType.IPv6FragmentHeader,
            ProtocolType.IPSecAuthenticationHeader,
            ProtocolType.Encapsulation,
            ProtocolType.IPv6DestinationOptions,
            ProtocolType.MobilityHeader,
            ProtocolType.HostIdentity,
            ProtocolType.Shim6,
            ProtocolType.Reserved253,
            ProtocolType.Reserved254
        };

        private List<IPv6ExtensionHeader> _extensionHeaders;

        /// <value>
        /// The position of the byte the contains the encapsulated protocol
        /// </value>
        private int _protocolOffset;

        /// <value>
        /// The length of all the extension headers
        /// </value>
        private int _totalExtensionHeadersLength = -1;

        /// <summary>
        /// Create an IPv6 packet from values
        /// </summary>
        /// <param name="sourceAddress">
        /// A <see cref="IPAddress" />
        /// </param>
        /// <param name="destinationAddress">
        /// A <see cref="IPAddress" />
        /// </param>
        public IPv6Packet
        (
            IPAddress sourceAddress,
            IPAddress destinationAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = IPv6Fields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            // set some default values to make this packet valid
            PayloadLength = 0;
            TimeToLive = DefaultTimeToLive;

            // set instance values
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            Version = IPVersion;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IPv6Packet(ByteArraySegment byteArraySegment)
        {
            Log.Debug(byteArraySegment.ToString());

            // IPv6 headers have a fixed length.
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = IPv6Fields.HeaderLength
            };

            Log.DebugFormat("PayloadLength: {0}", PayloadLength);

            if (PayloadLength > 0)
            {
                // parse the payload
                PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
                                                                         {
                                                                             if (_totalExtensionHeadersLength == -1)
                                                                                 CalculateExtensionHeaders();

                                                                             var startingOffset = Header.Offset + Header.Length + _totalExtensionHeadersLength;
                                                                             var segmentLength = Math.Min(PayloadLength, Header.BytesLength - startingOffset);
                                                                             var bytesLength = startingOffset + segmentLength;

                                                                             var payload = new ByteArraySegment(Header.Bytes, startingOffset, segmentLength, bytesLength);

                                                                             return ParseNextSegment(payload,
                                                                                                     Protocol,
                                                                                                     this);
                                                                         },
                                                                         LazyThreadSafetyMode.PublicationOnly);
            }
        }

        /// <summary>
        /// Constructor with parent
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IPv6Packet
        (
            ByteArraySegment byteArraySegment,
            Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.White;

        /// <summary>
        /// The destination address field of the IPv6 Packet.
        /// </summary>
        public override IPAddress DestinationAddress
        {
            get => GetIPAddress(AddressFamily.InterNetworkV6,
                                Header.Offset + IPv6Fields.DestinationAddressPosition,
                                Header.Bytes);
            set
            {
                var address = value.GetAddressBytes();

                for (var i = 0; i < address.Length; i++)
                    Header.Bytes[Header.Offset + IPv6Fields.DestinationAddressPosition + i] = address[i];
            }
        }

        /// <summary>
        /// The extension headers of the IPv6 Packet.
        /// </summary>
        public List<IPv6ExtensionHeader> ExtensionHeaders
        {
            get
            {
                if (_totalExtensionHeadersLength == -1)
                    CalculateExtensionHeaders();

                return _extensionHeaders;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the extension headers.
        /// </summary>
        public int ExtensionHeadersLength
        {
            get
            {
                if (_totalExtensionHeadersLength == -1)
                    CalculateExtensionHeaders();

                return _totalExtensionHeadersLength;
            }
        }

        /// <summary>
        /// The flow label field of the IPv6 Packet.
        /// </summary>
        public int FlowLabel
        {
            get => VersionTrafficClassFlowLabel & 0xFFFFF;
            set
            {
                // read the original value
                var field = (uint) VersionTrafficClassFlowLabel;

                // make the value in
                field = (field & 0xFFF00000) | ((uint) value & 0x000FFFFF);

                // write the updated value back
                VersionTrafficClassFlowLabel = (int) field;
            }
        }

        /// <value>
        /// Backwards compatibility property for IPv4.HeaderLength
        /// NOTE: This field is the number of 32bit words
        /// </value>
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        public override int HeaderLength
        {
            get => IPv6Fields.HeaderLength / 4;
            set => ThrowHelper.ThrowNotImplementedException();
        }

        /// <summary>
        /// The hop limit field of the IPv6 Packet.
        /// NOTE: Replaces the 'time to live' field of IPv4
        /// 8-bit value
        /// </summary>
        public override int HopLimit
        {
            get => Header.Bytes[Header.Offset + IPv6Fields.HopLimitPosition];
            set => Header.Bytes[Header.Offset + IPv6Fields.HopLimitPosition] = (byte) value;
        }

        /// <summary>
        /// Identifies the next header field, which is the protocol of the encapsulated in packet unless there are extended headers.
        /// </summary>
        public ProtocolType NextHeader
        {
            get => (ProtocolType) Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition];
            set => Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition] = (byte) value;
        }

        /// <summary>
        /// The size of the payload in octets, including any extension headers.
        /// This differs from the IPv4's Total Length, which also includes the length of the header.
        /// </summary>
        public override ushort PayloadLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv6Fields.PayloadLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + IPv6Fields.PayloadLengthPosition);
        }

        /// <value>
        /// The protocol of the packet encapsulated in this ip packet
        /// </value>
        public override ProtocolType Protocol
        {
            get
            {
                if (_totalExtensionHeadersLength == -1)
                    CalculateExtensionHeaders();

                if (_totalExtensionHeadersLength == 0)
                    return (ProtocolType) Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition];


                return (ProtocolType) Header.Bytes[_protocolOffset];
            }
            set => Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition + _totalExtensionHeadersLength] = (byte) value;
        }

        /// <summary>
        /// The source address field of the IPv6 Packet.
        /// </summary>
        public override IPAddress SourceAddress
        {
            get => GetIPAddress(AddressFamily.InterNetworkV6,
                                Header.Offset + IPv6Fields.SourceAddressPosition,
                                Header.Bytes);
            set
            {
                var address = value.GetAddressBytes();

                for (var i = 0; i < address.Length; i++)
                    Header.Bytes[Header.Offset + IPv6Fields.SourceAddressPosition + i] = address[i];
            }
        }

        /// <value>
        /// Helper alias for 'HopLimit'
        /// </value>
        public override int TimeToLive
        {
            get => HopLimit;
            set => HopLimit = value;
        }

        /// <value>
        /// Backwards compatibility property for IPv4.TotalLength
        /// </value>
        public override int TotalLength
        {
            get => PayloadLength + (HeaderLength * 4);
            set => PayloadLength = (ushort) (value - (HeaderLength * 4));
        }

        /// <summary>
        /// The traffic class field of the IPv6 Packet.
        /// </summary>
        public int TrafficClass
        {
            get => (VersionTrafficClassFlowLabel >> 20) & 0xFF;
            set
            {
                // read the original value
                var field = (uint) VersionTrafficClassFlowLabel;

                // mask in the new field
                field = (field & 0xF00FFFFF) | ((uint) value << 20) & 0x0FF00000;

                // write the updated value back
                VersionTrafficClassFlowLabel = (int) field;
            }
        }

        /// <summary>
        /// The version field of the IPv6 Packet.
        /// </summary>
        public override IPVersion Version
        {
            get => (IPVersion) ((VersionTrafficClassFlowLabel >> 28) & 0xF);
            set
            {
                var v = (int) value;

                // read the existing value
                var field = (uint) VersionTrafficClassFlowLabel;

                // mask the new field into place
                field = (uint) ((field & 0x0FFFFFFF) | ((v << 28) & 0xF0000000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int) field;
            }
        }

        private int VersionTrafficClassFlowLabel
        {
            get => EndianBitConverter.Big.ToInt32(Header.Bytes,
                                                  Header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
        }

        /// <summary>
        /// Calculates the extension headers.
        /// </summary>
        private void CalculateExtensionHeaders()
        {
            _extensionHeaders = new List<IPv6ExtensionHeader>();

            var nextHeaderPosition = Header.Offset + IPv6Fields.NextHeaderPosition;
            var totalExtensionHeadersLength = 0;

            var nextHeader = (ProtocolType) Header.Bytes[nextHeaderPosition];

            // Strip off the extension headers.
            while (ExtensionHeaderTypes.Contains(nextHeader) && nextHeader != ProtocolType.IPv6NoNextHeader)
            {
                nextHeaderPosition = Header.Offset + IPv6Fields.HeaderLength + totalExtensionHeadersLength;

                var extensionHeader = new IPv6ExtensionHeader(nextHeader, Header.NextSegment(PayloadLength));
                _extensionHeaders.Add(extensionHeader);

                totalExtensionHeadersLength += extensionHeader.Length;

                nextHeader = (ProtocolType) Header.Bytes[nextHeaderPosition];
            }

            _protocolOffset = nextHeaderPosition;
            _totalExtensionHeadersLength = totalExtensionHeadersLength;
        }

        /// <inheritdoc />
        internal override byte[] GetPseudoIPHeader(int originalHeaderLength)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            // 0-16: ip src addr
            bw.Write(Header.Bytes,
                     Header.Offset + IPv6Fields.SourceAddressPosition,
                     IPv6Fields.AddressLength);

            // 17-32: ip dst addr
            bw.Write(Header.Bytes,
                     Header.Offset + IPv6Fields.DestinationAddressPosition,
                     IPv6Fields.AddressLength);

            // 33-36: TCP length
            bw.Write((uint) IPAddress.HostToNetworkOrder(originalHeaderLength));

            // 37-39: 3 bytes of zeros
            bw.Write((byte) 0);
            bw.Write((byte) 0);
            bw.Write((byte) 0);

            // 40: Protocol
            bw.Write((byte) Protocol);

            // prefix the pseudoHeader to the header+data
            return ms.ToArray();
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[IPv6Packet: SourceAddress={2}, DestinationAddress={3}, NextHeader={4}]{1}",
                                    color,
                                    colorEscape,
                                    SourceAddress,
                                    DestinationAddress,
                                    Protocol);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>();
                var ipVersion = Convert.ToString((int) Version, 2).PadLeft(4, '0');
                properties.Add("version", ipVersion + " .... .... .... .... .... .... .... = " + (int) Version);
                var trafficClass = Convert.ToString(TrafficClass, 2).PadLeft(8, '0').Insert(4, " ");
                properties.Add("traffic class", ".... " + trafficClass + " .... .... .... .... .... = 0x" + TrafficClass.ToString("x").PadLeft(8, '0'));
                var flowLabel = Convert.ToString(FlowLabel, 2).PadLeft(20, '0').Insert(16, " ").Insert(12, " ").Insert(8, " ").Insert(4, " ");
                properties.Add("flow label", ".... .... .... " + flowLabel + " = 0x" + FlowLabel.ToString("x").PadLeft(8, '0'));
                properties.Add("payload length", PayloadLength.ToString());
                properties.Add("protocol", Protocol + " (0x" + Protocol.ToString("x") + ")");
                properties.Add("hop limit", HopLimit.ToString());
                properties.Add("source", SourceAddress.ToString());
                properties.Add("destination", DestinationAddress.ToString());

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("IP:  ******* IP - \"Internet Protocol (Version 6)\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("IP:");
                foreach (var property in properties)
                {
                    if (property.Key.Trim() != "")
                    {
                        buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }
                    else
                    {
                        buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                    }
                }

                buffer.AppendLine("IP");
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="Packet" />
        /// </returns>
        public static IPv6Packet RandomPacket()
        {
            var srcAddress = RandomUtils.GetIPAddress(IPVersion);
            var dstAddress = RandomUtils.GetIPAddress(IPVersion);
            return new IPv6Packet(srcAddress, dstAddress);
        }
    }
}