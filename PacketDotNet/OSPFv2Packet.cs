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
using System.Text;
using PacketDotNet;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// OSPFv2 packet.
    /// </summary>
    [Serializable]
    public abstract class  OSPFv2Packet : OSPFPacket
    {

#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif
        /// <value>
        /// Version number of this OSPF protocol
        /// </value>
        public static OSPFVersion ospfVersion = OSPFVersion.OSPFv2;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OSPFv2Packet()
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = OSPFv2Fields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            this.Version = ospfVersion;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        public OSPFv2Packet(byte[] Bytes, int Offset)
        {
            log.Debug("");
            header = new ByteArraySegment(Bytes, Offset, OSPFv2Fields.HeaderLength);
            this.Version = ospfVersion;
        }

        /// <summary>
        /// The OSPF version number.
        /// </summary>
        public OSPFVersion Version
        {
            get
            {
                return (OSPFVersion)header.Bytes[header.Offset + OSPFv2Fields.VersionPosition];
            }

            set
            {
                header.Bytes[header.Offset + OSPFv2Fields.VersionPosition] = (byte)value;
            }
        }

        /// <summary>
        /// The OSPF packet types - see http://www.ietf.org/rfc/rfc2328.txt for details
        /// </summary>
        public virtual OSPFPacketType Type
        {
            get
            {
                var val = header.Bytes[header.Offset + OSPFv2Fields.TypePosition];

                if (Enum.IsDefined(typeof(OSPFPacketType), val))
                    return (OSPFPacketType)val;
                else
                    throw new NotImplementedException("No such OSPF packet type " + val);
            }
            set
            {
                header.Bytes[header.Offset + OSPFv2Fields.TypePosition] = (byte)value;
            }
        }

        /// <summary>
        /// The length of the OSPF protocol packet in bytes.
        /// </summary>
        public virtual ushort PacketLength
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + OSPFv2Fields.PacketLengthPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + OSPFv2Fields.PacketLengthPosition);
            }
        }

        /// <summary>
        /// The Router ID of the packet's source.
        /// </summary>
        public virtual System.Net.IPAddress RouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + OSPFv2Fields.RouterIDPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + OSPFv2Fields.RouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Identifies the area that this packet belongs to. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual System.Net.IPAddress AreaID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(header.Bytes, header.Offset + OSPFv2Fields.AreaIDPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + OSPFv2Fields.AreaIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The standard IP checksum of the entire contents of the packet,
        /// except the 64-bit authentication field
        /// </summary>
        public virtual ushort Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + OSPFv2Fields.ChecksumPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + OSPFv2Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// Authentication procedure. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual ushort AuType
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + OSPFv2Fields.AuTypePosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + OSPFv2Fields.AuTypePosition);
            }
        }

        ///<summary>
        /// A 64-bit field for use by the authentication scheme
        /// </summary>
        public virtual ulong Authentication
        {
            get
            {
                return EndianBitConverter.Big.ToUInt64(header.Bytes, header.Offset + OSPFv2Fields.AuthorizationPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + OSPFv2Fields.AuthorizationPosition);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2Packet"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2Packet"/>.</returns>
        public override string ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.AppendFormat("OSPFv2 packet, type {0} ", this.Type);
            packet.AppendFormat("length: {0}, ", this.PacketLength);
            packet.AppendFormat("Checksum: {0:X8}, ", this.Checksum);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        /// Output the packet information in the specified format
        ///  Normal - outputs the packet info to a single line
        ///  Colored - outputs the packet info to a single line with coloring
        ///  Verbose - outputs detailed info about the packet
        ///  VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override string ToString(StringOutputType outputFormat)
        {
            return ToString();
        }
    }
}
