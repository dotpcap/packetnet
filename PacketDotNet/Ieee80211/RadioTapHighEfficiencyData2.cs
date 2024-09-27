/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

/// <summary>
/// Flags that indicate the presence of fields in the HE radio tap field as well as the 
/// RU Allocation Offset and 
/// </summary>
[Flags]
public enum RadioTapHighEfficiencyData2 : ushort
{
    /// <summary>
    /// Pri/sec 80 MHz known
    /// </summary>
    PriSec80MHzKnown = 0x0001,

    /// <summary>
    /// GI Known
    /// </summary>
    GiKnown = 0x0002,

    /// <summary>
    /// Number of LTF symbols known
    /// </summary>
    NumberOfLtfSymbolsKnown = 0x0004,

    /// <summary>
    /// Pre-FEC Padding Factor known
    /// </summary>
    PreFecPaddingFactorKnown = 0x0008,

    /// <summary>
    /// TxBF known
    /// </summary>
    TxbfKnown = 0x0010,

    /// <summary>
    /// PE Disambiguity known
    /// </summary>
    PeDisambiguityKnown = 0x0020,

    /// <summary>
    /// TXOP known
    /// </summary>
    TxopKnown = 0x0040,

    /// <summary>
    /// Midamble periodicity known
    /// </summary>
    MidamblePeriodicityKnown = 0x0080,

    /// <summary>
    /// Bitmask for RU allocation offset
    /// </summary>
    RuAllocationOffset = 0x3f00,

    /// <summary>
    /// RU allocation offset known
    /// </summary>
    RuAllocationOffsetKnown = 0x4000,

    /// <summary>
    /// Pri/sec 80MHz.
    /// 0 = primary
    /// 1 = secondary
    /// </summary>
    PriSec80MHz = 0x8000,
}
