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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

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
        /// Minimum number of bytes in an IPv6 header
        /// </value>
        public const Int32 HeaderMinimumLength = 40;

        /// <value>
        /// The version of the IP protocol. The '6' in IPv6 indicates the version of the protocol
        /// </value>
        public static IPVersion IPVersion = IPVersion.IPv6;

        private Int32 VersionTrafficClassFlowLabel
        {
            get => EndianBitConverter.Big.ToInt32(Header.Bytes,
                                                  Header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);

            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
        }

        /// <summary>
        /// The version field of the IPv6 Packet.
        /// </summary>
        public override IPVersion Version
        {
            get => (IPVersion) ((VersionTrafficClassFlowLabel >> 28) & 0xF);

            set
            {
                var theValue = (Int32) value;

                // read the existing value
                var field = (UInt32) VersionTrafficClassFlowLabel;

                // mask the new field into place
                field = (UInt32) ((field & 0x0FFFFFFF) | ((theValue << 28) & 0xF0000000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        /// The traffic class field of the IPv6 Packet.
        /// </summary>
        public Int32 TrafficClass
        {
            get => (VersionTrafficClassFlowLabel >> 20) & 0xFF;

            set
            {
                // read the original value
                var field = (UInt32) VersionTrafficClassFlowLabel;

                // mask in the new field
                field = (field & 0xF00FFFFF) | ((UInt32) value << 20) & 0x0FF00000;

                // write the updated value back
                VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        /// The flow label field of the IPv6 Packet.
        /// </summary>
        public Int32 FlowLabel
        {
            get => VersionTrafficClassFlowLabel & 0xFFFFF;

            set
            {
                // read the original value
                var field = (UInt32) VersionTrafficClassFlowLabel;

                // make the value in
                field = (field & 0xFFF00000) | ((UInt32) value & 0x000FFFFF);

                // write the updated value back
                VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        /// The payload lengeth field of the IPv6 Packet
        /// NOTE: Differs from the IPv4 'Total length' field that includes the length of the header as
        /// payload length is ONLY the size of the payload.
        /// </summary>
        public override UInt16 PayloadLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv6Fields.PayloadLengthPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + IPv6Fields.PayloadLengthPosition);
        }

        /// <value>
        /// Backwards compatibility property for IPv4.HeaderLength
        /// NOTE: This field is the number of 32bit words
        /// </value>
        public override Int32 HeaderLength
        {
            get => IPv6Fields.HeaderLength / 4;

            set => throw new NotImplementedException();
        }

        /// <value>
        /// Backwards compatibility property for IPv4.TotalLength
        /// </value>
        public override Int32 TotalLength
        {
            get => PayloadLength + (HeaderLength * 4);

            set => PayloadLength = (UInt16) (value - (HeaderLength * 4));
        }

        /// <summary>
        /// Identifies the protocol encapsulated by this packet
        /// Replaces IPv4's 'protocol' field, has compatible values
        /// </summary>
        public override IPProtocolType NextHeader
        {
            get => (IPProtocolType) Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition];

            set => Header.Bytes[Header.Offset + IPv6Fields.NextHeaderPosition] = (Byte) value;
        }

        /// <value>
        /// The protocol of the packet encapsulated in this ip packet
        /// </value>
        public override IPProtocolType Protocol
        {
            get => NextHeader;
            set => NextHeader = value;
        }

        /// <summary>
        /// The hop limit field of the IPv6 Packet.
        /// NOTE: Replaces the 'time to live' field of IPv4
        /// 8-bit value
        /// </summary>
        public override Int32 HopLimit
        {
            get => Header.Bytes[Header.Offset + IPv6Fields.HopLimitPosition];

            set => Header.Bytes[Header.Offset + IPv6Fields.HopLimitPosition] = (Byte) value;
        }

        /// <value>
        /// Helper alias for 'HopLimit'
        /// </value>
        public override Int32 TimeToLive
        {
            get => HopLimit;
            set => HopLimit = value;
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
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + IPv6Fields.SourceAddressPosition,
                           address.Length);
            }
        }

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
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + IPv6Fields.DestinationAddressPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Create an IPv6 packet from values
        /// </summary>
        /// <param name="sourceAddress">
        /// A <see cref="System.Net.IPAddress" />
        /// </param>
        /// <param name="destinationAddress">
        /// A <see cref="System.Net.IPAddress" />
        /// </param>
        public IPv6Packet
        (
            IPAddress sourceAddress,
            IPAddress destinationAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = IPv6Fields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

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
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IPv6Packet(ByteArraySegment bas)
        {
            Log.Debug(bas.ToString());

            // slice off the header
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = HeaderMinimumLength;

            // set the actual length, we need to do this because we need to set
            // header to something valid above before we can retrieve the PayloadLength
            Log.DebugFormat("PayloadLength: {0}", PayloadLength);
            Header.Length = bas.Length - PayloadLength;

            // parse the payload
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var payload = Header.EncapsulatedBytes(PayloadLength);
                return ParseEncapsulatedBytes(payload,
                                              NextHeader,
                                              this);
            });
        }

        /// <summary>
        /// Constructor with parent
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IPv6Packet
        (
            ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            ParentPacket = parentPacket;
        }


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
            bw.Write((UInt32) IPAddress.HostToNetworkOrder(originalHeaderLength));

            // 37-39: 3 bytes of zeros
            bw.Write((Byte) 0);
            bw.Write((Byte) 0);
            bw.Write((Byte) 0);

            // 40: Next header
            bw.Write((Byte) NextHeader);

            // prefix the pseudoHeader to the header+data
            return ms.ToArray();
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
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
                                    NextHeader);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>();
                var ipVersion = Convert.ToString((Int32) Version, 2).PadLeft(4, '0');
                properties.Add("version", ipVersion + " .... .... .... .... .... .... .... = " + (Int32) Version);
                var trafficClass = Convert.ToString(TrafficClass, 2).PadLeft(8, '0').Insert(4, " ");
                properties.Add("traffic class", ".... " + trafficClass + " .... .... .... .... .... = 0x" + TrafficClass.ToString("x").PadLeft(8, '0'));
                var flowLabel = Convert.ToString(FlowLabel, 2).PadLeft(20, '0').Insert(16, " ").Insert(12, " ").Insert(8, " ").Insert(4, " ");
                properties.Add("flow label", ".... .... .... " + flowLabel + " = 0x" + FlowLabel.ToString("x").PadLeft(8, '0'));
                properties.Add("payload length", PayloadLength.ToString());
                properties.Add("next header", NextHeader + " (0x" + NextHeader.ToString("x") + ")");
                properties.Add("hop limit", HopLimit.ToString());
                properties.Add("source", SourceAddress.ToString());
                properties.Add("destination", DestinationAddress.ToString());

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

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

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.White;

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