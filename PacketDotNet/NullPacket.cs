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
 *  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Text;
using PacketDotNet.IP;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet
{
    /// <summary>
    ///     A packet of link type null.
    ///     See http://www.tcpdump.org/linktypes.html
    /// </summary>
    [Serializable]
    public class NullPacket : Packet
    {
#if DEBUG
        private static readonly log4net.ILog Log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        ///     See http://www.tcpdump.org/linktypes.html
        /// </summary>
        public NullPacketType Protocol
        {
            get => (NullPacketType) EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + NullFields.ProtocolPosition);

            set
            {
                var val = (UInt32) value;
                EndianBitConverter.Little.CopyBytes(val, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + NullFields.ProtocolPosition);
            }
        }

        /// <summary>
        ///     Construct a new NullPacket from source and destination mac addresses
        /// </summary>
        public NullPacket(NullPacketType theType)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = NullFields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            // setup some typical values and default values
            this.Protocol = theType;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public NullPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // slice off the header portion as our header
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = NullFields.HeaderLength
            };

            // parse the encapsulated bytes
            this.PayloadPacketOrData = ParseEncapsulatedBytes(this.HeaderByteArraySegment, this.Protocol);
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment header,
            NullPacketType protocol)
        {
            // slice off the payload
            var payload = header.EncapsulatedBytes();

            Log.DebugFormat("Protocol: {0}, payload: {1}", protocol, payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch (protocol)
            {
                case NullPacketType.IpV4:
                    payloadPacketOrData.ThePacket = new IPv4Packet(payload);
                    break;
                case NullPacketType.IpV6:
                case NullPacketType.IpV6_28:
                case NullPacketType.IpV6_30:
                    payloadPacketOrData.ThePacket = new IPv6Packet(payload);
                    break;
                case NullPacketType.IPX:
                default:
                    throw new NotImplementedException("Protocol of " + protocol + " is not implemented");
            }

            return payloadPacketOrData;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.LightPurple;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            buffer.Append(base.ToString(outputFormat));
            return buffer.ToString();
        }

        /// <summary>
        ///     Generate a random packet
        /// </summary>
        /// <returns>
        ///     A <see cref="NullPacket" />
        /// </returns>
        public static NullPacket RandomPacket()
        {
            throw new NotImplementedException();
        }
    }
}