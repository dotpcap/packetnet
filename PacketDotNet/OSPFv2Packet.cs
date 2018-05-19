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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

using System;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// OSPFv2 packet.
    /// </summary>
    [Serializable]
    public abstract class OSPFv2Packet : OSPFPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive Log;
#pragma warning restore 0169
#endif
        /// <value>
        /// Version number of this OSPF protocol
        /// </value>
        public static OSPFVersion OSPFVersion = OSPFVersion.OSPFv2;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected OSPFv2Packet()
        {
            Log.Debug("");

            // allocate memory for this packet
            var offset = 0;
            var length = OSPFv2Fields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            Version = OSPFVersion;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        protected OSPFv2Packet(Byte[] bytes, Int32 offset)
        {
            Log.Debug("");
            Header = new ByteArraySegment(bytes, offset, OSPFv2Fields.HeaderLength);
            Version = OSPFVersion;
        }

        /// <summary>
        /// The OSPF version number.
        /// </summary>
        public OSPFVersion Version
        {
            get => (OSPFVersion) Header.Bytes[Header.Offset + OSPFv2Fields.VersionPosition];

            set => Header.Bytes[Header.Offset + OSPFv2Fields.VersionPosition] = (Byte) value;
        }

        /// <summary>
        /// The OSPF packet types - see http://www.ietf.org/rfc/rfc2328.txt for details
        /// </summary>
        public virtual OSPFPacketType Type
        {
            get => (OSPFPacketType) Header.Bytes[Header.Offset + OSPFv2Fields.TypePosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.TypePosition] = (Byte) value;
        }

        /// <summary>
        /// The length of the OSPF protocol packet in bytes.
        /// </summary>
        public virtual UInt16 PacketLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.PacketLengthPosition);
        }

        /// <summary>
        /// The Router ID of the packet's source.
        /// </summary>
        public virtual IPAddress RouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.RouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.RouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Identifies the area that this packet belongs to. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual IPAddress AreaID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.AreaIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.AreaIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The standard IP checksum of the entire contents of the packet,
        /// except the 64-bit authentication field
        /// </summary>
        public virtual UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.ChecksumPosition);
        }

        /// <summary>
        /// Authentication procedure. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual UInt16 AuType
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.AuTypePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.AuTypePosition);
        }

        /// <summary>
        /// A 64-bit field for use by the authentication scheme
        /// </summary>
        public virtual UInt64 Authentication
        {
            get => EndianBitConverter.Big.ToUInt64(Header.Bytes, Header.Offset + OSPFv2Fields.AuthorizationPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.AuthorizationPosition);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2Packet" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2Packet" />.</returns>
        public override String ToString()
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
        public override String ToString(StringOutputType outputFormat)
        {
            return ToString();
        }
    }
}