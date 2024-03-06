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
public enum RadioTapVhtMcsNss : byte
{
    /// <summary>
    /// Number of Spatial Streams
    /// </summary>
    Nss = 0x0f,

    /// <summary>
    /// MCS rate index
    /// </summary>
    Mcs = 0xf0
}
