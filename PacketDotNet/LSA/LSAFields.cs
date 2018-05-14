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

using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LSA header.
    /// </summary>
    public class LSAFields
    {
        /// <summary> The length of the AdvertisingRouterID field in bytes</summary>
        public static readonly Int32 AdvertisingRouterIDLength = 4;

        /// <summary> The relative postion of the AdvertisingRouterID field</summary>
        public static readonly Int32 AdvertisingRouterIDPosition;

        /// <summary> The length of the Checksum field in bytes</summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> The relative postion of the Checksum field</summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> The relative postion of the header's end</summary>
        public static readonly Int32 HeaderEnd;

        /// <summary> The length of the LinkStateID field in bytes</summary>
        public static readonly Int32 LinkStateIDLength = 4;

        /// <summary> The relative postion of the LinkStateID field</summary>
        public static readonly Int32 LinkStateIDPosition;

        /// <summary> The length of the LSAge field in bytes</summary>
        public static readonly Int32 LSAgeLength = 2;

        /// <summary> The relative postion of the LSAge field</summary>
        public static readonly Int32 LSAgePosition = 0;

        /// <summary> The length of the LSSeqeunceNumber field in bytes</summary>
        public static readonly Int32 LSSequenceNumberLength = 4;

        /// <summary> The relative postion of the LSSequenceNumber field</summary>
        public static readonly Int32 LSSequenceNumberPosition;

        /// <summary> The length of the LSType field in bytes</summary>
        public static readonly Int32 LSTypeLength = 1;

        /// <summary> The relative postion of the LSType field</summary>
        public static readonly Int32 LSTypePosition;

        /// <summary> The length of the Options field in bytes</summary>
        public static readonly Int32 OptionsLength = 1;

        /// <summary> The relative postion of the Option field</summary>
        public static readonly Int32 OptionsPosition;

        /// <summary> The length of the Length field in bytes</summary>
        public static readonly Int32 PacketLength = 2;

        /// <summary> The relative postion of the Length field</summary>
        public static readonly Int32 PacketLengthPosition;

        static LSAFields()
        {
            OptionsPosition = LSAgePosition + LSAgeLength;
            LSTypePosition = OptionsPosition + OptionsLength;
            LinkStateIDPosition = LSTypePosition + LSTypeLength;
            AdvertisingRouterIDPosition = LinkStateIDPosition + LinkStateIDLength;
            LSSequenceNumberPosition = AdvertisingRouterIDPosition + AdvertisingRouterIDLength;
            ChecksumPosition = LSSequenceNumberPosition + LSSequenceNumberLength;
            PacketLengthPosition = ChecksumPosition + ChecksumLength;
            HeaderEnd = PacketLength + PacketLengthPosition;
        }
    }
}