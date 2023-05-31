/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet.Lldp;

    /// <summary>
    /// Interface Numbering Types
    /// </summary>
    /// <remarks>Source IETF RFC 802.1AB</remarks>
    public enum InterfaceNumber
    {
        /// <summary>Unknown</summary>
        Unknown,

        /// <summary>Interface Index</summary>
        IfIndex,

        /// <summary>System Port Number</summary>
        SystemPortNumber
    }