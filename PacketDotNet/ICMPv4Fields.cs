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
    public class ICMPv4Fields
    {
        /// <summary> Length of the ICMP message type code in bytes.</summary>
        public readonly static Int32 TypeCodeLength = 2;
        /// <summary> Length of the ICMP header checksum in bytes.</summary>
        public readonly static Int32 ChecksumLength = 2;
        /// <summary> Length of the ICMP ID field in bytes.</summary>
        public readonly static Int32 IDLength = 2;
        /// <summary> Length of the ICMP Sequence field in bytes </summary>
        public readonly static Int32 SequenceLength = 2;

        /// <summary> Position of the ICMP message type/code.</summary>
        public readonly static Int32 TypeCodePosition = 0;
        /// <summary> Position of the ICMP header checksum.</summary>
        public readonly static Int32 ChecksumPosition;
        /// <summary> Position of the ICMP ID field </summary>
        public readonly static Int32 IDPosition;
        /// <summary> Position of the Sequence field </summary>
        public readonly static Int32 SequencePosition;
        /// <summary> Length in bytes of an ICMP header.</summary>
        public readonly static Int32 HeaderLength;

        static ICMPv4Fields()
        {
            TypeCodePosition = 0;
            ChecksumPosition = TypeCodePosition + TypeCodeLength;
            IDPosition = ChecksumPosition + ChecksumLength;
            SequencePosition = IDPosition + IDLength;
            HeaderLength = SequencePosition + SequenceLength;
        }
    }
}
