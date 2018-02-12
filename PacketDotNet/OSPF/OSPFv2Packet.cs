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
using System.Text;
using PacketDotNet.PPP;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    ///     OSPFv2 packet.
    /// </summary>
    [Serializable]
    public abstract class OSPFv2Packet : OSPFPacket
    {
#if DEBUG
        private static readonly log4net.ILog Log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive Log;
#pragma warning restore 0169
#endif
        /// <value>
        ///     Version number of this OSPF protocol
        /// </value>
        public static OSPFVersion OSPFVersion = OSPFVersion.OSPFv2;

        /// <summary>
        ///     Default constructor
        /// </summary>
        protected OSPFv2Packet()
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = OSPFv2Fields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            this.Version = OSPFVersion;
        }

        /// <summary>
        ///     Constructs a packet from bytes and offset
        /// </summary>
        protected OSPFv2Packet(Byte[] bytes, Int32 offset)
        {
            Log.Debug("");
            this.HeaderByteArraySegment = new ByteArraySegment(bytes, offset, OSPFv2Fields.HeaderLength);
            this.Version = OSPFVersion;
        }

        /// <summary>
        ///     The OSPF version number.
        /// </summary>
        public OSPFVersion Version
        {
            get => (OSPFVersion) this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.VersionPosition];

            set =>
                this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.VersionPosition] =
                    (Byte) value;
        }

        /// <summary>
        ///     The OSPF packet types - see http://www.ietf.org/rfc/rfc2328.txt for details
        /// </summary>
        public virtual OSPFPacketType Type
        {
            get
            {
                var val = this.HeaderByteArraySegment.Bytes[
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.TypePosition];

                if (Enum.IsDefined(typeof(OSPFPacketType), val))
                    return (OSPFPacketType) val;
                throw new NotImplementedException("No such OSPF packet type " + val);
            }
            set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.TypePosition] =
                (Byte) value;
        }

        /// <summary>
        ///     The length of the OSPF protocol packet in bytes.
        /// </summary>
        public virtual UInt16 PacketLength
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.PacketLengthPosition);
        }

        /// <summary>
        ///     The Router ID of the packet's source.
        /// </summary>
        public virtual IPAddress RouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.RouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.RouterIDPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     Identifies the area that this packet belongs to. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual IPAddress AreaID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.AreaIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.AreaIDPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     The standard IP checksum of the entire contents of the packet,
        ///     except the 64-bit authentication field
        /// </summary>
        public virtual UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.ChecksumPosition);
        }

        /// <summary>
        ///     Authentication procedure. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual UInt16 AuType
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.AuTypePosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.AuTypePosition);
        }

        /// <summary>
        ///     A 64-bit field for use by the authentication scheme
        /// </summary>
        public virtual UInt64 Authentication
        {
            get => EndianBitConverter.Big.ToUInt64(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.AuthorizationPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.AuthorizationPosition);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="OSPFv2Packet" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="OSPFv2Packet" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.AppendFormat("OSPFv2 packet, type {0} ", this.Type);
            packet.AppendFormat("length: {0}, ", this.PacketLength);
            packet.AppendFormat("Checksum: {0:X8}, ", this.Checksum);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        ///     Output the packet information in the specified format
        ///     Normal - outputs the packet info to a single line
        ///     Colored - outputs the packet info to a single line with coloring
        ///     Verbose - outputs detailed info about the packet
        ///     VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override String ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}