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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An ICMP packet
    /// See http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol
    /// </summary>
    [Serializable]
    public class ICMPv4Packet : InternetPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        /// <value>
        /// The Type/Code enum value
        /// </value>
        virtual public ICMPv4TypeCodes TypeCode
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                          header.Offset + ICMPv4Fields.TypeCodePosition);

                //TODO: how to handle a mismatch in the mapping? maybe throw here?
                if(Enum.IsDefined(typeof(ICMPv4TypeCodes), val))
                    return (ICMPv4TypeCodes)val;
                else
                    throw new System.NotImplementedException("TypeCode of " + val + " is not defined in ICMPv4TypeCode");
            }

            set
            {
                var theValue = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ICMPv4Fields.TypeCodePosition);
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
                                                       header.Offset + ICMPv4Fields.ChecksumPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ICMPv4Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// ID field
        /// </summary>
        public ushort ID
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + ICMPv4Fields.IDPosition);
            }

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + ICMPv4Fields.IDPosition);
            }
        }

        /// <summary>
        /// Sequence field
        /// </summary>
        public ushort Sequence
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + ICMPv4Fields.SequencePosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + ICMPv4Fields.SequencePosition);
            }
        }

        /// <summary>
        /// Contents of the ICMP packet
        /// </summary>
        public byte[] Data
        {
            get
            {
                return payloadPacketOrData.TheByteArraySegment.ActualBytes();
            }

            set
            {
                payloadPacketOrData.TheByteArraySegment = new ByteArraySegment(value, 0, value.Length);
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
        public ICMPv4Packet(byte[] Bytes, int Offset) :
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
        public ICMPv4Packet(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            header = new ByteArraySegment(Bytes, Offset, ICMPv4Fields.HeaderLength);

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();

            
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
            buffer.Append(TypeCode);
            buffer.Append(", ");
            buffer.Append(" l=" + header.Length);
            buffer.Append(']');

            return buffer.ToString();
        }

        /// <summary>
        /// Returns the ICMPv4Packet inside of Packet p or null if
        /// there is no encapsulated ICMPv4Packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="ICMPv4Packet"/>
        /// </returns>
        public static ICMPv4Packet GetEncapsulated(Packet p)
        {
            log.Debug("");

            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    var payload2 = payload.PayloadPacket;
                    if(payload2 is ICMPv4Packet)
                    {
                        return (ICMPv4Packet)payload2;
                    }
                }
            }

            return null;
        }
    }
}
