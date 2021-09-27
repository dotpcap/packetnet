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
        /// Create from values
        /// </summary>
        public RtpPacket()
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = RtpFields.HeaderLength;
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
        public RtpPacket(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Log.Debug("");

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment) {Length = RtpFields.HeaderLength};

            if (CsrcCount > 0)
                Header.Length += CsrcCount * RtpFields.CsrcIdLength;

            if (HasExtension)
                Header.Length += RtpFields.ProfileSpecificExtensionHeaderLength + RtpFields.ExtensionLengthLength +
                                 ExtensionHeaderLength;

            ParentPacket = parentPacket;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment {ByteArraySegment = Header.NextSegment()};
                return result;
            });
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.BlueBackground;

        /// <summary>
        /// Gets or sets the RTP version (2 bits), Indicates the version of the protocol
        /// </summary>
        public int Version
        {
            get => (Header.Bytes[Header.Offset] & RtpFields.VersionMask) >> 6;
            set => Header.Bytes[Header.Offset] |= (byte) (value << 6);
        }

        /// <summary>
        /// Gets or sets the padding (1 bit). If the padding bit is set, the packet contains one or more
        /// additional padding octets at the end which are not part of the payload.
        /// </summary>
        public bool HasPadding
        {
            get => RtpFields.PaddingMask == (Header.Bytes[Header.Offset] & RtpFields.PaddingMask);
            set
            {
                if (value)
                {
                    Header.Bytes[Header.Offset] |= (byte) RtpFields.PaddingMask;
                }
                else
                {
                    Header.Bytes[Header.Offset] &= (byte) ~RtpFields.PaddingMask;
                }
            }
        }

        /// <summary>
        /// Gets or sets the extension (1 bit). If the extension bit is set, the fixed header MUST be followed by exactly one header extension
        /// </summary>
        public bool HasExtension
        {
            get => RtpFields.ExtensionFlagMask == (Header.Bytes[Header.Offset] & RtpFields.ExtensionFlagMask);
            set
            {
                if (value)
                {
                    Header.Bytes[Header.Offset] |= (byte) RtpFields.ExtensionFlagMask;
                }
                else
                {
                    Header.Bytes[Header.Offset] &= (byte) ~RtpFields.ExtensionFlagMask;
                }
            }
        }

        /// <summary>
        /// Gets or sets the CSRC count (4 bits). The CSRC count contains the number of CSRC identifiers that follow the fixed header.
        /// </summary>
        public int CsrcCount
        {
            get => (Header.Bytes[Header.Offset] & RtpFields.CsrcCountMask);
            set
            {
                Header.Bytes[Header.Offset] |= (byte) (value & RtpFields.CsrcCountMask);
                Header.Length = RtpFields.HeaderLength + CsrcCount * RtpFields.CsrcIdLength;
                if (HasExtension)
                    Header.Length += RtpFields.ProfileSpecificExtensionHeaderLength + RtpFields.ExtensionLengthLength +
                                     ExtensionHeaderLength;
            }
        }

        /// <summary>
        /// Gets or sets the Marker (1 bit). The interpretation of the marker is defined by a profile.
        /// </summary>
        public bool Marker
        {
            get => RtpFields.MarkerMask == (Header.Bytes[Header.Offset + 1] & RtpFields.MarkerMask);
            set
            {
                if (value)
                {
                    Header.Bytes[Header.Offset + 1] |= (byte) RtpFields.MarkerMask;
                }
                else
                {
                    Header.Bytes[Header.Offset + 1] &= (byte) ~RtpFields.MarkerMask;
                }
            }
        }

        /// <summary>
        /// Gets or sets the payload type (7 bits) This field identifies the format of the RTP payload and determines its interpretation by the application.
        /// </summary>
        public int PayloadType
        {
            get => (Header.Bytes[Header.Offset + 1] & RtpFields.PayloadTypeMask);
            set => Header.Bytes[Header.Offset + 1] |= (byte) (value & RtpFields.PayloadTypeMask);
        }

        /// <summary>
        /// Gets or sets the sequence number (16 bits). The sequence number increments by one for each RTP data packet sent, and may be used by the receiver
        /// to detect packet loss and to restore packet sequence.
        /// </summary>
        public ushort SequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 2);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 2);
        }

        /// <summary>
        /// Gets or sets the timestamp (32 bits). The timestamp reflects the sampling instant of the first octet in the RTP data packet.
        /// </summary>
        public uint Timestamp
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 4);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 4);
        }

        /// <summary>
        /// Gets or sets the SSRC (32 bits). The SSRC field identifies the synchronization source.
        /// </summary>
        public uint SsrcIdentifier
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 8);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 8);
        }

        /// <summary>
        /// Gets or sets the CSRC list, 0 to 15 items, 32 bits each
        /// The CSRC list identifies the contributing sources for the payload contained in this packet.
        /// </summary>
        public uint[] CsrcIdentifiers
        {
            get
            {
                var ids = new uint[CsrcCount];
                for (int i = 0; i < CsrcCount; i++)
                {
                    ids[i] = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + 12 + i*4);
                }
                return ids;
            }

            set
            {
                CsrcCount = value.Length;
                for (int i = 0; i < value.Length; i++)
                {
                    EndianBitConverter.Big.CopyBytes(value[i], Header.Bytes, Header.Offset + 12 + i*4);
                }
            }
        }

        /// <summary>
        /// Gets or sets the length of the extension
        /// </summary>
        public ushort ExtensionHeaderLength {
            get => HasExtension ? EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + 12 + CsrcCount * 4 + RtpFields.ProfileSpecificExtensionHeaderLength) : (ushort)0;
            set
            {
                if (value > 0)
                {
                    EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + 12 + CsrcCount * 4 + RtpFields.ProfileSpecificExtensionHeaderLength);
                    Header.Length = RtpFields.HeaderLength + CsrcCount * RtpFields.CsrcIdLength;
                    Header.Length += RtpFields.ProfileSpecificExtensionHeaderLength + RtpFields.ExtensionLengthLength +
                                     value;
                }
                else
                {
                    HasExtension = false;
                    Header.Length = RtpFields.HeaderLength + CsrcCount * RtpFields.CsrcIdLength;
                }
            }
        } 
    }
}
