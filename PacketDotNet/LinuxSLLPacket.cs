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
 */

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Represents a Linux cooked capture packet, the kinds of packets
    /// received when capturing on an 'any' device
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public class LinuxSLLPacket : InternetLinkLayerPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public LinuxSLLPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);
            Header.Length = LinuxSLLFields.SLLHeaderLength;

            // parse the payload via an EthernetPacket method
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => EthernetPacket.ParseEncapsulatedBytes(Header,
                                                                                                                 EthernetProtocolType));
        }

        /// <value>
        /// The encapsulated protocol type
        /// </value>
        public EthernetPacketType EthernetProtocolType
        {
            get => (EthernetPacketType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                       Header.Offset + LinuxSLLFields.EthernetProtocolTypePosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSLLFields.EthernetProtocolTypePosition);
            }
        }

        /// <value>
        /// Link layer header bytes, maximum of 8 bytes
        /// </value>
        public Byte[] LinkLayerAddress
        {
            get
            {
                var headerLength = LinkLayerAddressLength;
                var theHeader = new Byte[headerLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + LinuxSLLFields.LinkLayerAddressPosition,
                           theHeader,
                           0,
                           headerLength);
                return theHeader;
            }

            set
            {
                // update the link layer length
                LinkLayerAddressLength = value.Length;

                // copy in the new link layer header bytes
                Array.Copy(value,
                           0,
                           Header.Bytes,
                           Header.Offset + LinuxSLLFields.LinkLayerAddressPosition,
                           value.Length);
            }
        }

        /// <value>
        /// Number of bytes in the link layer address of the sender of the packet
        /// </value>
        public Int32 LinkLayerAddressLength
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + LinuxSLLFields.LinkLayerAddressLengthPosition);

            set
            {
                // range check
                if (value < 0 || value > 8)
                {
                    throw new InvalidOperationException("value of " + value + " out of range of 0 to 8");
                }

                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSLLFields.LinkLayerAddressLengthPosition);
            }
        }

        /// <value>
        /// The
        /// </value>
        public Int32 LinkLayerAddressType
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + LinuxSLLFields.LinkLayerAddressTypePosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSLLFields.LinkLayerAddressTypePosition);
            }
        }

        /// <value>
        /// Information about the packet direction
        /// </value>
        public LinuxSLLType Type
        {
            get => (LinuxSLLType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + LinuxSLLFields.PacketTypePosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSLLFields.PacketTypePosition);
            }
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
                buffer.AppendFormat("[{0}LinuxSLLPacket{1}: Type={2}, LinkLayerAddressType={3}, LinkLayerAddressLength={4}, Source={5}, ProtocolType={6}]",
                                    color,
                                    colorEscape,
                                    Type,
                                    LinkLayerAddressType,
                                    LinkLayerAddressLength,
                                    BitConverter.ToString(LinkLayerAddress, 0),
                                    EthernetProtocolType);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"type", Type + " (" + (Int32) Type + ")"},
                    {"link layer address type", LinkLayerAddressType.ToString()},
                    {"link layer address length", LinkLayerAddressLength.ToString()},
                    {"source", BitConverter.ToString(LinkLayerAddress)},
                    {"protocol", EthernetProtocolType + " (0x" + EthernetProtocolType.ToString("x") + ")"}
                };


                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("LCC:  ******* LinuxSLL - \"Linux Cooked Capture\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("LCC:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("LCC: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("LCC:");
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}