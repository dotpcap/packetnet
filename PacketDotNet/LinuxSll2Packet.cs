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

namespace PacketDotNet;

    /// <summary>
    /// Represents a Linux cooked capture packet, the kinds of packets
    /// received when capturing on an 'any' device
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public class LinuxSll2Packet : InternetLinkLayerPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public LinuxSll2Packet(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment) { Length = LinuxSll2Fields.SLL2HeaderLength };

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
                                                                 Header.Offset + LinuxSll2Fields.EthernetProtocolTypePosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSll2Fields.EthernetProtocolTypePosition);
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
                           Header.Offset + LinuxSll2Fields.LinkLayerAddressPosition,
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
                           Header.Offset + LinuxSll2Fields.LinkLayerAddressPosition,
                           value.Length);
            }
        }

        /// <value>
        /// Number of bytes in the link layer address of the sender of the packet
        /// </value>
        public int LinkLayerAddressLength
        {
            get => Header.Bytes[Header.Offset + LinuxSll2Fields.LinkLayerAddressLengthPosition];
            set
            {
                // range check
                if (value is < 0 or > 8)
                {
                    throw new InvalidOperationException("value of " + value + " out of range of 0 to 8");
                }

                Header.Bytes[Header.Offset + LinuxSll2Fields.LinkLayerAddressLengthPosition] = (byte) value;
            }
        }

        /// <value>
        /// The
        /// </value>
        public int LinkLayerAddressType
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + LinuxSll2Fields.LinkLayerAddressTypePosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSll2Fields.LinkLayerAddressTypePosition);
            }
        }

        /// <value>
        /// Information about the packet direction
        /// </value>
        public LinuxSll2Type Type
        {
            get => (LinuxSll2Type)Header.Bytes[Header.Offset + LinuxSll2Fields.PacketTypePosition];
            set
            {
                Header.Bytes[Header.Offset + LinuxSll2Fields.PacketTypePosition] = (byte) value;
            }
        }

        /// <value>
        /// Information about the interface index
        /// </value>
        public int InterfaceIndex
        {
            get => EndianBitConverter.Big.ToInt32(Header.Bytes,
                                                  Header.Offset + LinuxSll2Fields.InterfaceIndexPosition);
            set
            {
                var v = (int)value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + LinuxSll2Fields.InterfaceIndexPosition);
            }
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

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("[{0}LinuxSll2Packet{1}: ProtocolType={2}, InterfaceIndex={3}, LinkLayerAddressType={4}, Type={5}, LinkLayerAddressLength={6}, Source={7}]",
                                    color,
                                    colorEscape,
                                    EthernetProtocolType,
                                    InterfaceIndex,
                                    LinkLayerAddressType,
                                    Type,
                                    LinkLayerAddressLength,
                                    BitConverter.ToString(LinkLayerAddress, 0));
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "protocol", EthernetProtocolType + " (0x" + EthernetProtocolType.ToString("x") + ")" },
                    { "interface index", InterfaceIndex.ToString() },
                    { "link layer address type", LinkLayerAddressType.ToString() },
                    { "type", Type + " (" + (int) Type + ")" },
                    { "link layer address length", LinkLayerAddressLength.ToString() },
                    { "source", BitConverter.ToString(LinkLayerAddress) },
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("LCC:  ******* LinuxSll2 - \"Linux Cooked Capture v2\" - offset=? length=" + TotalPacketLength);
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