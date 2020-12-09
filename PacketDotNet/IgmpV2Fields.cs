/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>IGMP protocol field encoding information.</summary>
    public struct IgmpV2Fields
    {
        /// <summary>Length of the IGMP header checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the IGMP header checksum.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length of group address in bytes.</summary>
        public static readonly int GroupAddressLength;

        /// <summary>Position of the IGMP group address.</summary>
        public static readonly int GroupAddressPosition;

        /// <summary>Length in bytes of an IGMP header.</summary>
        public static readonly int HeaderLength; // 8

        /// <summary>Length of the IGMP max response code in bytes.</summary>
        public static readonly int MaxResponseTimeLength = 1;

        /// <summary>Position of the IGMP max response code.</summary>
        public static readonly int MaxResponseTimePosition;

        /// <summary>Length of the IGMP message type code in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary>Position of the IGMP message type.</summary>
        public static readonly int TypePosition = 0;

        static IgmpV2Fields()
        {
            GroupAddressLength = IPv4Fields.AddressLength;
            MaxResponseTimePosition = TypePosition + TypeLength;
            ChecksumPosition = MaxResponseTimePosition + MaxResponseTimeLength;
            GroupAddressPosition = ChecksumPosition + ChecksumLength;
            HeaderLength = GroupAddressPosition + GroupAddressLength;
        }
    }
}