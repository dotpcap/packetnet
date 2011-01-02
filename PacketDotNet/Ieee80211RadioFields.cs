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
using System;

namespace PacketDotNet
{
    /// <summary>
    /// As defined by Airpcap.h
    ///
    /// NOTE: PresentPosition may not be the only position present
    /// as this the field can be extended if the high bit is set
    /// </summary>
    public class Ieee80211RadioFields
    {
        /// <summary>Length of the version field</summary>
        public readonly static int VersionLength = 1;

        /// <summary>Length of the pad field</summary>
        public readonly static int PadLength = 1;

        /// <summary>Length of the length field</summary>
        public readonly static int LengthLength = 2;

        /// <summary>Length of the first present field (others may follow)</summary>
        public readonly static int PresentLength = 4;

        /// <summary>Position of the version field</summary>
        public readonly static int VersionPosition = 0;

        /// <summary>Position of the padding field</summary>
        public readonly static int PadPosition;

        /// <summary>Position of the length field</summary>
        public readonly static int LengthPosition;

        /// <summary>Position of the first present field</summary>
        public readonly static int PresentPosition;

        /// <summary>Default header length, assuming one present field entry</summary>
        public readonly static int DefaultHeaderLength;

        static Ieee80211RadioFields()
        {
            PadPosition = VersionPosition + VersionLength;
            LengthPosition = PadPosition + PadLength;
            PresentPosition = LengthPosition + LengthLength;

            // default to the normal header size until the header length can be read
            DefaultHeaderLength = PresentPosition + PresentLength;
        }
    }
};

