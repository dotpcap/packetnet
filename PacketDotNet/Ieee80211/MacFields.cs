/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Ieee80211;

    public struct MacFields
    {
        public static readonly int Address1Position;
        public static readonly int AddressLength = EthernetFields.MacAddressLength;
        public static readonly int DurationIDLength = 2;
        public static readonly int DurationIDPosition;
        public static readonly int FrameCheckSequenceLength = 4;
        public static readonly int FrameControlLength = 2;
        public static readonly int FrameControlPosition = 0;
        public static readonly int SequenceControlLength = 2;

        /// <summary>
        /// Not all MAC Frames contain a sequence control field. The value of this field is only meaningful when they do.
        /// </summary>
        public static readonly int SequenceControlPosition;

        /// <summary>
        /// NOTE: All positions are not defined here because the frame type changes
        /// whether some address fields are present or not, causing the sequence control
        /// field to move. In addition the payload size determines where the frame control
        /// sequence value is as it is after the payload bytes, if any payload is present
        /// </summary>
        static MacFields()
        {
            DurationIDPosition = FrameControlPosition + FrameControlLength;
            Address1Position = DurationIDPosition + DurationIDLength;
            SequenceControlPosition = Address1Position + (AddressLength * 3);
        }
    }