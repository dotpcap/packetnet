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

namespace PacketDotNet
{
    /// <summary>
    /// Represents a Linux cooked capture packet, the kinds of packets
    /// received when capturing on an 'any' device
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public class LinuxSllPacket : InternetLinkLayerPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public LinuxSllPacket(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment) { Length = LinuxSllFields.SLLHeaderLength };

            // parse the payload via an EthernetPacket method
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => EthernetPacket.ParseNextSegment(Header,
                                                                                                           EthernetProtocolType));
        }

        /// <value>
        /// The encapsulated protocol type
        /// </value>
        public EthernetType EthernetProtocolType
        {
            get => (EthernetType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + LinuxSllFields.EthernetProtocolTypePosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSllFields.EthernetProtocolTypePosition);
            }
        }

        /// <value>
        /// Link layer header bytes, maximum of 8 bytes
        /// </value>
        public byte[] LinkLayerAddress
        {
            get
            {
                var headerLength = LinkLayerAddressLength;
                var theHeader = new byte[headerLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + LinuxSllFields.LinkLayerAddressPosition,
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
                           Header.Offset + LinuxSllFields.LinkLayerAddressPosition,
                           value.Length);
            }
        }

        /// <value>
        /// Number of bytes in the link layer address of the sender of the packet
        /// </value>
        public int LinkLayerAddressLength
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + LinuxSllFields.LinkLayerAddressLengthPosition);
            set
            {
                // range check
                if (value is < 0 or > 8)
                {
                    throw new InvalidOperationException("value of " + value + " out of range of 0 to 8");
                }

                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSllFields.LinkLayerAddressLengthPosition);
            }
        }

        /// <value>
        /// The
        /// </value>
        public int LinkLayerAddressType
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + LinuxSllFields.LinkLayerAddressTypePosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSllFields.LinkLayerAddressTypePosition);
            }
        }

        /// <value>
        /// Information about the packet direction
        /// </value>
        public LinuxSllType Type
        {
            get => (LinuxSllType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + LinuxSllFields.PacketTypePosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSllFields.PacketTypePosition);
            }
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
                buffer.AppendFormat("[{0}LinuxSllPacket{1}: Type={2}, LinkLayerAddressType={3}, LinkLayerAddressLength={4}, Source={5}, ProtocolType={6}]",
                                    color,
                                    colorEscape,
                                    Type,
                                    LinkLayerAddressType,
                                    LinkLayerAddressLength,
                                    BitConverter.ToString(LinkLayerAddress, 0),
                                    EthernetProtocolType);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "type", Type + " (" + (int) Type + ")" },
                    { "link layer address type", LinkLayerAddressType.ToString() },
                    { "link layer address length", LinkLayerAddressLength.ToString() },
                    { "source", BitConverter.ToString(LinkLayerAddress) },
                    { "protocol", EthernetProtocolType + " (0x" + EthernetProtocolType.ToString("x") + ")" }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("LCC:  ******* LinuxSll - \"Linux Cooked Capture\" - offset=? length=" + TotalPacketLength);
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