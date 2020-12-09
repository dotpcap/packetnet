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

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// The Port ID TLV subtypes
    /// </summary>
    public enum PortSubType
    {
        /// <summary>An Interface Alias identifier</summary>
        /// <remarks>See IETF RFC 2863</remarks>
        InterfaceAlias = 1,

        /// <summary>A Port Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        PortComponent = 2,

        /// <summary>A MAC (Media Access Control) Address identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        MacAddress = 3,

        /// <summary>A Network Address (IP Address) Identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        NetworkAddress = 4,

        /// <summary>An Interface Name identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        InterfaceName = 5,

        /// <summary>An Agent Circiut ID identifier</summary>
        /// <remarks>See IETF RFC 3046</remarks>
        AgentCircuitId = 6,

        /// <summary>A Locally Assigned identifier</summary>
        /// <remarks>See IETF RFC 3046</remarks>
        LocallyAssigned = 7
    }
}