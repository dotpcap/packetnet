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
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An L2TP packet.
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public sealed class L2TPPacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">A <see cref="ByteArraySegment" /></param>
        /// <param name="parentPacket">The parent packet.</param>
        public L2TPPacket(ByteArraySegment bas, Packet parentPacket)
        {
            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = L2TPFields.HeaderLength;

            if (HasLength)
                Header.Length += L2TPFields.LengthsLength;
            if (HasSequence)
                Header.Length += L2TPFields.NsLength + L2TPFields.NrLength;
            if (HasOffset)
                Header.Length += L2TPFields.OffsetSizeLength + L2TPFields.OffsetPadLength;

            var payload = Header.EncapsulatedBytes();
            try
            {
                PayloadPacket = new PPPPacket(payload) {ParentPacket = this};
            }
            catch (Exception)
            {
                //it's not a PPP packet, just attach the data
                PayloadPacketOrData.Value.ByteArraySegment = payload;
            }

            ParentPacket = parentPacket;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;

        public Boolean DataMessage => 8 == (Header.Bytes[Header.Offset] & 0x8);

        public Boolean HasLength => 4 == (Header.Bytes[Header.Offset] & 0x4);

        public Boolean HasOffset => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public Boolean HasSequence => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public Boolean IsPriority => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public Int32 SessionID => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 5) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 4);

        public Int32 TunnelID => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 3) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);

        public Int32 Version => Header.Bytes[Header.Offset + 1] & 0x7;


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
                buffer.AppendFormat("{0}[L2TPPacket",
                                    color);
            }


            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}