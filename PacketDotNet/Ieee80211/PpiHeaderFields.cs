/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// As defined by Airpcap.h
    /// NOTE: PresentPosition may not be the only position present
    /// as this the field can be extended if the high bit is set
    /// </summary>
    public struct PpiHeaderFields
    {
        /// <summary>Length of the Data Link Type</summary>
        public static readonly int DataLinkTypeLength = 4;

        /// <summary>The data link type position.</summary>
        public static readonly int DataLinkTypePosition;

        /// <summary>The length of the PPI field header</summary>
        public static readonly int FieldHeaderLength = 4;

        /// <summary>Position of the first iField Header</summary>
        public static readonly int FirstFieldPosition;

        /// <summary>Length of the Flags field</summary>
        public static readonly int FlagsLength = 1;

        /// <summary>Position of the Flags field</summary>
        public static readonly int FlagsPosition;

        /// <summary>Length of the length field</summary>
        public static readonly int LengthLength = 2;

        /// <summary>Position of the length field</summary>
        public static readonly int LengthPosition;

        /// <summary>The total length of the ppi packet header</summary>
        public static readonly int PpiPacketHeaderLength;

        /// <summary>Length of the version field</summary>
        public static readonly int VersionLength = 1;

        /// <summary>Position of the version field</summary>
        public static readonly int VersionPosition = 0;

        static PpiHeaderFields()
        {
            FlagsPosition = VersionPosition + VersionLength;
            LengthPosition = FlagsPosition + FlagsLength;
            DataLinkTypePosition = LengthPosition + LengthLength;
            FirstFieldPosition = DataLinkTypePosition + DataLinkTypeLength;
            PpiPacketHeaderLength = FirstFieldPosition;
        }
    }
}