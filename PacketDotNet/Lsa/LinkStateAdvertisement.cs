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
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa
{
    /// <summary>
    /// The LSA header. All LSAs begin with a common 20 byte header.  This header contains
    /// enough information to uniquely identify the LSA (LS type, Link State
    /// ID, and Advertising Router). See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class LinkStateAdvertisement
    {
        /// <summary>
        /// The Ipv4 bytes count.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int IPv4BytesCount = 4;

        /// <summary>
        /// The length of the network mask.
        /// </summary>
        public const int NetworkMaskLength = 4;

        protected ByteArraySegment Header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinkStateAdvertisement()
        {
            var b = new byte[OspfV2Fields.LSAHeaderLength];
            Header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public LinkStateAdvertisement(byte[] packet, int offset, int length)
        {
            Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The Router ID of the router that originated the LSA.
        /// </summary>
        public IPAddress AdvertisingRouter
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LinkStateFields.AdvertisingRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LinkStateFields.AdvertisingRouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The time in seconds since the LSA was originated.
        /// </summary>
        public ushort Age
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + LinkStateFields.LinkStateAgePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LinkStateFields.LinkStateAgePosition);
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public virtual byte[] Bytes => Header.ActualBytes();

        /// <summary>
        /// The Fletcher checksum of the complete contents of the LSA,
        /// including the LSA header but excluding the LS age field.
        /// </summary>
        public ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + LinkStateFields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LinkStateFields.ChecksumPosition);
        }

        /// <summary>
        /// This field identifies the portion of the internet environment
        /// that is being described by the LSA.  The contents of this field
        /// depend on the LSA's LS type.
        /// </summary>
        public IPAddress Id
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LinkStateFields.LinkStateIdPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LinkStateFields.LinkStateIdPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The length in bytes of the LSA.  This includes the 20 byte LSA
        /// header.
        /// </summary>
        public ushort Length
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + LinkStateFields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LinkStateFields.PacketLengthPosition);
        }

        /// <summary>
        /// The optional capabilities supported by the described portion of the routing domain.
        /// </summary>
        public byte Options
        {
            get => Header.Bytes[Header.Offset + LinkStateFields.OptionsPosition];
            set => Header.Bytes[Header.Offset + LinkStateFields.OptionsPosition] = value;
        }

        /// <summary>
        /// Detects old or duplicate LSAs.  Successive instances of an LSA
        /// are given successive LS sequence numbers.
        /// </summary>
        public uint SequenceNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LinkStateFields.LinkStateSequenceNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LinkStateFields.LinkStateSequenceNumberPosition);
        }

        ///<summary>
        ///The type of the LSA. Each LSA type has a separate advertisement format.
        ///</summary>
        public LinkStateAdvertisementType Type
        {
            get => (LinkStateAdvertisementType) Header.Bytes[Header.Offset + LinkStateFields.LinkStateTypePosition];
            set => Header.Bytes[Header.Offset + LinkStateFields.LinkStateTypePosition] = (byte) value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="LinkStateAdvertisement" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="LinkStateAdvertisement" />.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("LSA Type {0}, Checksum {1:X2}\n", Type, Checksum);
            return builder.ToString();
        }
    }
}