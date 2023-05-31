/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Lsa;

    /// <summary>
    /// The different LSA types
    /// </summary>
    public enum LinkStateAdvertisementType : byte
    {
#pragma warning disable 1591
        Router = 0x01,
        Network = 0x02,
        Summary = 0x03,
        SummaryASBR = 0x04,
        ASExternal = 0x05
#pragma warning restore 1591
    }