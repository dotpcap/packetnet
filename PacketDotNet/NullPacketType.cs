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
*  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
*/

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for link layer null packet payload types.
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum NullPacketType : ushort
    {
        /// <summary>
        /// Internet Protocol, Version 4 (IPv4)
        /// </summary>
        IPv4 = 2,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6 = 24,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6_28 = 28,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6_30 = 30,

        /// <summary>
        /// IPX
        /// </summary>
        IPX = 23
    }
}