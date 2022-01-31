/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
  */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An IGMP packet.
    /// </summary>
    public sealed class IgmpV2Packet : IgmpPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV2Packet" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public IgmpV2Packet(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = UdpFields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment { ByteArraySegment = Header.NextSegment() });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV2Packet" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="parentPacket">The parent packet.</param>
        public IgmpV2Packet(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Gets or sets the IGMP header checksum.</summary>
        public short Checksum
        {
            get => BitConverter.ToInt16(Header.Bytes,
                                        Header.Offset + IgmpV2Fields.ChecksumPosition);
            set
            {
                var v = BitConverter.GetBytes(value);
                Array.Copy(v, 0, Header.Bytes, Header.Offset + IgmpV2Fields.ChecksumPosition, 2);
            }
        }

        /// <summary>Gets or sets ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Brown;

        /// <summary>Gets or sets the IGMP group address.</summary>
        public IPAddress GroupAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                               Header.Offset + IgmpV2Fields.GroupAddressPosition,
                                                               Header.Bytes);

        /// <summary>Gets or sets the IGMP max response time.</summary>
        public byte MaxResponseTime
        {
            get => Header.Bytes[Header.Offset + IgmpV2Fields.MaxResponseTimePosition];
            set => Header.Bytes[Header.Offset + IgmpV2Fields.MaxResponseTimePosition] = value;
        }

        /// <summary>
        /// Gets or sets the type of IGMP message.
        /// </summary>
        public override IgmpMessageType Type
        {
            get => (IgmpMessageType) Header.Bytes[Header.Offset + IgmpV2Fields.TypePosition];
            set => Header.Bytes[Header.Offset + IgmpV2Fields.TypePosition] = (byte) value;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
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

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[IgmpV2Packet: Type={2}, MaxResponseTime={3}, GroupAddress={4}]{1}",
                                    color,
                                    colorEscape,
                                    Type,
                                    $"{MaxResponseTime / 10:0.0}",
                                    GroupAddress);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "type", Type + " (0x" + Type.ToString("x") + ")" },
                    { "max response time", $"{MaxResponseTime / 10:0.0}" + " sec (0x" + MaxResponseTime.ToString("x") + ")" },
                    // TODO: Implement checksum validation for IGMPv2
                    { "header checksum", "0x" + Checksum.ToString("x") },
                    { "group address", GroupAddress.ToString() }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.Append("IGMP:  ******* IGMPv2 - \"Internet Group Management Protocol (Version 2)\" - offset=? length=").Append(TotalPacketLength).AppendLine();
                buffer.AppendLine("IGMP:");
                foreach (var property in properties)
                {
                    buffer.Append("IGMP: ").Append(property.Key.PadLeft(padLength)).Append(" = ").AppendLine(property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}