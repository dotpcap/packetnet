/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// The fields in a Null packet
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
    public struct NullFields
    {
        /// <summary>
        /// Length of the Protocol field in bytes, the field is of type
        /// </summary>
        public static readonly int ProtocolLength = 4;

        /// <summary>
        /// The length of the header
        /// </summary>
        public static readonly int HeaderLength = ProtocolLength;

        /// <summary>
        /// Offset from the start of the packet where the Protocol field is located
        /// </summary>
        public static readonly int ProtocolPosition = 0;
    }
}