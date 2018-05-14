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

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// NOTE: All positions are not defined here because the frame type changes
    /// whether some address fields are present or not, causing the sequence control
    /// field to move. In addition the payload size determines where the frame control
    /// sequence value is as it is after the payload bytes, if any payload is present
    /// </summary>
    class MacFields
    {
        public static readonly Int32 Address1Position;
        public static readonly Int32 AddressLength = EthernetFields.MacAddressLength;
        public static readonly Int32 DurationIDLength = 2;
        public static readonly Int32 DurationIDPosition;
        public static readonly Int32 FrameCheckSequenceLength = 4;
        public static readonly Int32 FrameControlLength = 2;

        public static readonly Int32 FrameControlPosition = 0;
        public static readonly Int32 SequenceControlLength = 2;

        /// <summary>
        /// Not all MAC Frames contain a sequence control field. The value of this field is only meaningful when they do.
        /// </summary>
        public static readonly Int32 SequenceControlPosition;

        static MacFields()
        {
            DurationIDPosition = FrameControlPosition + FrameControlLength;
            Address1Position = DurationIDPosition + DurationIDLength;
            SequenceControlPosition = Address1Position + (AddressLength * 3);
        }
    }
}