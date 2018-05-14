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
    /// Lengths and offsets to the fields in the LinuxSLL packet
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public class LinuxSLLFields
    {
        /// <summary>
        /// Length of the ethernet protocol field
        /// </summary>
        public static readonly Int32 EthernetProtocolTypeLength = 2;

        /// <summary>
        /// Position of the ethernet protocol type field
        /// </summary>
        public static readonly Int32 EthernetProtocolTypePosition;

        /// <summary>
        /// Link layer address length
        /// </summary>
        public static readonly Int32 LinkLayerAddressLengthLength = 2;

        /// <summary>
        /// Positino of the link layer address length field
        /// </summary>
        public static readonly Int32 LinkLayerAddressLengthPosition;

        /// <summary>
        /// The link layer address field length
        /// NOTE: the actual link layer address MAY be shorter than this
        /// </summary>
        public static readonly Int32 LinkLayerAddressMaximumLength = 8;

        /// <summary>
        /// Position of the link layer address field
        /// </summary>
        public static readonly Int32 LinkLayerAddressPosition;

        /// <summary>
        /// Link layer address type
        /// </summary>
        public static readonly Int32 LinkLayerAddressTypeLength = 2;

        /// <summary>
        /// Position of the link layer address type field
        /// </summary>
        public static readonly Int32 LinkLayerAddressTypePosition;

        /// <summary>
        /// Length of the packet type field
        /// </summary>
        public static readonly Int32 PacketTypeLength = 2;

        /// <summary>
        /// Position of the packet type field
        /// </summary>
        public static readonly Int32 PacketTypePosition = 0;

        /// <summary>
        /// Number of bytes in a SLL header
        /// </summary>
        public static readonly Int32 SLLHeaderLength = 16;

        static LinuxSLLFields()
        {
            LinkLayerAddressTypePosition = PacketTypePosition + PacketTypeLength;
            LinkLayerAddressLengthPosition = LinkLayerAddressTypePosition + LinkLayerAddressTypeLength;
            LinkLayerAddressPosition = LinkLayerAddressLengthPosition + LinkLayerAddressLengthLength;
            EthernetProtocolTypePosition = LinkLayerAddressPosition + LinkLayerAddressMaximumLength;
        }
    }
}