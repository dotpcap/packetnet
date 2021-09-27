/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>L2TP protocol field encoding information.</summary>
    // ReSharper disable once InconsistentNaming
    public struct L2tpFields
    {
        /// <summary>Length of the Flags in bytes.</summary>
        public static readonly int FlagsLength = 2;

        /// <summary>Length of the Base Header in bytes.</summary>
        public static readonly int HeaderLength = 2;

        /// <summary>Length of the Length in bytes.</summary>
        public static readonly int LengthsLength = 2;

        /// <summary>Length of the Nr in bytes.</summary>
        public static readonly int NrLength = 2;

        /// <summary>Length of the Ns in bytes.</summary>
        public static readonly int NsLength = 2;

        /// <summary>Length of the Offset Pad in bytes (Optional).</summary>
        public static readonly int OffsetPadLength = 2;

        /// <summary>Length of the Offset Size in bytes (Optional).</summary>
        public static readonly int OffsetSizeLength = 2;

        /// <summary>The port of L2TP.</summary>
        public static readonly ushort Port = 1701;
    }
}