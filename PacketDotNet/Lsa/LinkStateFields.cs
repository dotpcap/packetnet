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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

namespace PacketDotNet.Lsa
{
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
}