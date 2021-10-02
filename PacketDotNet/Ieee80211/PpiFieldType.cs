/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2011 David Thedens <dthedens@metageek.net>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// from PPI v 1.0.10
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PpiFieldType
    {
        /// <summary>
        /// PpiReserved0
        /// </summary>
        PpiReserved0 = 0,

        /// <summary>
        /// PpiReserved1
        /// </summary>
        PpiReserved1 = 1,

        /// <summary>
        /// PpiCommon
        /// </summary>
        PpiCommon = 2,

        /// <summary>
        /// PpiMacExtensions
        /// </summary>
        PpiMacExtensions = 3,

        /// <summary>
        /// PpiMacPhy
        /// </summary>
        PpiMacPhy = 4,

        /// <summary>
        /// PpiSpectrum
        /// </summary>
        PpiSpectrum = 5,

        /// <summary>
        /// PpiProcessInfo
        /// </summary>
        PpiProcessInfo = 6,

        /// <summary>
        /// PpiCaptureInfo
        /// </summary>
        PpiCaptureInfo = 7,

        /// <summary>
        /// PpiAggregation
        /// </summary>
        PpiAggregation = 8,

        /// <summary>
        /// Ppi802_3
        /// </summary>
        Ppi802_3 = 9,

        /// <summary>
        /// PpiReservedAll
        /// </summary>
        PpiReservedAll = 10
    }
}