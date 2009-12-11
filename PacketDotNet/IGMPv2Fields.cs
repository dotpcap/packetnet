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
namespace PacketDotNet
{
    /// <summary> IGMP protocol field encoding information. </summary>
    public class IGMPv2Fields
    {
        /// <summary> Length of the IGMP message type code in bytes.</summary>
        public readonly static int TypeLength = 1;
        /// <summary> Length of the IGMP max response code in bytes.</summary>
        public readonly static int MaxResponseTimeLength = 1;
        /// <summary> Length of the IGMP header checksum in bytes.</summary>
        public readonly static int ChecksumLength = 2;
        /// <summary> Length of group address in bytes.</summary>
        public readonly static int GroupAddressLength;
        /// <summary> Position of the IGMP message type.</summary>
        public readonly static int TypePosition = 0;
        /// <summary> Position of the IGMP max response code.</summary>
        public readonly static int MaxResponseTimePosition;
        /// <summary> Position of the IGMP header checksum.</summary>
        public readonly static int ChecksumPosition;
        /// <summary> Position of the IGMP group address.</summary>
        public readonly static int GroupAddressPosition;
        /// <summary> Length in bytes of an IGMP header.</summary>
        public readonly static int HeaderLength; // 8

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
