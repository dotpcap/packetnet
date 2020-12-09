/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
*  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
*/

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for link layer null packet payload types.
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum NullPacketType : ushort
    {
        /// <summary>
        /// Internet Protocol, Version 4 (IPv4)
        /// </summary>
        IPv4 = 2,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6 = 24,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6_28 = 28,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6_30 = 30,

        /// <summary>
        /// IPX
        /// </summary>
        IPX = 23
    }
}