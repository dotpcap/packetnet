/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Net;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// OSPFv2 packet.
    /// </summary>
    public abstract class OspfV2Packet : OspfPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning restore 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Default constructor
        /// </summary>
        protected OspfV2Packet()
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = OspfV2Fields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            Version = OspfVersion.OspfV2;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        protected OspfV2Packet(byte[] bytes, int offset)
        {
            Log.Debug("");
            Header = new ByteArraySegment(bytes, offset, OspfV2Fields.HeaderLength);
            Version = OspfVersion.OspfV2;
        }

        /// <summary>
        /// Identifies the area that this packet belongs to. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual IPAddress AreaId
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OspfV2Fields.AreaIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OspfV2Fields.AreaIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// A 64-bit field for use by the authentication scheme
        /// </summary>
        public virtual ulong Authentication
        {
            get => EndianBitConverter.Big.ToUInt64(Header.Bytes, Header.Offset + OspfV2Fields.AuthorizationPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.AuthorizationPosition);
        }

        /// <summary>
        /// Authentication procedure. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual ushort AuType
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OspfV2Fields.AuTypePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.AuTypePosition);
        }

        /// <summary>
        /// The standard IP checksum of the entire contents of the packet,
        /// except the 64-bit authentication field
        /// </summary>
        public virtual ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OspfV2Fields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.ChecksumPosition);
        }

        /// <summary>
        /// The length of the OSPF protocol packet in bytes.
        /// </summary>
        public virtual ushort PacketLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OspfV2Fields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.PacketLengthPosition);
        }

        /// <summary>
        /// The Router ID of the packet's source.
        /// </summary>
        public virtual IPAddress RouterId
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OspfV2Fields.RouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OspfV2Fields.RouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The OSPF packet types - see http://www.ietf.org/rfc/rfc2328.txt for details
        /// </summary>
        public virtual OspfPacketType Type
        {
            get => (OspfPacketType) Header.Bytes[Header.Offset + OspfV2Fields.TypePosition];
            set => Header.Bytes[Header.Offset + OspfV2Fields.TypePosition] = (byte) value;
        }

        /// <summary>
        /// The OSPF version number.
        /// </summary>
        public OspfVersion Version
        {
            get => (OspfVersion) Header.Bytes[Header.Offset + OspfV2Fields.VersionPosition];
            set => Header.Bytes[Header.Offset + OspfV2Fields.VersionPosition] = (byte) value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="OspfV2Packet" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="OspfV2Packet" />.</returns>
        public override string ToString()
        {
            var packet = new StringBuilder();
            packet.AppendFormat("OSPFv2 packet, type {0} ", Type);
            packet.AppendFormat("length: {0}, ", PacketLength);
            packet.AppendFormat("Checksum: {0:X8}, ", Checksum);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        /// Output the packet information in the specified format
        /// Normal - outputs the packet info to a single line
        /// Colored - outputs the packet info to a single line with coloring
        /// Verbose - outputs detailed info about the packet
        /// VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override string ToString(StringOutputType outputFormat)
        {
            return ToString();
        }
    }
}