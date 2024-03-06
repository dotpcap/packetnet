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
public enum RadioTapVhtKnown : ushort
{
    StbcKnown                       = 0x0001,
    TxopPsNotAllowedKnown           = 0x0002,
    GuardIntervalKnown              = 0x0004,
    ShortGiNsymDisambiguationKnown  = 0x0008,
    LdpcExtraOfdmSymbolKnown        = 0x0010,
    BeamformedKnown                 = 0x0020,
    BandwidthKnown                  = 0x0040,
    GroupIdKnown                    = 0x0080,
    PartialAidKnown                 = 0x0100
}
