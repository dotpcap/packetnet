/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for IGMP message types.
    /// From RFC #2236.
    /// </summary>
    public enum IgmpV2MessageType : byte
    {
#pragma warning disable 1591
        MembershipQuery = 0x11,
        MembershipReportIGMPv1 = 0x12,
        MembershipReportIGMPv2 = 0x16,
        MembershipReportIGMPv3 = 0x22,
        LeaveGroup = 0x17,
#pragma warning restore 1591
    }
}