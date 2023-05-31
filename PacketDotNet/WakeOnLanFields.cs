/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// Wake-On-Lan protocol field encoding information.
    /// </summary>
    public struct WakeOnLanFields
    {
        /// <summary>Position of the Sync Sequence within the Wake-On-Lan header.</summary>
        public static readonly int SyncSequencePosition = 0;

        /// <summary>Size of an Sync Sequence in bytes.</summary>
        public static readonly int SyncSequenceLength = 6;

        /// <summary>Position of the Destination Address within the Wake-On-Lan header.</summary>
        public static readonly int DestinationAddressPosition;

        /// <summary>Size of an Destination Address in bytes.</summary>
        public static readonly int DestinationAddressLength;

        /// <summary>Number of times Destination Address is repeated in Wake-On-Lan.</summary>
        public static readonly int MacAddressRepetition = 16;

        /// <summary>Position of the Password within the Wake-On-Lan header.</summary>
        public static readonly int PasswordPosition;

        /// <summary>Port 0.</summary>
        public const int Port0 = 0;

        /// <summary>Port 7.</summary>
        public const int Port7 = 7;

        /// <summary>Port 9.</summary>
        public const int Port9 = 9;

        static WakeOnLanFields()
        {
            DestinationAddressPosition = SyncSequenceLength;
            DestinationAddressLength = MacAddressRepetition * EthernetFields.MacAddressLength;
            PasswordPosition = DestinationAddressPosition + DestinationAddressLength;
        }
    }