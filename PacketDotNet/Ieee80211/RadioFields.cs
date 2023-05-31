/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet.Ieee80211;

    /// <summary>
    /// As defined by Airpcap.h
    /// NOTE: PresentPosition may not be the only position present
    /// as this the field can be extended if the high bit is set
    /// </summary>
    public struct RadioFields
    {
        /// <summary>Default header length, assuming one present field entry</summary>
        public static readonly int DefaultHeaderLength;

        /// <summary>Length of the length field</summary>
        public static readonly int LengthLength = 2;

        /// <summary>Position of the length field</summary>
        public static readonly int LengthPosition;

        /// <summary>Length of the pad field</summary>
        public static readonly int PadLength = 1;

        /// <summary>Position of the padding field</summary>
        public static readonly int PadPosition;

        /// <summary>Length of the first present field (others may follow)</summary>
        public static readonly int PresentLength = 4;

        /// <summary>Position of the first present field</summary>
        public static readonly int PresentPosition;

        /// <summary>Length of the version field</summary>
        public static readonly int VersionLength = 1;

        /// <summary>Position of the version field</summary>
        public static readonly int VersionPosition = 0;

        static RadioFields()
        {
            PadPosition = VersionPosition + VersionLength;
            LengthPosition = PadPosition + PadLength;
            PresentPosition = LengthPosition + LengthLength;

            // default to the normal header size until the header length can be read
            DefaultHeaderLength = PresentPosition + PresentLength;
        }
    }