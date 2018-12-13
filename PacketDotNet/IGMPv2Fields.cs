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
    /// <summary> IGMP protocol field encoding information. </summary>
    public class IGMPv2Fields
    {
        /// <summary> Length of the IGMP header checksum in bytes.</summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> Position of the IGMP header checksum.</summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> Length of group address in bytes.</summary>
        public static readonly Int32 GroupAddressLength;

        /// <summary> Position of the IGMP group address.</summary>
        public static readonly Int32 GroupAddressPosition;

        /// <summary> Length in bytes of an IGMP header.</summary>
        public static readonly Int32 HeaderLength; // 8

        /// <summary> Length of the IGMP max response code in bytes.</summary>
        public static readonly Int32 MaxResponseTimeLength = 1;

        /// <summary> Position of the IGMP max response code.</summary>
        public static readonly Int32 MaxResponseTimePosition;

        /// <summary> Length of the IGMP message type code in bytes.</summary>
        public static readonly Int32 TypeLength = 1;

        /// <summary> Position of the IGMP message type.</summary>
        public static readonly Int32 TypePosition = 0;

        static IGMPv2Fields()
        {
            GroupAddressLength = IPv4Fields.AddressLength;
            MaxResponseTimePosition = TypePosition + TypeLength;
            ChecksumPosition = MaxResponseTimePosition + MaxResponseTimeLength;
            GroupAddressPosition = ChecksumPosition + ChecksumLength;
            HeaderLength = GroupAddressPosition + GroupAddressLength;
        }
    }
}