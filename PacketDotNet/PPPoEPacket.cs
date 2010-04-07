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
    /// Point to Point Protocol
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    public class PPPoEPacket : Packet
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

        private byte VersionType
        {
            get
            {
                return header.Bytes[header.Offset + PPPoEFields.VersionTypePosition];
            }

            set
            {
                header.Bytes[header.Offset + PPPoEFields.VersionTypePosition] = value;
            }
        }

        /// <summary>
        /// PPPoe version, must be 0x1 according to RFC
        /// </summary>
        public byte Version
        {
            get
            {
                return (byte)((VersionType >> 4) & 0xF0);
            }

            set
            {
                var versionType = VersionType;

                // mask the new value in
                versionType = (byte)((versionType & 0x0F) | ((value << 4) & 0xF0));

                VersionType = versionType;
            }
        }

        /// <summary>
        /// Type, must be 0x1 according to RFC
        /// </summary>
        public byte Type
        {
            get
            {
                return (byte)((VersionType) & 0x0F);
            }

            set
            {
                var versionType = VersionType;

                // mask the new value in
                versionType = (byte)((versionType & 0xF0) | (value & 0xF0));

                VersionType = versionType;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public PPPoECode Code
        {
            get
            {
                return (PPPoECode)EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                                  header.Offset + PPPoEFields.CodePosition);
            }

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + PPPoEFields.CodePosition);
            }
        }

        /// <summary>
        /// Session identifier for this PPPoe packet
        /// </summary>
        public UInt16 SessionId
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + PPPoEFields.SessionIdPosition);
            }

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + PPPoEFields.SessionIdPosition);
            }
        }

        /// <summary>
        /// Length of the PPPoe payload, not including the PPPoe header
        /// </summary>
        public UInt16 Length
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + PPPoEFields.LengthPosition);
            }

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + PPPoEFields.LengthPosition);
            }
        }

        /// <summary>
        /// Construct a new PPPoEPacket from source and destination mac addresses
        /// </summary>
        public PPPoEPacket(PPPoECode Code,
                     UInt16 SessionId)
            : base(new PosixTimeval())
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = PPPoEFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            this.Code = Code;
            this.SessionId = SessionId;

            // setup some typical values and default values
            this.Version = 1;
            this.Type = 1;
            this.Length = 0;
        }

        /// <summary>
        /// Create an PPPoEPacket from a byte array
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public PPPoEPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        {
            log.Debug("");
        }

        /// <summary>
        /// Create an PPPoEPacket from a byte array and a Timeval
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
        public PPPoEPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            // slice off the header portion
            header = new ByteArraySegment(Bytes, Offset, PPPoEFields.HeaderLength);

            // parse the encapsulated bytes
            payloadPacketOrData = ParseEncapsulatedBytes(header, Timeval);
        }

        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment Header,
                                                                        PosixTimeval Timeval)
        {
            // slice off the payload
            var payload = Header.EncapsulatedBytes();
            log.DebugFormat("payload {0}", payload.ToString());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // we assume that we have a PPPPacket as the payload
            payloadPacketOrData.ThePacket = new PPPPacket(payload.Bytes,
                                                          payload.Offset,
                                                          Timeval);

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

            buffer.AppendFormat("[PPPoEPacket] Version {0}, Type {1}, Code {2}, SessionId {3}, Length {4}",
                                Version, Type, Code, SessionId, Length);

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
