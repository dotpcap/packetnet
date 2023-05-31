/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// Ethernet protocol field encoding information.
    /// </summary>
    public struct EthernetFields
    {
        /// <summary>Position of the destination MAC address within the ethernet header.</summary>
        public static readonly int DestinationMacPosition = 0;

        /// <summary>Total length of an ethernet header in bytes.</summary>
        public static readonly int HeaderLength; // == 14

        /// <summary>Size of an ethernet mac address in bytes.</summary>
        public static readonly int MacAddressLength = 6;

        /// <summary>Position of the source MAC address within the ethernet header.</summary>
        public static readonly int SourceMacPosition;

        /// <summary>Width of the ethernet type code in bytes.</summary>
        public static readonly int TypeLength = 2;

        /// <summary>Position of the ethernet type field within the ethernet header.</summary>
        public static readonly int TypePosition;

        static EthernetFields()
        {
            SourceMacPosition = MacAddressLength;
            TypePosition = MacAddressLength * 2;
            HeaderLength = TypePosition + TypeLength;
        }
    }