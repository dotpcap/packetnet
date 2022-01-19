/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>IGMPv3 membership query field encoding information.</summary>
    public struct IgmpV3MembershipQueryFields
    {
        /// <summary>Length of the IGMPv3 header checksum in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the IGMPv3 header checksum.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length of group address in bytes.</summary>
        public static readonly int GroupAddressLength;

        /// <summary>Position of the IGMPv3 group address.</summary>
        public static readonly int GroupAddressPosition;

        /// <summary>Length in bytes of an IGMPv3 membership query header in bytes.</summary>
        public static readonly int HeaderLength; // 12

        /// <summary>Length of the IGMPv3 max response code in bytes.</summary>
        public static readonly int MaxResponseCodeLength = 1;

        /// <summary>Position of the IGMPv3 max response code.</summary>
        public static readonly int MaxResponseCodePosition;

        /// <summary>Length of the IGMPv3 number of sources, in bytes.</summary>
        public static readonly int NumberOfSourcesLength = 2;

        /// <summary>Position of the IGMPv3 number of sources.</summary>
        public static readonly int NumberOfSourcesPosition;

        /// <summary>Length of the IGMPv3 querier's query interval code, in bytes.</summary>
        public static readonly int QueriersQueryIntervalCodeLength = 1;

        /// <summary>Position of the IGMPv3 querier's query interval code.</summary>
        public static readonly int QueriersQueryIntervalCodePosition;

        /// <summary>Length of the IGMPv3 reserved field, suppress router-side processing flag, and querier's robustness variable, in bytes.</summary>
        public static readonly int ReservedSFlagAndQRVLength = 1;

        /// <summary>Position of the IGMPv3 reserved field, suppress router-side processing flag, and querier's robustness variable.</summary>
        public static readonly int ReservedSFlagAndQRVPosition;

        /// <summary>Starting position of the unicast source ip addresses.</summary>
        public static readonly int SourceAddressStart;

        /// <summary>Length of the IGMPv3 message type code in bytes.</summary>
        public static readonly int TypeLength = 1;

        /// <summary>Position of the IGMPv3 message type.</summary>
        public static readonly int TypePosition = 0;

        static IgmpV3MembershipQueryFields()
        {
            GroupAddressLength = IPv4Fields.AddressLength;
            MaxResponseCodePosition = TypePosition + TypeLength;
            ChecksumPosition = MaxResponseCodePosition + MaxResponseCodeLength;
            GroupAddressPosition = ChecksumPosition + ChecksumLength;
            ReservedSFlagAndQRVPosition = GroupAddressPosition + GroupAddressLength;
            QueriersQueryIntervalCodePosition = ReservedSFlagAndQRVPosition + ReservedSFlagAndQRVLength;
            NumberOfSourcesPosition = QueriersQueryIntervalCodePosition + QueriersQueryIntervalCodeLength;
            HeaderLength = NumberOfSourcesPosition + NumberOfSourcesLength;
            SourceAddressStart = HeaderLength;
        }
    }
}
