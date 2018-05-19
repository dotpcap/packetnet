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

using System;

namespace PacketDotNet
{
    /// <summary>
    /// The fields in a Null packet
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
    public class NullFields
    {
        /// <summary>
        /// Length of the Protocol field in bytes, the field is of type
        /// </summary>
        public static readonly Int32 ProtocolLength = 4;

        /// <summary>
        /// The length of the header
        /// </summary>
        public static readonly Int32 HeaderLength = ProtocolLength;


        /// <summary>
        /// Offset from the start of the packet where the Protocol field is located
        /// </summary>
        public static readonly Int32 ProtocolPosition = 0;
    }
}