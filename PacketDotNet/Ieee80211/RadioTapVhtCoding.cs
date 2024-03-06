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
public  enum RadioTapVhtCoding : byte
{
    CodingForUser1 = 0x01,
    CodingForUser2 = 0x02,
    CodingForUser3 = 0x03,
    CodingForUser4 = 0x04,

    Unused = 0xf0
}
