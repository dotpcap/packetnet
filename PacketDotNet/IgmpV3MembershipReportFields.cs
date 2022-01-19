/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>IGMPv3 membership report field encoding information.</summary>
    public struct IgmpV3MembershipReportFields
    {
        /// <summary>Length of the IGMPv3 header checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the IGMPv3 header checksum.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Starting position of the group records.</summary>
        public static readonly int GroupRecordStart;

        /// <summary>Length in bytes of an IGMPv3 header in bytes.</summary>
        public static readonly int HeaderLength; // 8

        /// <summary>Length of the IGMPv3 number of group records, in bytes.</summary>
        public static readonly int NumberOfGroupRecordsLength = 2;

        /// <summary>Position of the IGMPv3 number of group records.</summary>
        public static readonly int NumberOfGroupRecordsPosition;

        /// <summary>Length of the first IGMPv3 reserved field in bytes.</summary>
        public static readonly int Reserved1Length = 1;

        /// <summary>Length of the second IGMPv3 reserved field in bytes.</summary>
        public static readonly int Reserved2Length = 2;

        /// <summary>Length of the IGMPv3 message type code in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary>Position of the IGMPv3 message type.</summary>
        public static readonly int TypePosition = 0;

        static IgmpV3MembershipReportFields()
        {
            ChecksumPosition = TypePosition + TypeLength + Reserved1Length;
            NumberOfGroupRecordsPosition = ChecksumPosition + ChecksumLength + Reserved2Length;
            HeaderLength = NumberOfGroupRecordsPosition + NumberOfGroupRecordsLength;
            GroupRecordStart = HeaderLength;
        }
    }
}
