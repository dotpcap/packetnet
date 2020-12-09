/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// The System Capabilities options
    /// </summary>
    [Flags]
    public enum CapabilityOptions
    {
        /// <summary>
        /// An Other Type of System
        /// </summary>
        Other = 0x01,

        /// <summary>A Repeater</summary>
        /// <remarks>See IETF RFC 2108</remarks>
        Repeater = 0x02,

        /// <summary>A Bridge</summary>
        /// <remarks>IETF RFC 2674</remarks>
        Bridge = 0x04,

        /// <summary>A WLAN Access Point</summary>
        /// <remarks>IEEE 802.11 MIB</remarks>
        WLanAP = 0x08,

        /// <summary>A Router</summary>
        /// <remarks>IETF RFC 1812</remarks>
        Router = 0x10,

        /// <summary>A Telephone</summary>
        /// <remarks>IETF RFC 2011 </remarks>
        Telephone = 0x20,

        /// <summary>A DOCSIS Cable Device</summary>
        /// <remarks>
        /// See IETF RFC 2669
        /// See IETF RFC 2670
        /// </remarks>
        DocsisCableDevice = 0x40,

        /// <summary>A Station with no other capabilities</summary>
        /// <remarks>IETF RFC 2011</remarks>
        StationOnly = 0x80
    }
}