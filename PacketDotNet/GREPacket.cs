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
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An GRE packet.
    /// </summary>
    [Serializable]
    public sealed class GREPacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">A <see cref="ByteArraySegment" /></param>
        /// <param name="parentPacket">The parent packet.</param>
        public GREPacket(ByteArraySegment bas, Packet parentPacket)
        {
            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = GREFields.FlagsLength + GREFields.ProtocolLength;
            if (HasCheckSum)
                Header.Length += GREFields.ChecksumLength;
            if (HasReserved)
                Header.Length += GREFields.ReservedLength;
            if (HasKey)
                Header.Length += GREFields.KeyLength;
            if (HasSequence)
                Header.Length += GREFields.SequenceLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => EthernetPacket.ParseEncapsulatedBytes(Header, Protocol));
            ParentPacket = parentPacket;
        }


        /// <summary> Fetch the GRE header checksum.</summary>
        public Int16 Checksum => BitConverter.ToInt16(Header.Bytes,
                                                      Header.Offset + GREFields.ChecksumPosition);


        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;

        public Boolean HasCheckSum => 8 == (Header.Bytes[Header.Offset + 1] & 0x8);

        public Boolean HasKey => 2 == (Header.Bytes[Header.Offset + 1] & 0x2);

        public Boolean HasReserved => 4 == (Header.Bytes[Header.Offset + 1] & 0x4);

        public Boolean HasSequence => 1 == (Header.Bytes[Header.Offset + 1] & 0x1);

        public EthernetPacketType Protocol => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                                                   Header.Offset + GREFields.FlagsLength);


        public Int32 Version => Header.Bytes[2] & 0x7;


        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
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
                buffer.AppendFormat("{0}[GREPacket: Type={1}",
                                    color,
                                    Protocol);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var unused = new Dictionary<String, String>
                {
                    {"Protocol ", Protocol + " (0x" + Protocol.ToString("x") + ")"}
                };
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}