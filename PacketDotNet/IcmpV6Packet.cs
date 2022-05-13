/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// An ICMP packet.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    public sealed class IcmpV6Packet : InternetPacket
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
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IcmpV6Packet(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            Header = new ByteArraySegment(byteArraySegment.Bytes, byteArraySegment.Offset, 4);

            // parse the payload
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => ParseNextSegment(byteArraySegment,
                                                                                                Type));
        }

        /// <summary>
        /// Constructor with parent packet
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IcmpV6Packet(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Checksum value
        /// </summary>
        public ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IcmpV6Fields.ChecksumPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + IcmpV6Fields.ChecksumPosition);
            }
        }

        /// <summary>Fetch the ICMP code </summary>
        public byte Code
        {
            get => Header.Bytes[Header.Offset + IcmpV6Fields.CodePosition];
            set => Header.Bytes[Header.Offset + IcmpV6Fields.CodePosition] = value;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.LightBlue;

        /// <summary>
        /// The Type value
        /// </summary>
        public IcmpV6Type Type
        {
            get => (IcmpV6Type) Header.Bytes[Header.Offset + IcmpV6Fields.TypePosition];
            set => Header.Bytes[Header.Offset + IcmpV6Fields.TypePosition] = (byte) value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ICMPv6 checksum is valid.
        /// </summary>
        public bool ValidIcmpChecksum
        {
            get
            {
                Log.Debug("ValidIcmpChecksum");
                var calculatedChecksum = CalculateIcmpChecksum();
                var result = Checksum == calculatedChecksum;
                Log.DebugFormat("ValidIcmpChecksum: {0}", result);
                return result;
            }
        }

        /// <summary>
        /// Parses the next segment.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="type">The type.</param>
        /// <returns><see cref="PacketOrByteArraySegment" />.</returns>
        internal static PacketOrByteArraySegment ParseNextSegment
        (
            ByteArraySegment header,
            IcmpV6Type type)
        {
            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch (type)
            {
                case IcmpV6Type.RouterSolicitation:
                    payloadPacketOrData.Packet = new NdpRouterSolicitationPacket(header);
                    break;
                case IcmpV6Type.RouterAdvertisement:
                    payloadPacketOrData.Packet = new NdpRouterAdvertisementPacket(header);
                    break;
                case IcmpV6Type.NeighborSolicitation:
                    payloadPacketOrData.Packet = new NdpNeighborSolicitationPacket(header);
                    break;
                case IcmpV6Type.NeighborAdvertisement:
                    payloadPacketOrData.Packet = new NdpNeighborAdvertisementPacket(header);
                    break;
                case IcmpV6Type.RedirectMessage:
                    payloadPacketOrData.Packet = new NdpRedirectMessagePacket(header);
                    break;
                default:
                    payloadPacketOrData.ByteArraySegment = new ByteArraySegment(header.Bytes, header.Offset + 4, header.Length - 4);
                    break;
            }

            return payloadPacketOrData;
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateIcmpChecksum()
        {
            Checksum = CalculateIcmpChecksum();
        }

        /// <summary>
        /// Calculates the ICMP checksum.
        /// </summary>
        /// <returns>The calculated ICMP checksum.</returns>
        public ushort CalculateIcmpChecksum()
        {
            var originalChecksum = Checksum;

            Checksum = 0; // This needs to be reset first to calculate the checksum.

            var headerDataSegment = ParentPacket.HeaderDataSegment;
            var calculatedChecksum = (ushort) ChecksumUtils.OnesComplementSum(headerDataSegment, (ParentPacket as IPv6Packet)?.GetPseudoIPHeader(headerDataSegment.Length) ?? Array.Empty<byte>());

            Checksum = originalChecksum;

            return calculatedChecksum;
        }

        /// <inheritdoc cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat is StringOutputType.Colored or StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[IcmpV6Packet: Type={2}, Code={3}]{1}",
                                        color,
                                        colorEscape,
                                        Type,
                                        Code);

                    break;

                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    var properties = new Dictionary<string, string>
                    {
                        { "type", Type + " (" + (int) Type + ")" },
                        { "code", Code.ToString() },
                        // TODO: Implement a checksum verification for ICMPv6
                        { "checksum", "0x" + Checksum.ToString("x") }
                    };
                    // TODO: Implement ICMPv6 Option fields here?

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("ICMP:  ******* ICMPv6 - \"Internet Control Message Protocol (Version 6)\"- offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("ICMP:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("ICMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("ICMP:");
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}