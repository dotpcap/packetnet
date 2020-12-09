/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2013 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet
{
    /// <summary>802.1Q fields </summary>
    public struct Ieee8021QFields
    {
        /// <summary>Length in bytes of a Ieee8021Q header.</summary>
        public static readonly int HeaderLength; // 4

        /// <summary>Length of the tag control information in bytes.</summary>
        public static readonly int TagControlInformationLength = 2;

        /// <summary>Position of the tag control information </summary>
        public static readonly int TagControlInformationPosition = 0;

        /// <summary>Length of the ethertype value in bytes.</summary>
        public static readonly int TypeLength = 2;

        /// <summary>Position of the type field </summary>
        public static readonly int TypePosition;

        static Ieee8021QFields()
        {
            TypePosition = TagControlInformationPosition + TagControlInformationLength;
            HeaderLength = TypePosition + TypeLength;
        }
    }
}