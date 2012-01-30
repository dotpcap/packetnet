#region Header

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
 * Copyright 2011 David Thedens <dthedens@metageek.net>
 */

#endregion Header
using System;

namespace PacketDotNet
{
    namespace Ieee80211
    {

    #region Enumerations

        ///<summary>
        /// from PPI v 1.0.10
        /// </summary>
        [Flags]
        public enum PpiFieldType : int
        {
            /// <summary>
            /// IEEE80211_PPI_RESERVED_0
            /// </summary>
            IEEE80211_PPI_RESERVED_0 = 0,

            /// <summary>
            /// IEEE80211_PPI_RESERVED_1
            /// </summary>
            IEEE80211_PPI_RESERVED_1 = 1,

            /// <summary>
            /// IEEE80211_PPI_COMMON
            /// </summary>
            IEEE80211_PPI_COMMON = 2,

            ///<summary>
            /// IEEE80211_PPI_MAC_EXTENSIONS
            ///</summary>
            IEEE80211_PPI_MAC_EXTENSIONS = 3,

            /// <summary>
            /// IIEEE80211_PPI_MAC_PHY
            /// </summary>
            IEEE80211_PPI_MAC_PHY = 4,

            /// <summary>
            /// IEEE80211_PPI_SPECTRUM
            /// </summary>
            IEEE80211_PPI_SPECTRUM = 5,

            /// <summary>
            /// IEEE80211_PPI_PROCESS_INFO
            /// </summary>
            IEEE80211_PPI_PROCESS_INFO = 6,

            /// <summary>
            /// IEEE80211_CAPTURE_INFO
            /// </summary>
            IEEE80211_PPI_CAPTURE_INFO = 7,

            /// <summary>
            /// IEEE80211_AGGREGATION
            /// </summary>
            IEEE80211_PPI_AGGREGATION = 8,

            /// <summary>
            /// IEEE80211_802_3
            /// </summary>
            IEEE80211_PPI_802_3 = 9,

            /// <summary>
            /// IEEE80211_RESERVED_ALL
            /// </summary>
            IEEE80211_PPI_RESERVED_ALL = 10,
        }

    #endregion Enumerations
    }
}