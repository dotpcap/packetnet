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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An ICMP packet.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    [Serializable]
    public sealed class ICMPv6Packet : InternetPacket
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

        /// <value>
        /// The Type value
        /// </value>
        public ICMPv6Types Type
        {
            get => (ICMPv6Types) Header.Bytes[Header.Offset + ICMPv6Fields.TypePosition];

            set => Header.Bytes[Header.Offset + ICMPv6Fields.TypePosition] = (Byte) value;
        }

        /// <summary> Fetch the ICMP code </summary>
        public Byte Code
        {
            get => Header.Bytes[Header.Offset + ICMPv6Fields.CodePosition];

            set => Header.Bytes[Header.Offset + ICMPv6Fields.CodePosition] = value;
        }

        /// <value>
        /// Checksum value
        /// </value>
        public UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + ICMPv6Fields.ChecksumPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ICMPv6Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ICMPv6Packet(ByteArraySegment bas)
        {
            Log.Debug("");

            Header = new ByteArraySegment(bas);
        }

        /// <summary>
        /// Constructor with parent packet
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public ICMPv6Packet
        (
            ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Used to prevent a recursive stack overflow
        /// when recalculating in UpdateCalculatedValues()
        /// </summary>
        private Boolean _skipUpdating;

        /// <summary>
        /// Recalculate the checksum
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (_skipUpdating)
                return;


            // prevent us from entering this routine twice
            // by setting this flag, the act of retrieving the Bytes
            // property will cause this routine to be called which will
            // retrieve Bytes recursively and overflow the stack
            _skipUpdating = true;

            // start with this packet with a zeroed out checksum field
            Checksum = 0;

            var dataToChecksum = BytesHighPerformance;
            var ipv6Parent = ParentPacket as IPv6Packet;

            Checksum = (ushort) ChecksumUtils.OnesComplementSum(dataToChecksum, ipv6Parent?.GetPseudoIPHeader(dataToChecksum.Length) ?? new byte[0]);

            // clear the skip variable
            _skipUpdating = false;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.LightBlue;

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
                    buffer.AppendFormat("{0}[ICMPPacket: Type={2}, Code={3}]{1}",
                                        color,
                                        colorEscape,
                                        Type,
                                        Code);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    var properties = new Dictionary<String, String>
                    {
                        {"type", Type + " (" + (Int32) Type + ")"},
                        {"code", Code.ToString()},
                        // TODO: Implement a checksum verification for ICMPv6
                        {"checksum", "0x" + Checksum.ToString("x")}
                    };
                    // TODO: Implement ICMPv6 Option fields here?

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("ICMP:  ******* ICMPv6 - \"Internet Control Message Protocol (Version 6)\"- offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("ICMP:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("ICMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("ICMP:");
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}