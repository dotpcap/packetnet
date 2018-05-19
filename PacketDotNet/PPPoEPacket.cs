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
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Point to Point Protocol
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PPPoEPacket : Packet
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

        private Byte VersionType
        {
            get => Header.Bytes[Header.Offset + PPPoEFields.VersionTypePosition];

            set => Header.Bytes[Header.Offset + PPPoEFields.VersionTypePosition] = value;
        }

        /// <summary>
        /// PPPoe version, must be 0x1 according to RFC
        /// </summary>
        /// FIXME: This currently outputs the wrong version number
        public Byte Version
        {
            get => (Byte) ((VersionType >> 4) & 0xF0);

            set
            {
                var versionType = VersionType;

                // mask the new value in
                versionType = (Byte) ((versionType & 0x0F) | ((value << 4) & 0xF0));

                VersionType = versionType;
            }
        }

        /// <summary>
        /// Type, must be 0x1 according to RFC
        /// </summary>
        public Byte Type
        {
            get => (Byte) (VersionType & 0x0F);

            set
            {
                var versionType = VersionType;

                // mask the new value in
                versionType = (Byte) ((versionType & 0xF0) | (value & 0xF0));

                VersionType = versionType;
            }
        }

        /// <summary>
        /// </summary>
        /// FIXME: This currently outputs the wrong code
        public PPPoECode Code
        {
            get => (PPPoECode) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                               Header.Offset + PPPoEFields.CodePosition);

            set
            {
                var val = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + PPPoEFields.CodePosition);
            }
        }

        /// <summary>
        /// Session identifier for this PPPoe packet
        /// </summary>
        public UInt16 SessionId
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + PPPoEFields.SessionIdPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + PPPoEFields.SessionIdPosition);
            }
        }

        /// <summary>
        /// Length of the PPPoe payload, not including the PPPoe header
        /// </summary>
        public UInt16 Length
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + PPPoEFields.LengthPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + PPPoEFields.LengthPosition);
            }
        }

        /// <summary>
        /// Construct a new PPPoEPacket from source and destination mac addresses
        /// </summary>
        public PPPoEPacket
        (
            PPPoECode code,
            UInt16 sessionId)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = PPPoEFields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            Code = code;
            SessionId = sessionId;

            // setup some typical values and default values
            Version = 1;
            Type = 1;
            Length = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public PPPoEPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = PPPoEFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseEncapsulatedBytes(Header));
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment header)
        {
            // slice off the payload
            var payload = header.EncapsulatedBytes();
            Log.DebugFormat("payload {0}", payload);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var payloadPacketOrData = new PacketOrByteArraySegment();

            // we assume that we have a PPPPacket as the payload
            payloadPacketOrData.Packet = new PPPPacket(payload);

            return payloadPacketOrData;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[PPPoEPacket: Version={2}, Type={3}, Code={4}, SessionId={5}, Length={6}]{1}",
                                        color,
                                        colorEscape,
                                        Version,
                                        Type,
                                        Code,
                                        SessionId,
                                        Length);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    var properties = new Dictionary<String, String>
                    {
                        // FIXME: The version output is incorrect
                        {"", Convert.ToString(Version, 2).PadLeft(4, '0') + " .... = version: " + Version},
                        {" ", ".... " + Convert.ToString(Type, 2).PadLeft(4, '0') + " = type: " + Type},
                        // FIXME: The Code output is incorrect
                        {"code", Code + " (0x" + Code.ToString("x") + ")"},
                        {"session id", "0x" + SessionId.ToString("x")}
                    };
                    // TODO: Implement a PayloadLength property for PPPoE
                    //properties.Add("payload length", PayloadLength.ToString());

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("PPPoE:  ******* PPPoE - \"Point-to-Point Protocol over Ethernet\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("PPPoE:");
                    foreach (var property in properties)
                    {
                        if (property.Key.Trim() != "")
                        {
                            buffer.AppendLine("PPPoE: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                        }
                        else
                        {
                            buffer.AppendLine("PPPoE: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                        }
                    }

                    buffer.AppendLine("PPPoE:");
                    break;
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random PPPoEPacket
        /// </summary>
        /// <returns>
        /// A <see cref="PPPoEPacket" />
        /// </returns>
        public static PPPoEPacket RandomPacket()
        {
            throw new NotImplementedException();
        }
    }
}