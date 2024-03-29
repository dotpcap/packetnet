﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// The fields in a PPP packet
    /// See http://en.wikipedia.org/wiki/Point-to-Point_Protocol
    /// </summary>
    public struct PppFields
    {
        /// <summary>Length of the Protocol field in bytes, the field is of type PppProtocol.</summary>
        public static readonly int ProtocolLength = 2;

        /// <summary>The length of the header.</summary>
        public static readonly int HeaderLength = ProtocolLength;

        /// <summary>Offset from the start of the PPP packet where the Protocol field is located.</summary>
        public static readonly int ProtocolPosition = 0;
    }