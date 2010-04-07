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
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// PPP packet
    /// See http://en.wikipedia.org/wiki/Point-to-Point_Protocol
    /// </summary>
    public class PPPPacket : Packet
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

        /// <summary>
        /// See http://www.iana.org/assignments/ppp-numbers
        /// </summary>
        public PPPProtocol Protocol
        {
            get
            {
                return (PPPProtocol)EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + PPPFields.ProtocolPosition);
            }

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + PPPFields.ProtocolPosition);
            }
        }

        /// <summary>
        /// Construct a new PPPPacket from source and destination mac addresses
        /// </summary>
        public PPPPacket(PPPoECode Code,
                     UInt16 SessionId)
            : base(new PosixTimeval())
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = PPPFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // setup some typical values and default values
            this.Protocol = PPPProtocol.Padding;
        }

        /// <summary>
        /// Create an PPPPacket from a byte array
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public PPPPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        {
            log.Debug("");
        }

        /// <summary>
        /// Create an PPPPacket from a byte array and a Timeval
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
        public PPPPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            // slice off the header portion as our header
            header = new ByteArraySegment(Bytes, Offset, PPPFields.HeaderLength);

            // parse the encapsulated bytes
            payloadPacketOrData = ParseEncapsulatedBytes(header, Timeval, Protocol);
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment Header,
                                                                        PosixTimeval Timeval,
                                                                        PPPProtocol Protocol)
        {
            // slice off the payload
            var payload = Header.EncapsulatedBytes();

            log.DebugFormat("payload: {0}", payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch(Protocol)
            {
            case PPPProtocol.IPv4:
                payloadPacketOrData.ThePacket = new IPv4Packet(payload.Bytes,
                                                               payload.Offset,
                                                               Timeval);
                break;
            case PPPProtocol.IPv6:
                payloadPacketOrData.ThePacket = new IPv6Packet(payload.Bytes,
                                                               payload.Offset,
                                                               Timeval);
                break;
            default:
                throw new System.NotImplementedException("Protocol of " + Protocol + " is not implemented");
            }

            return payloadPacketOrData;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override System.String Color
        {
            get
            {
                return AnsiEscapeSequences.DarkGray;
            }
        }

        /// <summary> Convert this packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            var buffer = new System.Text.StringBuilder();

            buffer.AppendFormat("[PPPPacket] Protocol {0}",
                                Protocol);

            // append the base output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            //TODO: just output the colored output for now
            return ToColoredString(colored);
        }

        /// <summary>
        /// Generate a random PPPoEPacket
        /// </summary>
        /// <returns>
        /// A <see cref="PPPoEPacket"/>
        /// </returns>
        public static PPPoEPacket RandomPacket()
        {
            throw new System.NotImplementedException();
        }
    }
}

