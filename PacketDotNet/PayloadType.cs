/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet
{
    /// <summary>
    /// Differentiates between a packet class payload, a byte[] payload
    /// or no payload
    /// </summary>
    public enum PayloadType
    {
        /// <summary>
        /// The payload is a packet.
        /// </summary>
        Packet,

        /// <summary>
        /// The payload is a byte array.
        /// </summary>
        Bytes,

        /// <summary>
        /// There is no payload.
        /// </summary>
        None
    }
}