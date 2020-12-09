/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Specifies the different types of algorithms that the
    /// Alternative Checksum option are allowed to use
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1146/
    /// </remarks>
    public enum ChecksumAlgorithmType
    {
        /// <summary>Standard TCP Checksum Algorithm</summary>
        TcpChecksum = 0,

        /// <summary>8-bit Fletchers Algorithm</summary>
        EightBitFletchersAlgorithm = 1,

        /// <summary>16-bit Fletchers Algorithm</summary>
        SixteenBitFletchersAlgorithm = 2,

        /// <summary>Redundant Checksum Avoidance</summary>
        RedundantChecksumAvoidance = 3
    }
}