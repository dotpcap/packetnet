/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    public struct RtpFields
    {
        /// <summary>Length of the Base Header in bytes.</summary>
        public static readonly int HeaderLength = 12;

        /// <summary>Length of the CRSC in bytes.</summary>
        public static readonly int CsrcIdLength = 4;

        /// <summary>Length of the Profile-specific extension header ID in bytes.</summary>
        public static readonly int ProfileSpecificExtensionHeaderLength = 2;

        /// <summary>Length of the Extension Length field Length in bytes.</summary>
        public static readonly int ExtensionLengthLength = 2;

        // flag bit masks
        public static readonly int VersionMask = 0xC0;
        public static readonly int PaddingMask = 0x20;
        public static readonly int ExtensionFlagMask = 0x10;
        public static readonly int CsrcCountMask = 0xF;
        public static readonly int MarkerMask = 0x80;
        public static readonly int PayloadTypeMask = 0x7F;
    }
