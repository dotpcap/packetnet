/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    public enum IgmpV3MembershipReportGroupRecordType : byte
    {
#pragma warning disable 1591
        ModeIsInclude = 0x01,
        ModeIsExclude = 0x02,
        ChangeToIncludeMode = 0x03,
        ChangeToExcludeMode = 0x04,
        AllowNewSources = 0x05,
        BlockOldSources = 0x06,
#pragma warning restore 1591
    }
}
