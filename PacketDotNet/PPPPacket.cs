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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// PPP packet
    /// See http://en.wikipedia.org/wiki/Point-to-Point_Protocol
    /// </summary>
    [Serializable]
    public class PppPacket : Packet
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
        /// Construct a new PppPacket from source and destination mac addresses
        /// </summary>
        public PppPacket()
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = PppFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // setup some typical values and default values
            Protocol = PppProtocol.Padding;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public PppPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // slice off the header portion as our header
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = PppFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseNextSegment(Header, Protocol), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        /// <summary>
        /// See http://www.iana.org/assignments/ppp-numbers
        /// </summary>
        public PppProtocol Protocol
        {
            get => (PppProtocol) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                 Header.Offset + PppFields.ProtocolPosition);
            set
            {
                var val = (ushort) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + PppFields.ProtocolPosition);
            }
        }

        /// <summary>
        /// Parses the next segment.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="protocol">The protocol.</param>
        /// <returns><see cref="PacketOrByteArraySegment"/>.</returns>
        private static PacketOrByteArraySegment ParseNextSegment
        (
            ByteArraySegment header,
            PppProtocol protocol)
        {
            // slice off the payload
            var payload = header.NextSegment();

            Log.DebugFormat("payload: {0}", payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            switch (protocol)
            {
                case PppProtocol.IPv4:
                {
                    payloadPacketOrData.Packet = new IPv4Packet(payload);
                    break;
                }
                case PppProtocol.IPv6:
                {
                    payloadPacketOrData.Packet = new IPv6Packet(payload);
                    break;
                }
                default:
                {
                    //Probably a control packet, lets just add it to the data
                    payloadPacketOrData.ByteArraySegment = payload;
                    break;
                }
            }

            return payloadPacketOrData;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[PppPacket: Protocol={2}]{1}",
                                    color,
                                    colorEscape,
                                    Protocol);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "protocol", Protocol + " (0x" + Protocol.ToString("x") + ")" }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("PPP:  ******* PPP - \"Point-to-Point Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("PPP:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("PPP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("PPP:");
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}