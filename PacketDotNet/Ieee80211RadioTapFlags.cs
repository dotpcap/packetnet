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
    /// <summary>
    /// Radio tap flags
    /// </summary>
    [Flags]
    public enum Ieee80211RadioTapFlags
    {
        /// <summary>
        /// sent/received during cfp
        /// </summary>
        CFP                = 0x01,
        /// <summary>
        /// sent/received with short preamble
        /// </summary>
        ShortPreamble      = 0x02,
        /// <summary>
        /// sent/received with WEP encryption
        /// </summary>
        WepEncrypted       = 0x04,
        /// <summary>
        /// sent/received with fragmentation
        /// </summary>
        Fragmentation      = 0x08,
        /// <summary>
        /// frame includes FCS
        /// </summary>
        FcsIncludedInFrame = 0x10
    };
}
