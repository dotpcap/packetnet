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
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    /// <summary>
    /// IP protocol field encoding information.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct IPv4Fields
    {
        /// <summary> Width of the IP version and header length field in bytes.</summary>
        public static readonly Int32 VersionAndHeaderLengthLength = 1;

        /// <summary> Width of the Differentiated Services / Type of service field in bytes.</summary>
        public static readonly Int32 DifferentiatedServicesLength = 1;

        /// <summary> Width of the total length field in bytes.</summary>
        public static readonly Int32 TotalLengthLength = 2;

        /// <summary> Width of the ID field in bytes.</summary>
        public static readonly Int32 IdLength = 2;

        /// <summary> Width of the fragment offset bits and offset field in bytes.</summary>
        public static readonly Int32 FragmentOffsetAndFlagsLength = 2;

        /// <summary> Width of the TTL field in bytes.</summary>
        public static readonly Int32 TtlLength = 1;

        /// <summary> Width of the IP protocol code in bytes.</summary>
        public static readonly Int32 ProtocolLength = 1;

        /// <summary> Width of the IP checksum in bytes.</summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> Position of the version code and header length within the IP header.</summary>
        public static readonly Int32 VersionAndHeaderLengthPosition = 0;

        /// <summary> Position of the differentiated services value within the IP header.</summary>
        public static readonly Int32 DifferentiatedServicesPosition;

        /// <summary> Position of the header length within the IP header.</summary>
        public static readonly Int32 TotalLengthPosition;

        /// <summary> Position of the packet ID within the IP header.</summary>
        public static readonly Int32 IdPosition;

        /// <summary> Position of the flag bits and fragment offset within the IP header.</summary>
        public static readonly Int32 FragmentOffsetAndFlagsPosition;

        /// <summary> Position of the ttl within the IP header.</summary>
        public static readonly Int32 TtlPosition;

        /// <summary>
        /// Position of the protocol used within the IP data
        /// </summary>
        public static readonly Int32 ProtocolPosition;

        /// <summary> Position of the checksum within the IP header.</summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> Position of the source IP address within the IP header.</summary>
        public static readonly Int32 SourcePosition;

        /// <summary> Position of the destination IP address within a packet.</summary>
        public static readonly Int32 DestinationPosition;

        /// <summary> Length in bytes of an IP header, excluding options.</summary>
        public static readonly Int32 HeaderLength; // == 20

        /// <summary>
        /// Number of bytes in an IPv4 address
        /// </summary>
        public static readonly Int32 AddressLength = 4;

        static IPv4Fields()
        {
            DifferentiatedServicesPosition = VersionAndHeaderLengthPosition + VersionAndHeaderLengthLength;
            TotalLengthPosition = DifferentiatedServicesPosition + DifferentiatedServicesLength;
            IdPosition = TotalLengthPosition + TotalLengthLength;
            FragmentOffsetAndFlagsPosition = IdPosition + IdLength;
            TtlPosition = FragmentOffsetAndFlagsPosition + FragmentOffsetAndFlagsLength;
            ProtocolPosition = TtlPosition + TtlLength;
            ChecksumPosition = ProtocolPosition + ProtocolLength;
            SourcePosition = ChecksumPosition + ChecksumLength;
            DestinationPosition = SourcePosition + AddressLength;
            HeaderLength = DestinationPosition + AddressLength;
        }
    }
}