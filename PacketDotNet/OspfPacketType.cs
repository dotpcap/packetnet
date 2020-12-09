/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// The five different OSPF packet types
    /// </summary>
    public enum OspfPacketType : byte
    {
        Hello = 0x0001,
        DatabaseDescription = 0x0002,
        LinkStateRequest = 0x0003,
        LinkStateUpdate = 0x0004,
        LinkStateAcknowledgment = 0x0005
    }
}