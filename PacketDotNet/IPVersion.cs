/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for internet protocol versions.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum IPVersion
    {
        /// <summary>Internet protocol version 4.</summary>
        IPv4 = 4,

        /// <summary>Internet protocol version 6.</summary>
        IPv6 = 6
    }
}