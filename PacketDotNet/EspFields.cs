/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet
{
    public struct EspFields
    {
        /// <summary>Length of the Base Header in bytes.</summary>
        public static readonly int HeaderLength = 8;

        /// <summary>Length of the Security Parameters Index (SPI) in bytes.</summary>
        public static readonly int SecurityParametersIndexLength = 4;

        /// <summary>Length of the Sequence Number in bytes.</summary>
        public static readonly int SequenceNumberLength = 4;

        /// <summary>Length of the Pad Length in bytes.</summary>
        public static readonly int PadLengthLength = 1;

        /// <summary>Length of the Next Header in bytes.</summary>
        public static readonly int NextHeaderLength = 1;

    }
}
