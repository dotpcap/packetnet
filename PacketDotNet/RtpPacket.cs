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


using System.Diagnostics.CodeAnalysis;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// Real-time Transport Protocol (RTP)
    /// See: https://en.wikipedia.org/wiki/Real-time_Transport_Protocol
    /// See: https://wiki.wireshark.org/RTP
    /// </summary>
    public sealed class RtpPacket : Packet
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
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public RtpPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = RtpFields.HeaderLength;

            if (CsrcCount > 0)
                Header.Length += CsrcCount * RtpFields.CsrcIdLength;

            if (HasExtension)
                Header.Length += RtpFields.ProfileSpecificExtensionHeaderLength + RtpFields.ExtensionLengthLength + ExtensionHeaderLength;

            ParentPacket = parentPacket;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment { ByteArraySegment = Header.NextSegment() };
                return result;
            });
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.BlueBackground;

        public int Version => (Header.Bytes[Header.Offset] & RtpFields.VersionMask) >> 6;

        public bool HasPadding => RtpFields.PaddingMask == (Header.Bytes[Header.Offset] & RtpFields.PaddingMask);

        public bool HasExtension => RtpFields.ExtensionFlagMask == (Header.Bytes[Header.Offset] & RtpFields.ExtensionFlagMask);

        public int CsrcCount => (Header.Bytes[Header.Offset] & RtpFields.CsrcCountMask);

        public bool Marker => RtpFields.MarkerMask == (Header.Bytes[Header.Offset + 1] & RtpFields.MarkerMask);

        public int PayloadType => (Header.Bytes[Header.Offset + 1] & RtpFields.PayloadTypeMask);

        public ushort SequenceNumber => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);

        public uint Timestamp => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 4);

        public uint SsrcIdentifier => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 8);

        public int ExtensionHeaderLength => HasExtension ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 12 + CsrcCount * 4 + 2) : 0;

    }
}
