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
using PacketDotNet.Ethernet;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.GRE
{
    /// <summary>
    ///     An GRE packet.
    /// </summary>
    [Serializable]
    public class GREPacket : Packet
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public GREPacket(ByteArraySegment bas, Packet parentPacket)
        {
            // slice off the header portion
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = GREFields.FlagsLength + GREFields.ProtocolLength
            };
            if (this.HasCheckSum) this.HeaderByteArraySegment.Length += GREFields.ChecksumLength;
            if (this.HasReserved) this.HeaderByteArraySegment.Length += GREFields.ReservedLength;
            if (this.HasKey) this.HeaderByteArraySegment.Length += GREFields.KeyLength;
            if (this.HasSequence) this.HeaderByteArraySegment.Length += GREFields.SequenceLength;

            // parse the encapsulated bytes
            this.PayloadPacketOrData =
                EthernetPacket.ParseEncapsulatedBytes(this.HeaderByteArraySegment, this.Protocol);
            this.ParentPacket = parentPacket;
        }


        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;


        /// <summary> Fetch the GRE header checksum.</summary>
        public virtual Int16 Checksum => BitConverter.ToInt16(this.HeaderByteArraySegment.Bytes,
            this.HeaderByteArraySegment.Offset + GREFields.ChecksumPosition);

        public virtual Boolean HasCheckSum =>
            8 == (this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + 1] & 0x8);

        public virtual Boolean HasKey =>
            2 == (this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + 1] & 0x2);

        public virtual Boolean HasReserved =>
            4 == (this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + 1] & 0x4);

        public virtual Boolean HasSequence =>
            1 == (this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + 1] & 0x1);

        public virtual EthernetPacketType Protocol => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(
            this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + GREFields.FlagsLength);


        public virtual Int32 Version => (this.HeaderByteArraySegment.Bytes[2] & 0x7);


        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            String color = "";
            String colorEscape = "";


            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[GREPacket: Type={2}",
                        color,
                        colorEscape, this.Protocol);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>
                    {
                        {"Protocol ", this.Protocol + " (0x" + this.Protocol.ToString("x") + ")"}
                    };
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}