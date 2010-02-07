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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
namespace PacketDotNet
{
    /// <summary>
    /// The fields in a PPP packet
    /// See http://en.wikipedia.org/wiki/Point-to-Point_Protocol
    /// </summary>
    public class PPPFields
    {
        /// <summary>
        /// Length of the Protocol field in bytes, the field is of type
        /// PPPProtocol
        /// </summary>
        public static readonly int ProtocolLength = 2;

        /// <summary>
        /// Offset from the start of the PPP packet where the Protocol field is located
        /// </summary>
        public static readonly int ProtocolPosition = 0;

        /// <summary>
        /// The length of the header
        /// </summary>
        public static readonly int HeaderLength = ProtocolLength;
    }
}
