/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapHighEfficiencyData1 : ushort
{
    HePpduFormat = 0x0003,
    BssColorKnown = 0x0004,
    BeamChangeKnown = 0x0008,
    UlDlDown = 0x0010,
    DataMcsKnown = 0x0020,
    DataDcmKnown = 0x0040,
    CodingKnown = 0x0080,
    LdpcExtraSymbolSegmentKnown = 0x0100,
    StbcKnown = 0x0200,
    SpatialReuseKnownKnown = 0x0400,
    SpatialReuse2KnownKnown = 0x0800,
    SpatialReuse3KnownKnown = 0x1000,
    SpatialReuse4KnownKnown = 0x2000,
    DataBwRuAllocationKnown = 0x4000,
    DopplerKnown = 0x8000,
}
