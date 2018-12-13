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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

#endregion Header


using System;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// As defined by Airpcap.h
    /// NOTE: PresentPosition may not be the only position present
    /// as this the field can be extended if the high bit is set
    /// </summary>
    public class PpiHeaderFields
    {
        #region Constructors

        static PpiHeaderFields()
        {
            FlagsPosition = VersionPosition + VersionLength;
            LengthPosition = FlagsPosition + FlagsLength;
            DataLinkTypePosition = LengthPosition + LengthLength;
            FirstFieldPosition = DataLinkTypePosition + DataLinkTypeLength;
            PpiPacketHeaderLength = FirstFieldPosition;
        }

        #endregion Constructors


        #region Fields

        /// <summary>Position of the first iField Header</summary>
        public static readonly Int32 FirstFieldPosition;

        /// <summary>Length of the Data Link Type</summary>
        public static readonly Int32 DataLinkTypeLength = 4;

        /// <summary>The data link type position.</summary>
        public static readonly Int32 DataLinkTypePosition;

        /// <summary>Length of the Flags field</summary>
        public static readonly Int32 FlagsLength = 1;

        /// <summary>Position of the Flags field</summary>
        public static readonly Int32 FlagsPosition;

        /// <summary>Length of the length field</summary>
        public static readonly Int32 LengthLength = 2;

        /// <summary>Position of the length field</summary>
        public static readonly Int32 LengthPosition;

        /// <summary>Length of the version field</summary>
        public static readonly Int32 VersionLength = 1;

        /// <summary>Position of the version field</summary>
        public static readonly Int32 VersionPosition = 0;

        /// <summary>The total length of the ppi packet header</summary>
        public static readonly Int32 PpiPacketHeaderLength;

        /// <summary>The length of the PPI field header</summary>
        public static readonly Int32 FieldHeaderLength = 4;

        #endregion Fields
    }
}