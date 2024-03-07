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

/// <summary>
/// Flags that indicate the presence of fields in the HE radio tap field as well as the HE PPDU Format
/// </summary>
[Flags]
public enum RadioTapHighEfficiencyData1 : ushort
{
    /// <summary>
    /// HE PPDU Format:
    /// 0=HE_SU
    /// 1=HE_EXT_SU
    /// 2=HE_MY
    /// 3=HE_TRIG
    /// </summary>
    HePpduFormat = 0x0003,

    /// <summary>
    /// BSS Color Known
    /// </summary>
    BssColorKnown = 0x0004,

    /// <summary>
    /// Beam Change Known
    /// </summary>
    BeamChangeKnown = 0x0008,

    /// <summary>
    /// UL/DL Known
    /// </summary>
    UlDlDown = 0x0010,

    /// <summary>
    /// Data MCS Known
    /// </summary>
    DataMcsKnown = 0x0020,

    /// <summary>
    /// Data DCM Known
    /// </summary>
    DataDcmKnown = 0x0040,

    /// <summary>
    /// Coding Known
    /// </summary>
    CodingKnown = 0x0080,

    /// <summary>
    /// LDPC extra symbol segment known
    /// </summary>
    LdpcExtraSymbolSegmentKnown = 0x0100,

    /// <summary>
    /// STBC Known
    /// </summary>
    StbcKnown = 0x0200,

    /// <summary>
    /// Spatial Reuse known (Spatial Reuse 1 for HE_TRIG format)
    /// </summary>
    SpatialReuseKnownKnown = 0x0400,

    /// <summary>
    /// Spatial Reuse 2 known (HE_TRIG format), STA-ID known (HE_MU format)
    /// </summary>
    SpatialReuse2KnownKnown = 0x0800,

    /// <summary>
    /// Spatial Reuse 3 known (HE_TRIG format)
    /// </summary>
    SpatialReuse3KnownKnown = 0x1000,

    /// <summary>
    /// Spatial Reuse 4 known (HE_TRIG format)
    /// </summary>
    SpatialReuse4KnownKnown = 0x2000,

    /// <summary>
    /// Data BW/RU allocation known
    /// </summary>
    DataBwRuAllocationKnown = 0x4000,

    /// <summary>
    /// Doppler known
    /// </summary>
    DopplerKnown = 0x8000,
}
