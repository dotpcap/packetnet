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
using System;
using PacketDotNet.Utils;

﻿namespace PacketDotNet
{
    /// <summary>
    /// Represents a Layer 2 protocol.
    /// </summary>
    public abstract class DataLinkPacket : Packet
    {
        /// <summary>
        /// DataLinkPacket constructor
        /// </summary>
        public DataLinkPacket()
        {}
    }
}
