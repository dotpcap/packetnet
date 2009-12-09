/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
﻿using System;
using System.Net.NetworkInformation;
using Packet.Net.Utils;
using MiscUtil.Conversion;

namespace Packet.Net
{
    public class EthernetPacket : DataLinkPacket
    {
        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress SourceHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[EthernetFields.MacAddressLength];
                Array.Copy(header.Bytes, header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress, 0, header.Bytes, header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress.Length);
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress DestinationHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[EthernetFields.MacAddressLength];
                Array.Copy(header.Bytes, header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress, 0, header.Bytes, header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Type of packet that this ethernet packet encapsulates
        /// </value>
        public virtual EthernetPacketType Type
        {
            get
            {
                return (EthernetPacketType)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                          header.Offset + EthernetFields.TypePosition);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + EthernetFields.TypePosition);
            }
        }

        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket(PhysicalAddress SourceHwAddress,
                              PhysicalAddress DestinationHwAddress,
                              EthernetPacketType ethernetPacketType)
        {
            // allocate memory for this packet
            int offset = 0;
            int length = EthernetFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArrayAndOffset(headerBytes, offset, length);

            // set the instance values
            this.SourceHwAddress = SourceHwAddress;
            this.DestinationHwAddress = DestinationHwAddress;
            this.Type = ethernetPacketType;
        }

        /// <summary>
        /// Create an EthernetPacket from a byte array 
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public EthernetPacket(byte[] bytes, int offset) :
            this(bytes, offset, new Timeval())
        { }

        /// <summary>
        /// Create an EthernetPacket from a byte array and a Timeval 
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="timeval">
        /// A <see cref="Timeval"/>
        /// </param>
        public EthernetPacket(byte[] bytes, int offset, Timeval timeval) :
            base(timeval)
        {
            header = new ByteArrayAndOffset(bytes, offset, EthernetFields.HeaderLength);

            //TODO: we need to add more code here to parse the containing packet
            // and either assign the bytes to payloaddata or payloadpacket
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override System.String Color
        {
            get
            {
                return AnsiEscapeSequences.DARK_GRAY;
            }
        }

        /// <summary> Convert this ethernet packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ethernet packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("EthernetPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.RESET);
            buffer.Append(": ");
            buffer.Append(SourceHwAddress + " -> " + DestinationHwAddress);
            buffer.Append(" proto=" + Type.ToString() + " (0x" + System.Convert.ToString((ushort)Type, 16) + ")");
            buffer.Append(" l=" + EthernetFields.HeaderLength); // + "," + data.length);
            buffer.Append(']');

            // append the base output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            //TODO: just output the colored output for now
            return ToColoredString(colored);
        }
    }
}
