/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet;

    /// <summary>
    /// Ieee p8021 P priorities.
    /// http://en.wikipedia.org/wiki/IEEE_802.1p
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum IeeeP8021PPriority : byte
    {
        /// <summary>
        /// Background
        /// </summary>
        Background = 1,

        /// <summary>
        /// Best effort
        /// </summary>
        BestEffort = 0,

        /// <summary>
        /// Excellent effort
        /// </summary>
        ExcellentEffort = 2,

        /// <summary>
        /// Critical application
        /// </summary>
        CriticalApplications = 3,

        /// <summary>
        /// Video, &lt; 100ms latency and jitter
        /// </summary>
        Video100ms = 4,

        /// <summary>
        /// Voice, &lt; 10ms latency and jitter
        /// </summary>
        Voice10ms = 5,

        /// <summary>
        /// InterNetwork control
        /// </summary>
        InterNetworkControl = 6,

        /// <summary>
        /// Network control
        /// </summary>
        NetworkControl = 7
    }