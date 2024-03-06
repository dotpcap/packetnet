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
    /// Channel flags
    /// </summary>
    [Flags]
    public enum RadioTapMcsFlags : byte
    {
        /// <summary>Channel Width: 20MHz=0, 40MHz=1, 20L=2, 20U=3</summary>
        Bandwidth = 0x03,

        /// <summary>Guard Interval (GI): Long GI=0, Short GI=1</summary>
        GuardInterval = 0x04,

        /// <summary>HT Format: 0=Mixed, 1=Greenfield</summary>
        HtFormat = 0x08,

        /// <summary>FEC Type: BCC=0, LDPC=1</summary>
        FecType = 0x10,

        /// <summary>Number of STBC Streams</summary>
        NumberOfStbcStreams = 0x60,

        /// <summary>Number of extension spacial streams</summary>
        Ness = 0x80
    }