/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Ieee80211;

/// <summary>
/// Indicates the bandwidth and side channel for a frame capture with VHT
/// </summary>
public enum RadioTapVhtBandwidth : byte
{
    /// <summary>
    /// 20MHz bandwidth
    /// </summary>
    MHz20 = 0x00,

    /// <summary>
    /// 40MHz bandwidth
    /// </summary>
    MHz40 = 0x01,

    /// <summary>
    /// 40MHz bandwidth with 20L side band. Sideband index=0
    /// </summary>
    MHz40Side20L = 2,

    /// <summary>
    /// 40MHz bandwidth with 20U side band. Sideband index=1
    /// </summary>
    MHz40Side20U = 3,

    /// <summary>
    /// 80MHz bandwidth
    /// </summary>
    MHz80 = 4,

    /// <summary>
    /// 80MHz with 40L side channel
    /// </summary>
    MHz80Side40L = 5,

    /// <summary>
    /// 80MHz with 44U side channel
    /// </summary>
    MHz80Side40U = 6,

    /// <summary>
    /// 80MHz with 20LL side channel
    /// </summary>
    MHz80Side20LL = 7,

    /// <summary>
    /// 80MHz with 20LU side channel
    /// </summary>
    MHz80Side20LU = 8,

    /// <summary>
    /// 80MHz with 20UL side channel
    /// </summary>
    MHz80Side20UL = 9,

    /// <summary>
    /// 80MHz with 20UU side channel
    /// </summary>
    MHz80Side20UU = 10,

    /// <summary>
    /// 160MHz bandwidth
    /// </summary>
    MHz160 = 11,

    /// <summary>
    /// 160MHz with 80L side channel
    /// </summary>
    MHz160Side80L = 12,

    /// <summary>
    /// 160MHz with 80U side channel
    /// </summary>
    MHz160Side80U = 13,

    /// <summary>
    /// 160MHz with 40LL side channel
    /// </summary>
    MHz160Side40LL = 14,

    /// <summary>
    /// 160MHz with 40LU side channel
    /// </summary>
    MHz160Side40LU = 15,

    /// <summary>
    /// 160MHz with 40UL side channel
    /// </summary>
    MHz160Side40UL = 16,

    /// <summary>
    /// 160MHz with 40UU side channel
    /// </summary>
    MHz160Side40UU = 17,

    /// <summary>
    /// 160MHz with 20LLL side channel
    /// </summary>
    MHz160Side20LLL = 18,

    /// <summary>
    /// 160MHz with 20LLU side channel
    /// </summary>
    MHz160Side20LLU = 19,

    /// <summary>
    /// 160MHz with 20LUL side channel
    /// </summary>
    MHz160Side20LUL = 20,

    /// <summary>
    /// 160MHz with 20LUU side channel
    /// </summary>
    MHz160Side20LUU = 21,

    /// <summary>
    /// 160MHz with 20ULL side channel
    /// </summary>
    MHz160Side20ULL = 22,

    /// <summary>
    /// 160MHz with 20ULU side channel
    /// </summary>
    MHz160Side20ULU = 23,

    /// <summary>
    /// 160MHz with 20UUL side channel
    /// </summary>
    MHz160Side20UUL = 24,

    /// <summary>
    /// 160MHz with 20UUU side channel
    /// </summary>
    MHz160Side20UUU = 25,
}