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
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// A packet of link type null.
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
    [Serializable]
    public class NullPacket : Packet
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// See http://www.tcpdump.org/linktypes.html
        /// </summary>
        public NullPacketType Protocol
        {
            get => (NullPacketType) EndianBitConverter.Little.ToUInt32(Header.Bytes,
                                                                       Header.Offset + NullFields.ProtocolPosition);

            set
            {
                var val = (UInt32) value;
                EndianBitConverter.Little.CopyBytes(val,
                                                    Header.Bytes,
                                                    Header.Offset + NullFields.ProtocolPosition);
            }
        }

        /// <summary>
        /// Construct a new NullPacket from source and destination mac addresses
        /// </summary>
        public NullPacket(NullPacketType nullPacketType)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = NullFields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // setup some typical values and default values
            Protocol = nullPacketType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public NullPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // slice off the header portion as our header
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = NullFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseEncapsulatedBytes(Header, Protocol));
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes
        (
            ByteArraySegment header,
            NullPacketType protocol)
        {
            // slice off the payload
            var payload = header.EncapsulatedBytes();

            Log.DebugFormat("Protocol: {0}, payload: {1}", protocol, payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch (protocol)
            {
                case NullPacketType.IPv4:
                    payloadPacketOrData.Packet = new IPv4Packet(payload);
                    break;
                case NullPacketType.IPv6:
                case NullPacketType.IPv6_28:
                case NullPacketType.IPv6_30:
                    payloadPacketOrData.Packet = new IPv6Packet(payload);
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
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="NullPacket" />
        /// </returns>
        public static NullPacket RandomPacket()
        {
            throw new NotImplementedException();
        }
    }
}