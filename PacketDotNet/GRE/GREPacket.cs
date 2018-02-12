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
    /// An GRE packet.
    /// </summary>
    [Serializable]
    public class GREPacket : Packet
    {

        virtual public bool HasCheckSum => 8 == (this.header.Bytes[this.header.Offset + 1] & 0x8);

        virtual public bool HasReserved => 4 == (this.header.Bytes[this.header.Offset + 1] & 0x4);

        virtual public bool HasKey => 2 == (this.header.Bytes[this.header.Offset + 1] & 0x2);

        virtual public bool HasSequence => 1 == (this.header.Bytes[this.header.Offset + 1] & 0x1);


        virtual public int Version => (this.header.Bytes[2] & 0x7);

        virtual public EthernetPacketType Protocol => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(this.header.Bytes, this.header.Offset + GREFields.FlagsLength);


        /// <summary> Fetch the GRE header checksum.</summary>
        virtual public short Checksum => BitConverter.ToInt16(this.header.Bytes, this.header.Offset + GREFields.ChecksumPosition);


        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public String Color => AnsiEscapeSequences.DarkGray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public GREPacket(ByteArraySegment bas, Packet ParentPacket)
        {
            // slice off the header portion
            this.header = new ByteArraySegment(bas)
            {
                Length = GREFields.FlagsLength + GREFields.ProtocolLength
            };
            if (this.HasCheckSum) this.header.Length += GREFields.ChecksumLength;
            if (this.HasReserved) this.header.Length += GREFields.ReservedLength;
            if (this.HasKey) this.header.Length += GREFields.KeyLength;
            if (this.HasSequence) this.header.Length += GREFields.SequenceLength;

            // parse the encapsulated bytes
            this.payloadPacketOrData = EthernetPacket.ParseEncapsulatedBytes(this.header, this.Protocol);
            this.ParentPacket = ParentPacket;
        }
        

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";
            

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[GREPacket: Type={2}",
                    color,
                    colorEscape, this.Protocol);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("Protocol ", this.Protocol + " (0x" + this.Protocol.ToString("x") + ")");
            }

            // append the base string output
            buffer.Append((string) base.ToString(outputFormat));
            
            return buffer.ToString();
        }
    }
}
