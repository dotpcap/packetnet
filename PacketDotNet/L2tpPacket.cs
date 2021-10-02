/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2018 Steven Haufe<haufes@hotmail.com>
  */

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
            catch
            {
                //it's not a PPP packet, just attach the data
                PayloadPacketOrData.Value.ByteArraySegment = payload;
            }

            ParentPacket = parentPacket;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        public bool DataMessage => (Header.Bytes[Header.Offset] & 0x8) == 8;

        public bool HasLength => (Header.Bytes[Header.Offset] & 0x4) == 4;

        public bool HasOffset => (Header.Bytes[Header.Offset] & 0x2) == 2;

        public bool HasSequence => (Header.Bytes[Header.Offset] & 0x2) == 2;

        public bool IsPriority => (Header.Bytes[Header.Offset] & 0x2) == 2;

        public int SessionId => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 5) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 4);

        public int TunnelId => HasLength ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 3) : EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);

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
                buffer.AppendFormat("{0}[L2tpPacket", color);
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }



        /// <summary>
        /// Determines whether the payload can be decoded by <see cref="L2tpPacket" />.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="udpPacket">The UDP packet.</param>
        /// <returns>
        /// <c>true</c> if the payload can be decoded by <see cref="L2tpPacket"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanDecode(ByteArraySegment payload, UdpPacket udpPacket)
        {
            if (udpPacket.SourcePort is L2tpFields.Port || udpPacket.DestinationPort is L2tpFields.Port)
            {
                // Ver MUST be 2, indicating the version of the L2TP data message header
                // described in this document. The value 1 is reserved to permit
                // detection of L2F [RFC2341] packets should they arrive intermixed with
                // L2TP packets. Packets received with an unknown Ver field MUST be
                // discarded.

                return (payload[1] & 0x07) is 1 or 2;
            }

            return false;
        }
    }
}