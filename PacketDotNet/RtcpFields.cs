/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    public struct RtcpFields
    {
        /// <summary>Length of the Base Header in bytes.</summary>
        public static readonly int HeaderLength = 8;

        // flag bit masks
        public static readonly int VersionMask = 0xC0;
        public static readonly int PaddingMask = 0x20;
        public static readonly int ReceptionReportCountMask = 0x1F;
    }
}
