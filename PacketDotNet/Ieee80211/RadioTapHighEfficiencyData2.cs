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
public enum RadioTapHighEfficiencyData2 : ushort
{
    PriSec80MHzKnown = 0x0001,
    GiKnown = 0x0002,
    NumberOfLtfSymbolsKnown = 0x0004,
    PreFecPaddingFactorKnown = 0x0008,
    TxbfKnown = 0x0010,
    PeDisambiguityKnown = 0x0020,
    TxopKnown = 0x0040,
    MidamblePeriodicityKnown = 0x0080,
    RuAllocationOffset = 0x3f00,
    RuAllocationOffsetKnown = 0x4000,

    /// <summary>
    /// 0 = primary, 1 = secondary
    /// </summary>
    PriSec80MHz = 0x8000,
}
