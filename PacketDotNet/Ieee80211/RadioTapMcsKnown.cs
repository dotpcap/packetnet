/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Ieee80211;

    /// <summary>
    /// Known fields in the MCS radiko tap header
    /// </summary>
    [Flags]
    public enum RadioTapMcsKnown : byte
    {
        /// <summary>Bandwidth</summary>
        Bandwidth = 0x01,

        /// <summary>MCS index</summary>
        McsIndexKnown = 0x02,

        /// <summary>Guard interval</summary>
        GuardInterval = 0x04,

        /// <summary>HT Format</summary>
        HtFormat = 0x08,

        /// <summary>FEC type</summary>
        FecType = 0x10,

        /// <summary>STBC known</summary>
        SbtcKnown = 0x20,

        /// <summary>Ness known (number of extension spatial streams)</summary>
        NessKnown = 0x40,

        /// <summary>Bandwidth</summary>
        NessData = 0x80,
    }