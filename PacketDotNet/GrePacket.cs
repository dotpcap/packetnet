/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2018 Steven Haufe<haufes@hotmail.com>
  */

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet
{
    /// <summary>
    /// An GRE packet.
    /// </summary>
    public sealed class GrePacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">A <see cref="ByteArraySegment" /></param>
        /// <param name="parentPacket">The parent packet.</param>
        public GrePacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            // slice off the header portion
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = GreFields.FlagsLength + GreFields.ProtocolLength
            };

            if (HasCheckSum)
                Header.Length += GreFields.ChecksumLength;

            if (HasReserved)
                Header.Length += GreFields.ReservedLength;

            if (HasKey)
                Header.Length += GreFields.KeyLength;

            if (HasSequence)
                Header.Length += GreFields.SequenceLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                if (CustomProtocolParser.TryGetValue((ushort)Protocol, out var customParser))
                {
                    return customParser(Header.NextSegment(), this);
                }

                // If no custom parser is registered, parse as standard protocols.
                return EthernetPacket.ParseNextSegment(Header, Protocol);
            });
            ParentPacket = parentPacket;
        }

        /// <summary>Fetch the GRE header checksum.</summary>
        public short Checksum => BitConverter.ToInt16(Header.Bytes,
                                                      Header.Offset + GreFields.ChecksumPosition);

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        public bool HasCheckSum => 0x80 == (Header.Bytes[Header.Offset + 0] & 0x80);

        public bool HasKey => 0x20 == (Header.Bytes[Header.Offset + 0] & 0x20);

        public bool HasReserved => false; // Reserved bits so currently not used

        public bool HasSequence => 0x10 == (Header.Bytes[Header.Offset + 0] & 0x10);

        public EthernetType Protocol => (EthernetType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                                       Header.Offset + GreFields.FlagsLength);

        public int Version => Header.Bytes[Header.Offset + 1] & 0x7;

        // Faster access for multiple flags
        public UInt16 FlagsAndVersion
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 0);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 0);
        }

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
                // build the output string
                buffer.AppendFormat("{0}[GrePacket: Type={1}",
                                    color,
                                    Protocol);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var unused = new Dictionary<string, string>
                {
                    { "Protocol ", Protocol + " (0x" + Protocol.ToString("x") + ")" }
                };
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Custom parsers for GRE non-IANA assigned protocol numbers or proprietary protocols, e.g. ERSPAN.
        /// The Key of dictionary is the protocol number. The value is the function of the custom parser.
        /// The function must take two parameters and return the decoded payload.
        /// The first parameter is the payload of the GRE packet. The second parameter is the GRE packet itself.
        /// Returned value is the decoded payload as PacketOrByteArraySegment.
        /// </summary>
        public static Dictionary<ushort, Func<ByteArraySegment, GrePacket, PacketOrByteArraySegment>> CustomProtocolParser = new();
    }
}