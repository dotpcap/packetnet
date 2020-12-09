/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

namespace PacketDotNet
{
    /// <summary>Code constants for OSPF protocol versions.</summary>
    public enum OspfVersion : byte
    {
        /// <summary>OSPF protocol version 2.</summary>
        OspfV2 = 2,

        /// <summary>OSPF protocol version 3.</summary>
        OspfV3 = 3
    }
}