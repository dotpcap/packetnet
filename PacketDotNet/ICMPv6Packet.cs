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
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    [Serializable]
    public class ICMPv6Packet : InternetPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = ILogActive.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        private static readonly ILogActive log = ILogActive.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#endif

        /// <value>
        /// The Type value
        /// </value>
        virtual public ICMPv6Types Type
        {
            get
            {
                var val = header.Bytes[header.Offset + ICMPv6Fields.TypePosition];

                //TODO: how to handle a mismatch in the mapping? maybe throw here?
                if(Enum.IsDefined(typeof(ICMPv6Types), val))
                    return (ICMPv6Types)val;
                else
                    throw new System.NotImplementedException("Type of " + val + " is not defined in ICMPv6Types");
            }

            set
            {
                header.Bytes[header.Offset + ICMPv6Fields.TypePosition] = (byte)value;
            }
        }

        /// <summary> Fetch the ICMP code </summary>
        virtual public byte Code
        {
            get
            {
                return header.Bytes[header.Offset + ICMPv6Fields.CodePosition];
            }

            set
            {
                header.Bytes[header.Offset + ICMPv6Fields.CodePosition] = (byte)value;
            }
        }

        /// <value>
        /// Checksum value
        /// </value>
        public ushort Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + ICMPv6Fields.ChecksumPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ICMPv6Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// byte[]/int offset constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public ICMPv6Packet(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        {}

        /// <summary>
        /// byte[]/int Offset/PosixTimeval constructor
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
        public ICMPv6Packet(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            header = new ByteArrayAndOffset(Bytes, Offset, Bytes.Length - Offset);
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.LightBlue;
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
            buffer.Append(Type);
            buffer.Append(Code);
            buffer.Append(", ");
            buffer.Append(" l=" + header.Length);
            buffer.Append(']');

            return buffer.ToString();
        }
    }
}
