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

namespace PacketDotNet
{
    /// <summary>
    /// Lengths and offsets to the fields in the LinuxSll packet
    /// See http://github.com/mcr/libpcap/blob/master/pcap/sll.h
    /// </summary>
    public struct LinuxSllFields
    {
        /// <summary>
        /// Length of the ethernet protocol field
        /// </summary>
        public static readonly int EthernetProtocolTypeLength = 2;

        /// <summary>
        /// Position of the ethernet protocol type field
        /// </summary>
        public static readonly int EthernetProtocolTypePosition;

        /// <summary>
        /// Link layer address length
        /// </summary>
        public static readonly int LinkLayerAddressLengthLength = 2;

        /// <summary>
        /// Positino of the link layer address length field
        /// </summary>
        public static readonly int LinkLayerAddressLengthPosition;

        /// <summary>
        /// The link layer address field length
        /// NOTE: the actual link layer address MAY be shorter than this
        /// </summary>
        public static readonly int LinkLayerAddressMaximumLength = 8;

        /// <summary>
        /// Position of the link layer address field
        /// </summary>
        public static readonly int LinkLayerAddressPosition;

        /// <summary>
        /// Link layer address type
        /// </summary>
        public static readonly int LinkLayerAddressTypeLength = 2;

        /// <summary>
        /// Position of the link layer address type field
        /// </summary>
        public static readonly int LinkLayerAddressTypePosition;

        /// <summary>
        /// Length of the packet type field
        /// </summary>
        public static readonly int PacketTypeLength = 2;

        /// <summary>
        /// Position of the packet type field
        /// </summary>
        public static readonly int PacketTypePosition = 0;

        /// <summary>
        /// Number of bytes in a SLL header
        /// </summary>
        public static readonly int SLLHeaderLength = 16;

        static LinuxSllFields()
        {
            LinkLayerAddressTypePosition = PacketTypePosition + PacketTypeLength;
            LinkLayerAddressLengthPosition = LinkLayerAddressTypePosition + LinkLayerAddressTypeLength;
            LinkLayerAddressPosition = LinkLayerAddressLengthPosition + LinkLayerAddressLengthLength;
            EthernetProtocolTypePosition = LinkLayerAddressPosition + LinkLayerAddressMaximumLength;
        }
    }
}