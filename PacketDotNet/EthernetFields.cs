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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
using System;

namespace PacketDotNet
{
    /// <summary>
    /// Ethernet protocol field encoding information.
    /// </summary>
    public class EthernetFields
    {
        /// <summary> Width of the ethernet type code in bytes.</summary>
        public readonly static int TypeLength = 2;

        /// <summary> Position of the destination MAC address within the ethernet header.</summary>
        public readonly static int DestinationMacPosition = 0;

        /// <summary> Position of the source MAC address within the ethernet header.</summary>
        public readonly static int SourceMacPosition;

        /// <summary> Position of the ethernet type field within the ethernet header.</summary>
        public readonly static int TypePosition;

        /// <summary> Total length of an ethernet header in bytes.</summary>
        public readonly static int HeaderLength; // == 14

        static EthernetFields()
        {
            SourceMacPosition = EthernetFields.MacAddressLength;
            TypePosition = EthernetFields.MacAddressLength * 2;
            HeaderLength = EthernetFields.TypePosition + EthernetFields.TypeLength;
        }

        /// <summary>
        /// size of an ethernet mac address in bytes
        /// </summary>
        public readonly static int MacAddressLength = 6;
    }
}
