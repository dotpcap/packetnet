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

        virtual public bool HasCheckSum
        {
            get
            {
                return 8 == (header.Bytes[header.Offset + 1] & 0x8);
            }
        }
        virtual public bool HasReserved
        {
            get
            {
                return 4 == (header.Bytes[header.Offset + 1] & 0x4);
            }
        }

        virtual public bool HasKey
        {
            get
            {
                return 2 == (header.Bytes[header.Offset + 1] & 0x2);
            }
        }
        virtual public bool HasSequence
        {
            get
            {
                return 1 == (header.Bytes[header.Offset + 1] & 0x1);
            }
        }


        virtual public int Version
        {
            get
            {
                return (header.Bytes[2] & 0x7);
            }
        }

        virtual public EthernetPacketType Protocol
        {
            get
            {
                return (EthernetPacketType) EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                          header.Offset + GREFields.FlagsLength);
            }
        }


        /// <summary> Fetch the GRE header checksum.</summary>
        virtual public short Checksum
        {
            get
            {
                return BitConverter.ToInt16(header.Bytes,
                                                      header.Offset + GREFields.ChecksumPosition);
            }

        }
        

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.DarkGray;
            }

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public GREPacket(ByteArraySegment bas, Packet ParentPacket)
        {
            // slice off the header portion
            header = new ByteArraySegment(bas);

            header.Length = GREFields.FlagsLength + GREFields.ProtocolLength;
            if (HasCheckSum)
                header.Length += GREFields.ChecksumLength;
            if (HasReserved)
                header.Length += GREFields.ReservedLength;
            if (HasKey)
                header.Length += GREFields.KeyLength;
            if (HasSequence)
                header.Length += GREFields.SequenceLength;

            // parse the encapsulated bytes
            payloadPacketOrData = EthernetPacket.ParseEncapsulatedBytes(header, Protocol);
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
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("Protocol ", Protocol + " (0x" + Protocol.ToString("x") + ")");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));
            
            return buffer.ToString();
        }
    }
}
