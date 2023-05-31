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
    /// Vxlan
    /// </summary>
    public sealed class VxlanPacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VxlanPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public VxlanPacket(ByteArraySegment byteArraySegment)
        {
            // slice off the header portion
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = VxlanFields.HeaderLength
            };

            // parse the encapsulated bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment { Packet = new EthernetPacket(Header.NextSegment()) });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VxlanPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="parentPacket">The parent packet.</param>
        public VxlanPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
            : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Gets or sets the vxlan flags. A valid flag byte should always be 0x08.
        /// </summary>
        public byte Flags
        {
            get => Header.Bytes [Header.Offset + VxlanFields.FlagsPosition];
            set => Header.Bytes [Header.Offset + VxlanFields.FlagsPosition] = value;
        }

        /// <summary>
        /// Gets or sets the VXLAN Network Identifier (VNI)
        /// </summary>
        public uint Vni
        {
            // Note: VNI is 24-bit so the padding of reserved2 byte needs to be considered.
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + VxlanFields.VniPosition) >> 8;
            set => EndianBitConverter.Big.CopyBytes(
                (value << 8) | (uint)Header.Bytes[Header.Offset + VxlanFields.VniPosition + VxlanFields.VniLength],
                Header.Bytes,
                Header.Offset + VxlanFields.VniPosition
                );
        }

        /// <summary>
        /// Fetch ascii escape sequence of the color associated with this packet type.
        /// </summary>
        public override string Color => AnsiEscapeSequences.LightCyan;

        /// <inheritdoc cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";

            if (outputFormat is StringOutputType.Colored or StringOutputType.VerboseColored)
            {
                color = Color;
            }

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                buffer.AppendFormat("{0}[VxlanPacket: VNI={1}]",
                                    color,
                                    Vni);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "flags", "0x" + Flags.ToString("x") },
                    { "VNI", "0x" + Vni.ToString("x") },
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                foreach (var property in properties)
                    buffer.AppendLine("Vxlan: " + property.Key.PadLeft(padLength) + " = " + property.Value);

                buffer.AppendLine("Vxlan:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Determines whether the payload can be decoded by <see cref="VxlanPacket" />.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="udpPacket">The UDP packet.</param>
        /// <returns>
        /// <c>true</c> if the payload can be decoded by <see cref="VxlanPacket"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanDecode(ByteArraySegment payload, UdpPacket udpPacket) => udpPacket.DestinationPort == VxlanFields.DestinationPort;
    }
