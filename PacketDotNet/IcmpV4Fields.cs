/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// ICMP protocol field encoding information.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    public struct IcmpV4Fields
    {
        /// <summary>Length of the ICMP header checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the ICMP header checksum.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length in bytes of an ICMP header.</summary>
        public static readonly int HeaderLength;

        /// <summary>Length of the ICMP ID field in bytes.</summary>
        public static readonly int IdLength = 2;

        /// <summary>Position of the ICMP ID field </summary>
        public static readonly int IdPosition;

        /// <summary>Length of the ICMP Sequence field in bytes </summary>
        public static readonly int SequenceLength = 2;

        /// <summary>Position of the Sequence field </summary>
        public static readonly int SequencePosition;

        /// <summary>Length of the ICMP message type code in bytes.</summary>
        public static readonly int TypeCodeLength = 2;

        /// <summary>Position of the ICMP message type/code.</summary>
        public static readonly int TypeCodePosition;

        static IcmpV4Fields()
        {
            TypeCodePosition = 0;
            ChecksumPosition = TypeCodePosition + TypeCodeLength;
            IdPosition = ChecksumPosition + ChecksumLength;
            SequencePosition = IdPosition + IdLength;
            HeaderLength = SequencePosition + SequenceLength;
        }
    }
}