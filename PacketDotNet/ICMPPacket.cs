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
    /// An ICMP packet.
    /// See http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol
    /// </summary>
    [Serializable]
    public class ICMPPacket : InternetPacket
    {
        /// <summary> Fetch the ICMP message type code.</summary>
        /// <remarks>This could be an enum based on the table at http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol#List_of_permitted_control_messages_.28incomplete_list.29</remarks>
        virtual public int Type
        {
            get
            {
                return header.Bytes[header.Offset + ICMPFields.TypePosition];
            }

            set
            {
                header.Bytes[header.Offset + ICMPFields.TypePosition] = (byte)value;
            }
        }

        /// <summary> Fetch the ICMP code </summary>
        virtual public int Code
        {
            get
            {
                return header.Bytes[header.Offset + ICMPFields.CodePosition];
            }

            set
            {
                header.Bytes[header.Offset + ICMPFields.CodePosition] = (byte)value;
            }
        }

        /// <value>
        /// Return the combined type and code as a value of the ICMPTypeCode enum
        /// <exception cref="System.NotImplementedException">If ICMPTypeCode has no entry for the given type and code</exception>
        /// </value>
        virtual public ICMPTypeCode TypeCode
        {
            get
            {
                var val = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                         header.Offset + ICMPFields.TypePosition);

                //TODO: how to handle a mismatch in the mapping? maybe throw here?
                if(Enum.IsDefined(typeof(ICMPTypeCode), val))
                    return (ICMPTypeCode)val;
                else
                    throw new System.NotImplementedException("TypeCode of " + val + " is not defined in ICMPTypeCode");
            }
        }

        public int Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + ICMPFields.ChecksumPosition);
            }

            set
            {
                var theValue = (Int16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ICMPFields.ChecksumPosition);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.LIGHT_BLUE;
            }
        }

        /// <summary> Convert this ICMP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this ICMP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("ICMPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append(TypeCode);
            buffer.Append(", ");
            buffer.Append(" l=" + header.Length);
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}
