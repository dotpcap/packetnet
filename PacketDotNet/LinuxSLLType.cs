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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    /// <summary>
    /// The types of cooked packets
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public enum LinuxSLLType
    {
        /// <summary>
        /// Packet was sent to us by somebody else
        /// </summary>
        PacketSentToUs = 0x0,

        /// <summary>
        /// Packet was broadcast by somebody else
        /// </summary>
        PacketBroadCast = 0x1,

        /// <summary>
        /// Packet was multicast, but not broadcast
        /// </summary>
        PacketMulticast = 0x2,

        /// <summary>
        /// Packet was sent by somebody else to somebody else
        /// </summary>
        PacketSentToSomeoneElse = 0x3,

        /// <summary>
        /// Packet was sent by us
        /// </summary>
        PacketSentByUs = 0x4
    }
}
