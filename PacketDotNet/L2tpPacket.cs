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
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet
{
    /// <summary>
    /// An L2TP packet.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class L2tpPacket : Packet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">A <see cref="ByteArraySegment" /></param>
        /// <param name="parentPacket">The parent packet.</param>
        public L2tpPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = L2tpFields.HeaderLength;

            if (HasLength)
                Header.Length += L2tpFields.LengthsLength;

            if (HasSequence)
                Header.Length += L2tpFields.NsLength + L2tpFields.NrLength;

            if (HasOffset)
                Header.Length += L2tpFields.OffsetSizeLength + L2tpFields.OffsetPadLength;

            var payload = Header.NextSegment();
            try
            {
                PayloadPacket = new PppPacket(payload) { ParentPacket = this };
            }
            catch (Exception)
            {
                //it's not a PPP packet, just attach the data
                PayloadPacketOrData.Value.ByteArraySegment = payload;
            }

            ParentPacket = parentPacket;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        public bool DataMessage => 8 == (Header.Bytes[Header.Offset] & 0x8);

        public bool HasLength => 4 == (Header.Bytes[Header.Offset] & 0x4);

        public bool HasOffset => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public bool HasSequence => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public bool IsPriority => 2 == (Header.Bytes[Header.Offset] & 0x2);

        public int SessionID => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 5) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 4);

        public int TunnelID => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 3) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);

        public int Version => Header.Bytes[Header.Offset + 1] & 0x7;

        /// <summary cref="Packet.ToString(StringOutputType)" />
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
                buffer.AppendFormat("{0}[L2tpPacket",
                                    color);
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}