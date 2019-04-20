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
 *  Copyright 2018 Steven Haufe<haufes@hotmail.com>
  */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An GRE packet.
    /// </summary>
    [Serializable]
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
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => EthernetPacket.ParseNextSegment(Header, Protocol), LazyThreadSafetyMode.PublicationOnly);
            ParentPacket = parentPacket;
        }

        /// <summary>Fetch the GRE header checksum.</summary>
        public short Checksum => BitConverter.ToInt16(Header.Bytes,
                                                      Header.Offset + GreFields.ChecksumPosition);

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        public bool HasCheckSum => 8 == (Header.Bytes[Header.Offset + 1] & 0x8);

        public bool HasKey => 2 == (Header.Bytes[Header.Offset + 1] & 0x2);

        public bool HasReserved => 4 == (Header.Bytes[Header.Offset + 1] & 0x4);

        public bool HasSequence => 1 == (Header.Bytes[Header.Offset + 1] & 0x1);

        public EthernetPacketType Protocol => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                                                   Header.Offset + GreFields.FlagsLength);

        public int Version => Header.Bytes[2] & 0x7;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[GrePacket: Type={1}",
                                    color,
                                    Protocol);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
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
    }
}