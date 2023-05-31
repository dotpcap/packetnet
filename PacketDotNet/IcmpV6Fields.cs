/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// ICMP protocol field encoding information.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    public struct IcmpV6Fields
    {
        /// <summary>Length of the ICMP header checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the ICMP header checksum.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length of the ICMP subcode in bytes.</summary>
        public static readonly int CodeLength = 1;

        /// <summary>Position of the ICMP message subcode.</summary>
        public static readonly int CodePosition;

        /// <summary>Length of the ICMP message type code in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary>Position of the ICMP message type.</summary>
        public static readonly int TypePosition = 0;

        /// <summary>The position of the ICMP message.</summary>
        public static readonly int MessagePosition;

        static IcmpV6Fields()
        {
            CodePosition = TypePosition + TypeLength;
            ChecksumPosition = CodePosition + CodeLength;
            MessagePosition = ChecksumPosition + ChecksumLength;
        }
    }