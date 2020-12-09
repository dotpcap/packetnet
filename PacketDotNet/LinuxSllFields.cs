/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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