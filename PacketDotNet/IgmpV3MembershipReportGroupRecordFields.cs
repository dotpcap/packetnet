/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>IGMPv3 group record field encoding information.</summary>
    public struct IgmpV3MembershipReportGroupRecordFields
    {
        /// <summary>Length of the IGMPv3 group record auxiliary data Length in bytes.</summary>
        public static readonly int AuxiliaryDataLengthLength = 1;

        /// <summary>Position of the IGMPv3 group record auxiliary data Length.</summary>
        public static readonly int AuxiliaryDataLengthPosition;

        /// <summary>Length of the IGMPv3 membership report group record header.</summary>
        public static readonly int IgmpV3MembershipReportGroupRecordHeaderLength;

        /// <summary>Length of the IGMPv3 group record multicast address in bytes.</summary>
        public static readonly int MulticastAddressLength;

        /// <summary>Position of the IGMPv3 group record multicast address.</summary>
        public static readonly int MulticastAddressPosition;

        /// <summary>Length of the IGMPv3 group record number of sources, in bytes.</summary>
        public static readonly int NumberOfSourcesLength = 2;

        /// <summary>Position of the IGMPv3 group record number of sources.</summary>
        public static readonly int NumberOfSourcesPosition;

        /// <summary>Length of the IGMPv3 group record type in bytes.</summary>
        public static readonly int RecordTypeLength = 1;

        /// <summary>Position of the IGMPv3 group record type.</summary>
        public static readonly int RecordTypePosition = 0;

        /// <summary>Starting position of the group record unicast source ip addresses.</summary>
        public static readonly int SourceAddressStart;

        static IgmpV3MembershipReportGroupRecordFields()
        {
            MulticastAddressLength = IPv4Fields.AddressLength;
            AuxiliaryDataLengthPosition = RecordTypePosition + RecordTypeLength;
            NumberOfSourcesPosition = AuxiliaryDataLengthPosition + AuxiliaryDataLengthLength;
            MulticastAddressPosition = NumberOfSourcesPosition + NumberOfSourcesLength;
            IgmpV3MembershipReportGroupRecordHeaderLength = MulticastAddressPosition + MulticastAddressLength;
            SourceAddressStart = IgmpV3MembershipReportGroupRecordHeaderLength;
        }
    }
}