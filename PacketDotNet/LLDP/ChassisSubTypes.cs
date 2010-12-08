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
namespace PacketDotNet.LLDP
{
    /// <summary>
    /// The Chassis ID TLV subtypes
    /// </summary>
    public enum ChassisSubTypes
    {
        /// <summary>A Chassis Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        ChassisComponent = 1,
        /// <summary>An Interface Alias identifier</summary>
        /// <remarks>See IETF RFC 2863</remarks>
        InterfaceAlias = 2,
        /// <summary>A Port Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        PortComponent = 3,
        /// <summary>A MAC (Media Access Control) Address identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        MACAddress = 4,
        /// <summary>A Network Address (IP Address) Identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        NetworkAddress = 5,
        /// <summary>An Interface Name identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        InterfaceName = 6,
        /// <summary>A Locally Assigned identifier</summary>
        LocallyAssigned = 7
    };
}