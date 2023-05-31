/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Ieee80211;

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