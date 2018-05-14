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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Network Address
    /// </summary>
    public class NetworkAddress
    {
        /// <summary>
        /// Length of AddressFamily field in bytes
        /// </summary>
        internal const Int32 AddressFamilyLength = 1;

        internal ByteArraySegment Data;

        internal Byte[] Bytes
        {
            get
            {
                var addressBytes = Address.GetAddressBytes();
                var data = new Byte[AddressFamilyLength + addressBytes.Length];
                data[0] = (Byte) AddressFamily;
                Array.Copy(addressBytes,
                           0,
                           data,
                           AddressFamilyLength,
                           addressBytes.Length);
                return data;
            }
        }

        /// <summary>
        /// Number of bytes in the NetworkAddress
        /// </summary>
        internal Int32 Length => AddressFamilyLength + Address.GetAddressBytes().Length;


        #region Constructors

        /// <summary>
        /// Creates a Network Address entity
        /// </summary>
        /// <param name="address">
        /// The Network Address
        /// </param>
        public NetworkAddress(IPAddress address)
        {
            Address = address;
        }

        /// <summary>
        /// Create a network address from byte data
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public NetworkAddress(Byte[] bytes, Int32 offset, Int32 length)
        {
            Data = new ByteArraySegment(bytes, offset, length);
        }

        #endregion


        #region Members

        /// <summary>The format of the Network Address</summary>
        public AddressFamily AddressFamily
        {
            get => (AddressFamily) Data.Bytes[Data.Offset];
            set => Data.Bytes[Data.Offset] = (Byte) value;
        }

        private static Int32 LengthFromAddressFamily(AddressFamily addressFamily)
        {
            Int32 length;

            if (addressFamily == AddressFamily.IPv4)
                length = IPv4Fields.AddressLength;
            else if (addressFamily == AddressFamily.IPv6)
                length = IPv6Fields.AddressLength;
            else
                throw new NotImplementedException("Unknown addressFamily of " + addressFamily);


            return length;
        }

        private static AddressFamily AddressFamilyFromSocketAddress(IPAddress address)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return AddressFamily.IPv4;
            }

            return AddressFamily.IPv6;
        }

        /// <summary>The Network Address</summary>
        public IPAddress Address
        {
            get
            {
                var length = LengthFromAddressFamily(AddressFamily);
                var bytes = new Byte[length];
                Array.Copy(Data.Bytes,
                           Data.Offset + AddressFamilyLength,
                           bytes,
                           0,
                           bytes.Length);

                return new IPAddress(bytes);
            }

            set
            {
                // do we have enough bytes for the address?
                var length = LengthFromAddressFamily(AddressFamilyFromSocketAddress(value));
                length += AddressFamilyLength;

                if (Data == null || Data.Length != length)
                {
                    var bytes = new Byte[length];
                    var offset = 0;

                    // allocate enough memory for the new Address
                    Data = new ByteArraySegment(bytes, offset, length);
                }

                AddressFamily = AddressFamilyFromSocketAddress(value);

                var addressBytes = value.GetAddressBytes();
                Array.Copy(addressBytes,
                           0,
                           Data.Bytes,
                           Data.Offset + AddressFamilyLength,
                           addressBytes.Length);
            }
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">
        /// A <see cref="System.Object" />
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean" />
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            var na = (NetworkAddress) obj;

            if (AddressFamily.Equals(na.AddressFamily) &&
                Address.Equals(na.Address))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// GetHashCode() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32" />
        /// </returns>
        public override Int32 GetHashCode()
        {
            return AddressFamily.GetHashCode() + Address.GetHashCode();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
        {
            return $"[NetworkAddress: AddressFamily={AddressFamily}, Address={Address}]";
        }

        #endregion
    }
}