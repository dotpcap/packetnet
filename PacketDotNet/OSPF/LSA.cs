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
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// The LSA header. All LSAs begin with a common 20 byte header.  This header contains
    /// enough information to uniquely identify the LSA (LS type, Link State
    /// ID, and Advertising Router). See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class LSA
    {
        internal ByteArraySegment Header;

        /// <summary>
        /// The I pv4 bytes count.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const Int32 IPv4BytesCount = 4;

        /// <summary>
        /// The length of the network mask.
        /// </summary>
        public const Int32 NetworkMaskLength = 4;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LSA()
        {
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderLength];
            this.Header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public LSA(Byte[] packet, Int32 offset, Int32 length)
        {
            this.Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The time in seconds since the LSA was originated.
        /// </summary>
        public UInt16 LSAge
        {
            get => EndianBitConverter.Big.ToUInt16(this.Header.Bytes, this.Header.Offset + LSAFields.LSAgePosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.Header.Bytes, this.Header.Offset + LSAFields.LSAgePosition);
        }

        /// <summary>
        /// The optional capabilities supported by the described portion of the routing domain.
        /// </summary>
        public Byte Options
        {
            get => this.Header.Bytes[this.Header.Offset + LSAFields.OptionsPosition];
            set => this.Header.Bytes[this.Header.Offset + LSAFields.OptionsPosition] = value;
        }

        ///<summary>
        ///The type of the LSA.  Each LSA type has a separate advertisement format.
        ///</summary>
        public LSAType LSType
        {
            get => (LSAType) this.Header.Bytes[this.Header.Offset + LSAFields.LSTypePosition];
            set => this.Header.Bytes[this.Header.Offset + LSAFields.LSTypePosition] = (Byte)value;
        }

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSA.  The contents of this field
        /// depend on the LSA's LS type.
        /// </summary>
        public IPAddress LinkStateID
        {
            get
            {

                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes, this.Header.Offset + LSAFields.LinkStateIDPosition);
                return new IPAddress(val);

            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           this.Header.Bytes, this.Header.Offset + LSAFields.LinkStateIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The Router ID of the router that originated the LSA.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes, this.Header.Offset + LSAFields.AdvertisingRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           this.Header.Bytes, this.Header.Offset + LSAFields.AdvertisingRouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Detects old or duplicate LSAs.  Successive instances of an LSA
        /// are given successive LS sequence numbers.
        /// </summary>
        public UInt32 LSSequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(this.Header.Bytes, this.Header.Offset + LSAFields.LSSequenceNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.Header.Bytes, this.Header.Offset + LSAFields.LSSequenceNumberPosition);
        }

        /// <summary>
        /// The Fletcher checksum of the complete contents of the LSA,
        /// including the LSA header but excluding the LS age field.
        /// </summary>
        public UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(this.Header.Bytes, this.Header.Offset + LSAFields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.Header.Bytes, this.Header.Offset + LSAFields.ChecksumPosition);
        }

        /// <summary>
        /// The length in bytes of the LSA.  This includes the 20 byte LSA
        /// header.
        /// </summary>
        public UInt16 Length
        {
            get => EndianBitConverter.Big.ToUInt16(this.Header.Bytes, this.Header.Offset + LSAFields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.Header.Bytes, this.Header.Offset + LSAFields.PacketLengthPosition);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="LSA"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="LSA"/>.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("LSA Type {0}, Checksum {1:X2}\n", this.LSType, this.Checksum);
            return builder.ToString();
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual Byte[] Bytes => this.Header.ActualBytes();
    }
}
