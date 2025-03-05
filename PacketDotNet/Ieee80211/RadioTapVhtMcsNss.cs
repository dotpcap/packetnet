/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

/// <summary>
/// MCS rate index and spatial streams for one user
/// </summary>
[Flags]
public enum RadioTapVhtMcsNss : byte
{
    /// <summary>
    /// Number of Spatial Streams (1-8)
    /// </summary>
    Nss = 0x0f,

    /// <summary>
    /// MCS rate index
    /// </summary>
    Mcs = 0xf0
}
