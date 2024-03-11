/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapHighEfficiencyData5 : ushort
{
    /// <summary>
    /// Data Bandwidth/RU allocation bitmask
    /// 0=20
    /// 1=40
    /// 2=80
    /// 3=160/80+80
    /// 4=26-tone RU
    /// 5=52-tone RU
    /// 6=106-tone RU
    /// 7=242-tone RU
    /// 8=484-tone RU
    /// 9=996-tone RU
    /// 10=2x996-tone RU
    /// </summary>
    DataBandwidth = 0x000f,

    /// <summary>
    /// Guard Interval (GI)
    /// 0=0.8us
    /// 1=1.6us
    /// 2=3.2us
    /// 3=reserved
    /// </summary>
    Gi = 0x30,

    /// <summary>
    /// LTF symbol size bitmask
    /// 0=unknown
    /// 1=1x
    /// 2=2x
    /// 3=4x
    /// </summary>
    LtfSymbolSize = 0x00c0,

    /// <summary>
    /// Number of LTF symbols 
    /// 0=1x
    /// 1=2x
    /// 2=4x
    /// 3=6x
    /// 4=8x
    /// 5-7=reserved
    /// </summary>
    NumberOfLtfSymbols = 0x0700,

    /// <summary>
    /// Pre-FEC Padding Factor bitmask
    /// </summary>
    PreFecPaddingFactor = 0x3000,

    /// <summary>
    /// TxBF bitmask
    /// </summary>
    Txbf = 0x4000,

    /// <summary>
    /// PE Disambiguity
    /// </summary>
    PeDisambiguity = 0x8000
}
