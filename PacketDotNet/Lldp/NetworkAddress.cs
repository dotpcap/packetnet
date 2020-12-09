/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Net;
using PacketDotNet.Utils;

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Network Address
    /// </summary>
    public class NetworkAddress
    {
        /// <summary>
        /// Length of IanaAddressFamily field in bytes
        /// </summary>
        internal const int AddressFamilyLength = 1;

        internal ByteArraySegment Data;

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
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public NetworkAddress(byte[] bytes, int offset, int length)
        {
            Data = new ByteArraySegment(bytes, offset, length);
        }

        /// <summary>The Network Address</summary>
        public IPAddress Address
        {
            get
            {
                var length = GetAddressFamilyLength(AddressFamily);
                var bytes = new byte[length];
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
                var length = GetAddressFamilyLength(AddressFamilyFromSocketAddress(value));
                length += AddressFamilyLength;

                if (Data == null || Data.Length != length)
                {
                    var bytes = new byte[length];
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

        /// <summary>The format of the Network Address</summary>
        public IanaAddressFamily AddressFamily
        {
            get => (IanaAddressFamily) Data.Bytes[Data.Offset];
            set => Data.Bytes[Data.Offset] = (byte) value;
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        internal byte[] Bytes
        {
            get
            {
                var addressBytes = Address.GetAddressBytes();
                var data = new byte[AddressFamilyLength + addressBytes.Length];
                data[0] = (byte) AddressFamily;
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
        internal int Length => AddressFamilyLength + Address.GetAddressBytes().Length;

        /// <summary>
        /// Gets the length of the address family.
        /// </summary>
        /// <param name="addressFamily">The address family.</param>
        private static int GetAddressFamilyLength(IanaAddressFamily addressFamily)
        {
            int length;

            switch (addressFamily)
            {
                case IanaAddressFamily.IPv4:
                {
                    length = IPv4Fields.AddressLength;
                    break;
                }
                case IanaAddressFamily.IPv6:
                {
                    length = IPv6Fields.AddressLength;
                    break;
                }
                default:
                {
                    throw new NotImplementedException("Unknown addressFamily of " + addressFamily);
                }
            }

            return length;
        }

        private static IanaAddressFamily AddressFamilyFromSocketAddress(IPAddress address)
        {
            return address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? IanaAddressFamily.IPv4 : IanaAddressFamily.IPv6;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">
        /// A <see cref="object" />
        /// </param>
        /// <returns>
        /// A <see cref="bool" />
        /// </returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;


            var na = (NetworkAddress) obj;

            return AddressFamily.Equals(na.AddressFamily) && Address.Equals(na.Address);
        }

        /// <summary>
        /// GetHashCode() override
        /// </summary>
        /// <returns>
        /// A <see cref="int" />
        /// </returns>
        public override int GetHashCode()
        {
            return AddressFamily.GetHashCode() + Address.GetHashCode();
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return $"[NetworkAddress: IanaAddressFamily={AddressFamily}, Address={Address}]";
        }
    }
}