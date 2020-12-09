/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Indicates the protocol encapsulated by the PPP packet
    /// See http://www.iana.org/assignments/ppp-numbers
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PppProtocol : ushort
    {
        /// <summary>Padding.</summary>
        Padding = 0x1,

        /// <summary>IPv4.</summary>
        IPv4 = 0x21,

        /// <summary>IPv6.</summary>
        IPv6 = 0x57
    }
}