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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Channel flags
        /// </summary>
        [Flags]
        public enum RadioTapChannelFlags : ushort
        {
            /// <summary>Turbo channel</summary>
            Turbo = 0x0010,
            ///<summary>CCK channel</summary>
            Cck = 0x0020,
            /// <summary>OFDM channel</summary>
            Ofdm = 0x0040,
            /// <summary>2 GHz spectrum channel</summary>
            Channel2Ghz = 0x0080,
            /// <summary>5 GHz spectrum channel</summary>
            Channel5Ghz = 0x0100,
            /// <summary>Only passive scan allowed</summary>
            Passive = 0x0200,
            /// <summary>Dynamic CCK-OFDM channel</summary>
            DynamicCckOfdm = 0x0400,
            /// <summary>GFSK channel (FHSS PHY)</summary>
            Gfsk = 0x0800,
            /// <summary>11a static turbo channel only</summary>
            StaticTurbo = 0x2000
        }; 
    }
}
