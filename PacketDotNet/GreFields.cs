/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>GRE protocol field encoding information.</summary>
    public struct GreFields
    {
        /// <summary>Length of the Checksum in bytes (Optional).</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the Checksum in bytes (Optional).</summary>
        public static readonly int ChecksumPosition = 2;

        /// <summary>Length of the Flags in bytes.</summary>
        public static readonly int FlagsLength = 2;

        /// <summary>Length of the Key in bytes (Optional).</summary>
        public static readonly int KeyLength = 4;

        /// <summary>Length of the Protocol in bytes.</summary>
        public static readonly int ProtocolLength = 2;

        /// <summary>Position of the Protocol.</summary>
        public static readonly int ProtocolPosition = 2;

        /// <summary>Length of the Reserved in bytes (Optional).</summary>
        public static readonly int ReservedLength = 2;

        /// <summary>Length of the Sequence Number in bytes (Optional).</summary>
        public static readonly int SequenceLength = 4;
    }
}