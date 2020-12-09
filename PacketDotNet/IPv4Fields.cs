/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet
{
    /// <summary>
    /// IP protocol field encoding information.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct IPv4Fields
    {
        /// <summary>Width of the IP version and header length field in bytes.</summary>
        public static readonly int VersionAndHeaderLengthLength = 1;

        /// <summary>Width of the Differentiated Services / Type of service field in bytes.</summary>
        public static readonly int DifferentiatedServicesLength = 1;

        /// <summary>Width of the total length field in bytes.</summary>
        public static readonly int TotalLengthLength = 2;

        /// <summary>Width of the ID field in bytes.</summary>
        public static readonly int IdLength = 2;

        /// <summary>Width of the fragment offset bits and offset field in bytes.</summary>
        public static readonly int FragmentOffsetAndFlagsLength = 2;

        /// <summary>Width of the TTL field in bytes.</summary>
        public static readonly int TtlLength = 1;

        /// <summary>Width of the IP protocol code in bytes.</summary>
        public static readonly int ProtocolLength = 1;

        /// <summary>Width of the IP checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the version code and header length within the IP header.</summary>
        public static readonly int VersionAndHeaderLengthPosition = 0;

        /// <summary>Position of the differentiated services value within the IP header.</summary>
        public static readonly int DifferentiatedServicesPosition;

        /// <summary>Position of the header length within the IP header.</summary>
        public static readonly int TotalLengthPosition;

        /// <summary>Position of the packet ID within the IP header.</summary>
        public static readonly int IdPosition;

        /// <summary>Position of the flag bits and fragment offset within the IP header.</summary>
        public static readonly int FragmentOffsetAndFlagsPosition;

        /// <summary>Position of the ttl within the IP header.</summary>
        public static readonly int TtlPosition;

        /// <summary>
        /// Position of the protocol used within the IP data
        /// </summary>
        public static readonly int ProtocolPosition;

        /// <summary>Position of the checksum within the IP header.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Position of the source IP address within the IP header.</summary>
        public static readonly int SourcePosition;

        /// <summary>Position of the destination IP address within a packet.</summary>
        public static readonly int DestinationPosition;

        /// <summary>Length in bytes of an IP header, excluding options.</summary>
        public static readonly int HeaderLength; // == 20

        /// <summary>
        /// Number of bytes in an IPv4 address
        /// </summary>
        public static readonly int AddressLength = 4;

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