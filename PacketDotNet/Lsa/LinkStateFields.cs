/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Lsa;

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LSA header.
    /// </summary>
    public struct LinkStateFields
    {
        /// <summary>The length of the AdvertisingRouterID field in bytes</summary>
        public static readonly int AdvertisingRouterIDLength = 4;

        /// <summary>The relative position of the AdvertisingRouterID field</summary>
        public static readonly int AdvertisingRouterIDPosition;

        /// <summary>The length of the Checksum field in bytes</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>The relative position of the Checksum field</summary>
        public static readonly int ChecksumPosition;

        /// <summary>The relative position of the header's end</summary>
        public static readonly int HeaderEnd;

        /// <summary>The length of the LinkStateID field in bytes</summary>
        public static readonly int LinkStateIDLength = 4;

        /// <summary>The relative position of the LinkStateID field</summary>
        public static readonly int LinkStateIdPosition;

        /// <summary>The length of the LSAge field in bytes</summary>
        public static readonly int LSAgeLength = 2;

        /// <summary>The relative position of the LSAge field</summary>
        public static readonly int LinkStateAgePosition = 0;

        /// <summary>The length of the LSSeqeunceNumber field in bytes</summary>
        public static readonly int LinkStateSequenceNumberLength = 4;

        /// <summary>The relative position of the SequenceNumber field</summary>
        public static readonly int LinkStateSequenceNumberPosition;

        /// <summary>The length of the LSType field in bytes</summary>
        public static readonly int LinkStateTypeLength = 1;

        /// <summary>The relative position of the LSType field</summary>
        public static readonly int LinkStateTypePosition;

        /// <summary>The length of the Options field in bytes</summary>
        public static readonly int OptionsLength = 1;

        /// <summary>The relative position of the Option field</summary>
        public static readonly int OptionsPosition;

        /// <summary>The length of the Length field in bytes</summary>
        public static readonly int PacketLength = 2;

        /// <summary>The relative position of the Length field</summary>
        public static readonly int PacketLengthPosition;

        static LinkStateFields()
        {
            OptionsPosition = LinkStateAgePosition + LSAgeLength;
            LinkStateTypePosition = OptionsPosition + OptionsLength;
            LinkStateIdPosition = LinkStateTypePosition + LinkStateTypeLength;
            AdvertisingRouterIDPosition = LinkStateIdPosition + LinkStateIDLength;
            LinkStateSequenceNumberPosition = AdvertisingRouterIDPosition + AdvertisingRouterIDLength;
            ChecksumPosition = LinkStateSequenceNumberPosition + LinkStateSequenceNumberLength;
            PacketLengthPosition = ChecksumPosition + ChecksumLength;
            HeaderEnd = PacketLength + PacketLengthPosition;
        }
    }