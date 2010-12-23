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
        internal const int AddressFamilyLength = 1;

        internal ByteArraySegment data;

        #region Constructors

        /// <summary>
        /// Creates a Network Address entity
        /// </summary>
        /// <param name="address">
        /// The Network Address
        /// </param>
        public NetworkAddress(System.Net.IPAddress address)
        {
            Address = address;
        }

        /// <summary>
        /// Create a network address from byte data
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public NetworkAddress(byte[] bytes, int offset, int length)
        {
            data = new ByteArraySegment(bytes, offset, length);
        }

        #endregion

        /// <summary>
        /// Number of bytes in the NetworkAddress
        /// </summary>
        internal int Length
        {
            get
            {
                return AddressFamilyLength + Address.GetAddressBytes().Length;
            }
        }

        internal byte[] Bytes
        {
            get
            {
                var addressBytes = Address.GetAddressBytes();
                var data = new byte[AddressFamilyLength + addressBytes.Length];
                data[0] = (byte)AddressFamily;
                Array.Copy(addressBytes, 0,
                           data, AddressFamilyLength,
                           addressBytes.Length);
                return data;
            }
        }

        #region Members

        /// <summary>The format of the Network Address</summary>
        public LLDP.AddressFamily AddressFamily
        {
            get { return (LLDP.AddressFamily)data.Bytes[data.Offset]; }
            set { data.Bytes[data.Offset] = (byte)value; }
        }

        private static int LengthFromAddressFamily(LLDP.AddressFamily addressFamily)
        {
            int length;

            if(addressFamily == LLDP.AddressFamily.IPv4)
                length = IPv4Fields.AddressLength;
            else if(addressFamily == LLDP.AddressFamily.IPv6)
                length = IPv6Fields.AddressLength;
            else
                throw new System.NotImplementedException("Unknown addressFamily of " + addressFamily);

            return length;
        }

        private static LLDP.AddressFamily AddressFamilyFromSocketAddress(System.Net.IPAddress address)
        {
            if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return AddressFamily.IPv4;
            } else
            {
                return AddressFamily.IPv6;
            }
        }

        /// <summary>The Network Address</summary>
        public System.Net.IPAddress Address
        {
            get
            {
                var length = LengthFromAddressFamily(AddressFamily);
                var bytes = new byte[length];
                Array.Copy(data.Bytes, data.Offset + AddressFamilyLength,
                           bytes, 0,
                           bytes.Length);

                return new System.Net.IPAddress(bytes);
            }

            set
            {
                // do we have enough bytes for the address?
                var length = LengthFromAddressFamily(AddressFamilyFromSocketAddress(value));
                length += AddressFamilyLength;

                if((data == null) || data.Length != length)
                {
                    var bytes = new byte[length];
                    var offset = 0;

                    // allocate enough memory for the new Address
                    data = new ByteArraySegment(bytes, offset, length);
                }

                AddressFamily = AddressFamilyFromSocketAddress(value);

                var addressBytes = value.GetAddressBytes();
                Array.Copy(addressBytes, 0,
                           data.Bytes, data.Offset + AddressFamilyLength,
                           addressBytes.Length);
            }
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">
        /// A <see cref="System.Object"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public override bool Equals (object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var na = (NetworkAddress)obj;

            if(this.AddressFamily.Equals(na.AddressFamily) &&
               this.Address.Equals(na.Address))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// GetHashCode() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public override int GetHashCode ()
        {
            return AddressFamily.GetHashCode() + Address.GetHashCode();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[NetworkAddress: AddressFamily={0}, Address={1}]",
                                 AddressFamily, Address);
        }

        #endregion
    }
}