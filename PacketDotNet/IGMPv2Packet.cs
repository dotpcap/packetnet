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
    [Serializable]
    public sealed class IGMPv2Packet : InternetPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IGMPv2Packet(ByteArraySegment bas)
        {
            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = UdpFields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.EncapsulatedBytes()};
                return result;
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
        public IGMPv2Packet
        (
            ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            ParentPacket = parentPacket;
        }

        /// <summary> Fetch the IGMP header checksum.</summary>
        public Int16 Checksum
        {
            get => BitConverter.ToInt16(Header.Bytes,
                                        Header.Offset + IGMPv2Fields.ChecksumPosition);

            set
            {
                var theValue = BitConverter.GetBytes(value);
                Array.Copy(theValue, 0, Header.Bytes, Header.Offset + IGMPv2Fields.ChecksumPosition, 2);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.Brown;

        /// <summary> Fetch the IGMP group address.</summary>
        public IPAddress GroupAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                               Header.Offset + IGMPv2Fields.GroupAddressPosition,
                                                               Header.Bytes);

        /// <summary> Fetch the IGMP max response time.</summary>
        public Byte MaxResponseTime
        {
            get => Header.Bytes[Header.Offset + IGMPv2Fields.MaxResponseTimePosition];

            set => Header.Bytes[Header.Offset + IGMPv2Fields.MaxResponseTimePosition] = value;
        }

        /// <value>
        /// The type of IGMP message
        /// </value>
        public IGMPMessageType Type
        {
            get => (IGMPMessageType) Header.Bytes[Header.Offset + IGMPv2Fields.TypePosition];

            set => Header.Bytes[Header.Offset + IGMPv2Fields.TypePosition] = (Byte) value;
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
                buffer.AppendFormat("{0}[IGMPv2Packet: Type={2}, MaxResponseTime={3}, GroupAddress={4}]{1}",
                                    color,
                                    colorEscape,
                                    Type,
                                    $"{MaxResponseTime / 10:0.0}",
                                    GroupAddress);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"type", Type + " (0x" + Type.ToString("x") + ")"},
                    {"max response time", $"{MaxResponseTime / 10:0.0}" + " sec (0x" + MaxResponseTime.ToString("x") + ")"},
                    // TODO: Implement checksum validation for IGMPv2
                    {"header checksum", "0x" + Checksum.ToString("x")},
                    {"group address", GroupAddress.ToString()}
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("IGMP:  ******* IGMPv2 - \"Internet Group Management Protocol (Version 2)\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("IGMP:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("IGMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}