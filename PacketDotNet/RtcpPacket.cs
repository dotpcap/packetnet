/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// RTP Control Protocol
    /// See: https://en.wikipedia.org/wiki/RTP_Control_Protocol
    /// See: https://wiki.wireshark.org/RTCP
    /// </summary>
    public sealed class RtcpPacket : Packet
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Create from values
        /// </summary>
        public RtcpPacket()
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = RtcpFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public RtcpPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment) {Length = RtcpFields.HeaderLength};

            if (Length > 1)
            {
                Header.Length += (Length - 1) * 4;

                var length = Length - 1;
                var rtcpPayload = new byte[length*4];
                Buffer.BlockCopy(Header.Bytes, Header.Offset + RtcpFields.HeaderLength, rtcpPayload, 0, length*4);
                PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment
                {
                    ByteArraySegment = new ByteArraySegment(rtcpPayload, 0, length)
                });
            }

            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Gets or sets the RTP version (2 bits), Indicates the version of the protocol
        /// </summary>
        public int Version
        {
            get => (Header.Bytes[Header.Offset] & RtcpFields.VersionMask) >> 6;
            set => Header.Bytes[Header.Offset] |= (byte) (value << 6);
        }

        /// <summary>
        /// Gets or sets the padding (1 bit). If the padding bit is set, the packet contains one or more
        /// additional padding octets at the end which are not part of the payload.
        /// </summary>
        public bool HasPadding
        {
            get => RtcpFields.PaddingMask == (Header.Bytes[Header.Offset] & RtcpFields.PaddingMask);
            set
            {
                if (value)
                {
                    Header.Bytes[Header.Offset] |= (byte) RtcpFields.PaddingMask;
                }
                else
                {
                    Header.Bytes[Header.Offset] &= (byte) ~RtcpFields.PaddingMask;
                }
            }
        }

        /// <summary>
        /// Gets or sets (5 bits) The number of reception report blocks contained in this packet. A value of zero is valid.
        /// </summary>
        public int ReceptionReportCount
        {
            get => (Header.Bytes[Header.Offset] & RtcpFields.ReceptionReportCountMask);
            set => Header.Bytes[Header.Offset] |= (byte) (value & RtcpFields.ReceptionReportCountMask);
        }

        /// <summary>
        /// Gets or sets the Packet type: (8 bits) Contains a constant to identify RTCP packet type
        /// </summary>
        public ushort PacketType
        {
            get => Header.Bytes[Header.Offset + 1];
            set => Header.Bytes[Header.Offset + 1] = (byte) (value);
        }

        /// <summary>
        /// Gets or sets the Length (16 bits) Indicates the length of this RTCP packet (including the header itself) in 32-bit units minus one
        /// </summary>
        public ushort Length
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 2);
        }

        /// <summary>
        /// Gets or sets the SSRC (32 bits). The SSRC field identifies the synchronization source.
        /// </summary>
        public uint SsrcIdentifier
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 4);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 4);
        }

        public bool IsValid()
        {
            if (Header.Length < RtcpFields.HeaderLength + (Length - 1) * 4)
                return false;

            if (Version != 2)
                return false;
            
            return true;
        }
    }
}
