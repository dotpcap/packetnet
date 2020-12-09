/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2010 Cameron Elliott <cameron/at/cameronelliott/dot/com>
 *  
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Indicates the protocol encapsulated by the PPP packet
    /// See http://www.iana.org/assignments/ppp-numbers
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RawIPPacketProtocol : ushort
    {
        /// <summary>IPv4 </summary>
        IPv4 = 4,

        /// <summary>IPv6 </summary>
        IPv6 = 6
    }
}