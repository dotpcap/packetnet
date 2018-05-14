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
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An GRE packet.
    /// </summary>
    [Serializable]
    public class GREPacket : Packet
    {

        public virtual Boolean HasCheckSum => 8 == (Header.Bytes[Header.Offset + 1] & 0x8);

        public virtual Boolean HasReserved => 4 == (Header.Bytes[Header.Offset + 1] & 0x4);

        public virtual Boolean HasKey => 2 == (Header.Bytes[Header.Offset + 1] & 0x2);

        public virtual Boolean HasSequence => 1 == (Header.Bytes[Header.Offset + 1] & 0x1);


        public virtual Int32 Version => (Header.Bytes[2] & 0x7);

        public virtual EthernetPacketType Protocol => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
            Header.Offset + GREFields.FlagsLength);


        /// <summary> Fetch the GRE header checksum.</summary>
        public virtual Int16 Checksum => BitConverter.ToInt16(Header.Bytes,
            Header.Offset + GREFields.ChecksumPosition);


        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override System.String Color => AnsiEscapeSequences.DarkGray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public GREPacket(ByteArraySegment bas, Packet ParentPacket)
        {
            // slice off the header portion
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
            this.ParentPacket = ParentPacket;
        }
        

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            String color = "";
            String colorEscape = "";
            

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[GREPacket: Type={2}",
                    color,
                    colorEscape,
                    Protocol);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<String,String> properties = new Dictionary<String,String>();
                properties.Add("Protocol ", Protocol + " (0x" + Protocol.ToString("x") + ")");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));
            
            return buffer.ToString();
        }
    }
}
