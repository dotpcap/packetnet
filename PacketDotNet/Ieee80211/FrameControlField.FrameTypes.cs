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


namespace PacketDotNet.Ieee80211
{
    public partial class FrameControlField
    {
        /// <summary>
        /// Specifies the main frame type: Control, Management or Data.
        /// </summary>
        public enum FrameTypes
        {
            /// <summary>
            /// Management frame.
            /// </summary>
            Management = 0,

            /// <summary>
            /// Control frame.
            /// </summary>
            Control = 1,

            /// <summary>
            /// Data frame.
            /// </summary>
            Data = 2
        }
    }
}