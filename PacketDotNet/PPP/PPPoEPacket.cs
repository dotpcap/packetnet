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
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.PPP
{
    /// <summary>
    /// Point to Point Protocol
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PPPoEPacket : Packet
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        private Byte VersionType
        {
            get => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + PPPoEFields.VersionTypePosition];

            set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + PPPoEFields.VersionTypePosition] = value;
        }

        /// <summary>
        /// PPPoe version, must be 0x1 according to RFC
        /// </summary>
        /// FIXME: This currently outputs the wrong version number
        public Byte Version
        {
            get => (Byte)((this.VersionType >> 4) & 0xF0);

            set
            {
                var versionType = this.VersionType;

                // mask the new value in
                versionType = (Byte)((versionType & 0x0F) | ((value << 4) & 0xF0));

                this.VersionType = versionType;
            }
        }

        /// <summary>
        /// Type, must be 0x1 according to RFC
        /// </summary>
        public Byte Type
        {
            get => (Byte)((this.VersionType) & 0x0F);

            set
            {
                var versionType = this.VersionType;

                // mask the new value in
                versionType = (Byte)((versionType & 0xF0) | (value & 0xF0));

                this.VersionType = versionType;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// FIXME: This currently outputs the wrong code
        public PPPoECode Code
        {
            get => (PPPoECode)EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.CodePosition);

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.CodePosition);
            }
        }

        /// <summary>
        /// Session identifier for this PPPoe packet
        /// </summary>
        public UInt16 SessionId
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.SessionIdPosition);

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.SessionIdPosition);
            }
        }

        /// <summary>
        /// Length of the PPPoe payload, not including the PPPoe header
        /// </summary>
        public UInt16 Length
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.LengthPosition);

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + PPPoEFields.LengthPosition);
            }
        }

        /// <summary>
        /// Construct a new PPPoEPacket from source and destination mac addresses
        /// </summary>
        public PPPoEPacket(PPPoECode code,
                     UInt16 sessionId)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = PPPoEFields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            this.Code = code;
            this.SessionId = sessionId;

            // setup some typical values and default values
            this.Version = 1;
            this.Type = 1;
            this.Length = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public PPPoEPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // slice off the header portion
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = PPPoEFields.HeaderLength
            };

            // parse the encapsulated bytes
            this.PayloadPacketOrData = ParseEncapsulatedBytes(this.HeaderByteArraySegment);
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment header)
        {
            // slice off the payload
            var payload = header.EncapsulatedBytes();
            Log.DebugFormat("payload {0}", payload.ToString());

            var payloadPacketOrData = new PacketOrByteArraySegment
            {

                // we assume that we have a PPPPacket as the payload
                ThePacket = new PPPPacket(payload)
            };

            return payloadPacketOrData;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            String color = "";
            String colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[PPPoEPacket: Version={2}, Type={3}, Code={4}, SessionId={5}, Length={6}]{1}",
                        color,
                        colorEscape, this.Version, this.Type, this.Code, this.SessionId, this.Length);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>
                    {
                        // FIXME: The version output is incorrect
                        { "", Convert.ToString((Byte)this.Version, 2).PadLeft(4, '0') + " .... = version: " + this.Version.ToString() },
                        { " ", ".... " + Convert.ToString((Byte)this.Type, 2).PadLeft(4, '0') + " = type: " + this.Type.ToString() },
                        // FIXME: The Code output is incorrect
                        { "code", this.Code.ToString() + " (0x" + this.Code.ToString("x") + ")" },
                        { "session id", "0x" + this.SessionId.ToString("x") }
                    };
                    // TODO: Implement a PayloadLength property for PPPoE
                    //properties.Add("payload length", PayloadLength.ToString());

                    // calculate the padding needed to right-justify the property names
                    Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("PPPoE:  ******* PPPoE - \"Point-to-Point Protocol over Ethernet\" - offset=? length=" + this.TotalPacketLength);
                    buffer.AppendLine("PPPoE:");
                    foreach(var property in properties)
                    {
                        if(property.Key.Trim() != "")
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
            buffer.Append((String) base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random PPPoEPacket
        /// </summary>
        /// <returns>
        /// A <see cref="PPPoEPacket"/>
        /// </returns>
        public static PPPoEPacket RandomPacket()
        {
            throw new NotImplementedException();
        }
    }
}
