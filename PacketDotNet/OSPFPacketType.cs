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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

namespace PacketDotNet
{
    /// <summary>
    /// The five different OSPF packet types
    /// </summary>
    public enum OSPFPacketType : byte
    {
#pragma warning disable 1591
        Hello = 0x0001,
        DatabaseDescription = 0x0002,
        LinkStateRequest = 0x0003,
        LinkStateUpdate = 0x0004,
        LinkStateAcknowledgment = 0x0005,
#pragma warning restore 1591
    }
}