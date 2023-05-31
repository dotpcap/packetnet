/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// The DDM Parameter subsection field
    /// </summary>
    public class DrdaDdmParameter
    {
        /// <summary>
        /// The Other Data field
        /// </summary>
        public string Data { set; get; }

        /// <summary>
        /// The Drda Code Point Type field
        /// </summary>
        public DrdaCodePointType DrdaCodepoint { set; get; }

        /// <summary>
        /// The Length field
        /// </summary>
        public int Length { set; get; }
    }