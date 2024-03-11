/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapHighEfficiencyData3 : ushort
{
    /// <summary>
    /// BSS color bitmask
    /// </summary>
    BssColor = 0x003f,

    /// <summary>
    /// Beam change bitmask
    /// </summary>
    BeamChange = 0x0040,

    /// <summary>
    /// UL/DL bitmask
    /// </summary>
    UlDl = 0x0080,

    /// <summary>
    /// Data MCS (not SIG-B MCS from HE-SIG-A for HE_MU format bitmask
    /// </summary>
    DataMcs = 0x0f00,

    /// <summary>
    /// Data DCM (cf. data MCS) bitmask
    /// </summary>
    DataDcm = 0x1000,

    /// <summary>
    /// Coding bitmask
    /// 0=BCC
    /// 1=LDPC
    /// </summary>
    Coding = 0x2000,

    /// <summary>
    /// LDPC extra symbol segment 
    /// </summary>
    LdpcExtraSymbolSegment = 0x4000,

    /// <summary>
    /// STBC 
    /// </summary>
    Stbc = 0x800
}
