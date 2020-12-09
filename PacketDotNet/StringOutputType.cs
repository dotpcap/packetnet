/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// The available types of strings that the ToString(StringOutputType) can handle.
    /// </summary>
    public enum StringOutputType
    {
        /// <summary>
        /// Outputs the packet info on a single line
        /// </summary>
        Normal,

        /// <summary>
        /// Outputs the packet info on a single line with coloring
        /// </summary>
        Colored,

        /// <summary>
        /// Outputs the detailed packet info
        /// </summary>
        Verbose,

        /// <summary>
        /// Outputs the detailed packet info with coloring
        /// </summary>
        VerboseColored
    }
}