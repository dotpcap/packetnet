/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */
using System;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Specifies the different types of algorithms that the
    ///  Alternative Checksum option are allowed to use
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://datatracker.ietf.org/doc/rfc1146/
    /// </remarks>
    public enum ChecksumAlgorighmType
    {
        /// <summary>Standard TCP Checksum Algorithm</summary>
        TCPChecksum = 0,

        /// <summary>8-bit Fletchers Algorighm</summary>
        EightBitFletchersAlgorithm = 1,

        /// <summary>16-bit Fletchers Algorithm</summary>
        SixteenBitFletchersAlgorithm = 2,

        /// <summary>Redundant Checksum Avoidance</summary>
        RedundantChecksumAvoidance = 3
    }
}

