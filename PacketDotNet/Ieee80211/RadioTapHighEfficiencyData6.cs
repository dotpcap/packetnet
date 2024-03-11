/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapHighEfficiencyData6 : ushort
{
    /// <summary>
    /// NSTS (actual number of space-time streams, 0=unknown, 1=1, etc.
    /// </summary>
    Nsts = 0x000f,

    /// <summary>
    /// Doppler value
    /// </summary>
    DopplerValue = 0x0010,

    /// <summary>
    /// Reserved.
    /// </summary>
    Reserved = 0x00e0,

    /// <summary>
    /// TXOP Value
    /// </summary>
    TxopValue = 0x7f00,

    /// <summary>
    /// Midamble periodicity 
    /// 0=10
    /// 1=20
    /// </summary>
    MidamblePeriodicity = 0x8000,
}