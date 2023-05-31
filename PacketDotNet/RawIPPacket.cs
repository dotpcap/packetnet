/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2016 Cameron Elliott <cameron@cameronelliott.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet;

    /// <summary>
    /// Raw IP packet
    /// See http://www.tcpdump.org/linktypes.html look for LINKTYPE_RAW or DLT_RAW
    /// </summary>
    public class RawIPPacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public RawIPPacket(ByteArraySegment byteArraySegment)
        {
            // Pcap raw link layer format does not have any header
            // you need to identify whether you have ipv4 or ipv6
            // directly by checking the IP version number.
            // If the first nibble is 0x04, then you have IP v4
            // If the first nibble is 0x06, then you have IP v6
            // The RawIPPacketProtocol enum has been defined to match this.
            var firstNibble = byteArraySegment.Bytes[0] >> 4;
            Protocol = (RawIPPacketProtocol) firstNibble;

            Header = new ByteArraySegment(byteArraySegment) { Length = 0 };

            // parse the encapsulated bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment();
                switch (Protocol)
                {
                    case RawIPPacketProtocol.IPv4:
                    {
                        result.Packet = new IPv4Packet(Header.NextSegment());
                        break;
                    }
                    case RawIPPacketProtocol.IPv6:
                    {
                        result.Packet = new IPv6Packet(Header.NextSegment());
                        break;
                    }
                    default:
                    {
                        throw new NotImplementedException("Protocol of " + Protocol + " is not implemented");
                    }
                }

                return result;
            });
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        public RawIPPacketProtocol Protocol { get; set; }

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

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[RawPacket: Protocol={2}]{1}",
                                    color,
                                    colorEscape,
                                    Protocol);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "protocol", Protocol + " (0x" + Protocol.ToString("x") + ")" }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("Raw:  ******* Raw - \"Raw IP Packet\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("Raw:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("Raw: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("Raw:");
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }