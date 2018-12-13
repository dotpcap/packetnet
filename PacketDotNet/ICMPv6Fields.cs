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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    /// <summary>
    /// ICMP protocol field encoding information.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    public class ICMPv6Fields
    {
        /// <summary> Length of the ICMP header checksum in bytes.</summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> Position of the ICMP header checksum.</summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> Length of the ICMP subcode in bytes.</summary>
        public static readonly Int32 CodeLength = 1;

        /// <summary> Position of the ICMP message subcode.</summary>
        public static readonly Int32 CodePosition;

        /// <summary> Length in bytes of an ICMP header.</summary>
        public static readonly Int32 HeaderLength; // == 4

        /// <summary> Length of the ICMP message type code in bytes.</summary>
        public static readonly Int32 TypeLength = 1;

        /// <summary> Position of the ICMP message type.</summary>
        public static readonly Int32 TypePosition = 0;

        static ICMPv6Fields()
        {
            CodePosition = TypePosition + TypeLength;
            ChecksumPosition = CodePosition + CodeLength;
            HeaderLength = ChecksumPosition + ChecksumLength;
        }
    }
}