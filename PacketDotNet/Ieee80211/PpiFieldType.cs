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
using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet.Ieee80211
{
    #region Enumerations

    /// <summary>
    /// from PPI v 1.0.10
    /// </summary>
    [Flags]
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

    #endregion Enumerations
}