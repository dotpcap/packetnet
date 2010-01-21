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
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An IGMP packet.
    /// </summary>
    [Serializable]
    public class IGMPv2Packet : InternetPacket
    {
        /// <value>
        /// The type of IGMP message
        /// </value>
        virtual public IGMPMessageType Type
        {
            get
            {
                return (IGMPMessageType)header.Bytes[header.Offset + IGMPv2Fields.TypePosition];
            }

            set
            {
                header.Bytes[header.Offset + IGMPv2Fields.TypePosition] = (byte)value;
            }
        }

        /// <summary> Fetch the IGMP max response time.</summary>
        virtual public int MaxResponseTime
        {
            get
            {
                return header.Bytes[header.Offset + IGMPv2Fields.MaxResponseTimePosition];
            }

            set
            {
                header.Bytes[header.Offset + IGMPv2Fields.MaxResponseTimePosition] = (byte)value;
            }
        }

        /// <summary> Fetch the IGMP header checksum.</summary>
        virtual public int Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + IGMPv2Fields.ChecksumPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + IGMPv2Fields.ChecksumPosition);
            }
        }

        /// <summary> Fetch the IGMP group address.</summary>
        virtual public System.Net.IPAddress GroupAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + IGMPv2Fields.GroupAddressPosition,
                                             header.Bytes);
            }

        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Brown;
            }

        }

        /// <summary>
        /// byte[]/int Offset constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public IGMPv2Packet(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        { }

        /// <summary>
        /// byte[]/int offset/PosixTimeval constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public IGMPv2Packet(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            throw new System.NotImplementedException();
        }

        /// <summary> Convert this IGMP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this IGMP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IGMPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append(Type);
            buffer.Append(", ");
            buffer.Append(GroupAddress + ": ");
            buffer.Append(" l=" + IGMPv2Fields.HeaderLength);
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}