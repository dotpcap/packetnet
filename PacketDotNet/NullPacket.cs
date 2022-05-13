/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// A packet of link type null.
    /// See http://www.tcpdump.org/linktypes.html
    /// </summary>
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
        /// Construct a new NullPacket from source and destination mac addresses
        /// </summary>
        public NullPacket(NullPacketType nullPacketType)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = NullFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            // setup some typical values and default values
            Protocol = nullPacketType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public NullPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // slice off the header portion as our header
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = NullFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => ParseNextSegment(Header, Protocol));
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.LightPurple;

        /// <summary>
        /// See http://www.tcpdump.org/linktypes.html
        /// </summary>
        public NullPacketType Protocol
        {
            get => (NullPacketType) EndianBitConverter.Little.ToUInt32(Header.Bytes,
                                                                       Header.Offset + NullFields.ProtocolPosition);
            set
            {
                var val = (uint) value;
                EndianBitConverter.Little.CopyBytes(val,
                                                    Header.Bytes,
                                                    Header.Offset + NullFields.ProtocolPosition);
            }
        }

        internal static PacketOrByteArraySegment ParseNextSegment
        (
            ByteArraySegment header,
            NullPacketType protocol)
        {
            // slice off the payload
            var payload = header.NextSegment();

            Log.DebugFormat("Protocol: {0}, payload: {1}", protocol, payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch (protocol)
            {
                case NullPacketType.IPv4:
                {
                    payloadPacketOrData.Packet = new IPv4Packet(payload);
                    break;
                }
                case NullPacketType.IPv6:
                case NullPacketType.IPv6_28:
                case NullPacketType.IPv6_30:
                {
                    payloadPacketOrData.Packet = new IPv6Packet(payload);
                    break;
                }
                //case NullPacketType.IPX:
                default:
                {
                    throw new NotImplementedException("Protocol of " + protocol + " is not implemented");
                }
            }

            return payloadPacketOrData;
        }

        /// <inheritdoc cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
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