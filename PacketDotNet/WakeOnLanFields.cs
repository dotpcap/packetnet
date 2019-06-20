
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
 *  Copyright 2019 George Rahul<georgerahul143@gmail.com>
 */

namespace PacketDotNet
{
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

        static WakeOnLanFields()
        {
            DestinationAddressPosition = SyncSequenceLength;
            DestinationAddressLength = MacAddressRepetition * EthernetFields.MacAddressLength;
            PasswordPosition = DestinationAddressPosition + DestinationAddressLength;
        }
    }
}