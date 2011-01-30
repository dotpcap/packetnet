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
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An IGMP packet.
    /// </summary>
    [Serializable]
    public class IGMPv2Packet : InternetPacket
    {
        /// <value>
        /// The type of IGMP message
        /// </value>
        virtual public IGMPMessageType Type
        {
            get
            {
                return (IGMPMessageType)header.Bytes[header.Offset + IGMPv2Fields.TypePosition];
            }

            set
            {
                header.Bytes[header.Offset + IGMPv2Fields.TypePosition] = (byte)value;
            }
        }

        /// <summary> Fetch the IGMP max response time.</summary>
        virtual public byte MaxResponseTime
        {
            get
            {
                return header.Bytes[header.Offset + IGMPv2Fields.MaxResponseTimePosition];
            }

            set
            {
                header.Bytes[header.Offset + IGMPv2Fields.MaxResponseTimePosition] = value;
            }
        }

        /// <summary> Fetch the IGMP header checksum.</summary>
        virtual public short Checksum
        {
            get
            {
                return BitConverter.ToInt16(header.Bytes,
                                                      header.Offset + IGMPv2Fields.ChecksumPosition);
            }

            set
            {
                byte[] theValue = BitConverter.GetBytes(value);
                Array.Copy(theValue, 0, header.Bytes, (header.Offset + IGMPv2Fields.ChecksumPosition), 2);
            }
        }

        /// <summary> Fetch the IGMP group address.</summary>
        virtual public System.Net.IPAddress GroupAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + IGMPv2Fields.GroupAddressPosition,
                                             header.Bytes);
            }

        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Brown;
            }

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public IGMPv2Packet(ByteArraySegment bas)
        {
            // set the header field, header field values are retrieved from this byte array
            header = new ByteArraySegment(bas);
            header.Length = UdpFields.HeaderLength;

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
        }

        /// <summary>
        /// Constructor with parent
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public IGMPv2Packet(ByteArraySegment bas,
                            Packet ParentPacket) : this(bas)
        {
            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// Returns the encapsulated IGMPv2Packet of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="IGMPv2Packet"/>
        /// </returns>
        public static IGMPv2Packet GetEncapsulated(Packet p)
        {
            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    Console.WriteLine("Is an IP packet");
                    var innerPayload = payload.PayloadPacket;
                    if(innerPayload is IGMPv2Packet)
                    {
                        return (IGMPv2Packet)innerPayload;
                    }
                }
            }

            return null;

        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[IGMPv2Packet: Type={2}, MaxResponseTime={3}, GroupAddress={4}]{1}",
                    color,
                    colorEscape,
                    Type,
                    String.Format("{0:0.0}", (MaxResponseTime / 10)),
                    GroupAddress);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("type", Type + " (0x" + Type.ToString("x") + ")");
                properties.Add("max response time", String.Format("{0:0.0}", MaxResponseTime / 10) + " sec (0x" + MaxResponseTime.ToString("x") + ")");
                // TODO: Implement checksum validation for IGMPv2
                properties.Add("header checksum", "0x" + Checksum.ToString("x"));
                properties.Add("group address", GroupAddress.ToString());

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

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