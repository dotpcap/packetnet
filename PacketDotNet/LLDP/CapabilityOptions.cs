/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;

namespace PacketDotNet.LLDP
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
        ///</remarks>
        DocsisCableDevice = 0x40,
        /// <summary>A Station with no other capabilities</summary>
        /// <remarks>IETF RFC 2011</remarks>
        StationOnly = 0x80,
    };
}