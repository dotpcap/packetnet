/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

/// <summary>
/// Indicates the precense of data fields
/// </summary>
[Flags]
public enum RadioTapVhtKnown : ushort
{
    /// <summary>
    /// STBC Known
    /// </summary>
    StbcKnown                       = 0x0001,

    /// <summary>
    /// TXOP PS Not Allowed Known
    /// </summary>
    TxopPsNotAllowedKnown           = 0x0002,

    /// <summary>
    /// Guard Interval (GI) Known
    /// </summary>
    GuardIntervalKnown              = 0x0004,

    /// <summary>
    /// Short GI NSYM Disambiguation Known
    /// </summary>
    ShortGiNsymDisambiguationKnown  = 0x0008,

    /// <summary>
    /// LDPC Extra OFDM Sybmol Known
    /// </summary>
    LdpcExtraOfdmSymbolKnown        = 0x0010,

    /// <summary>
    /// Beamformed Known
    /// </summary>
    BeamformedKnown                 = 0x0020,

    /// <summary>
    /// Bandwidth Known
    /// </summary>
    BandwidthKnown                  = 0x0040,

    /// <summary>
    /// Group ID Known
    /// </summary>
    GroupIdKnown                    = 0x0080,

    /// <summary>
    /// Partial AID Known
    /// </summary>
    PartialAidKnown                 = 0x0100
}