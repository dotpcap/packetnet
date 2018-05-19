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
    /// An ICMP packet
    /// See http://en.wikipedia.org/wiki/Internet_Control_Message_Protocol
    /// </summary>
    [Serializable]
    public sealed class ICMPv4Packet : InternetPacket
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
        /// The Type/Code enum value
        /// </value>
        public ICMPv4TypeCodes TypeCode
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                          Header.Offset + ICMPv4Fields.TypeCodePosition);
                return (ICMPv4TypeCodes) val;
            }

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ICMPv4Fields.TypeCodePosition);
            }
        }

        /// <value>
        /// Checksum value
        /// </value>
        public UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + ICMPv4Fields.ChecksumPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ICMPv4Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// ID field
        /// </summary>
        public UInt16 ID
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + ICMPv4Fields.IDPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ICMPv4Fields.IDPosition);
            }
        }

        /// <summary>
        /// Sequence field
        /// </summary>
        public UInt16 Sequence
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + ICMPv4Fields.SequencePosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + ICMPv4Fields.SequencePosition);
        }

        /// <summary>
        /// Contents of the ICMP packet
        /// </summary>
        public Byte[] Data
        {
            get => PayloadPacketOrData.Value.ByteArraySegment.ActualBytes();

            set => PayloadPacketOrData.Value.ByteArraySegment = new ByteArraySegment(value, 0, value.Length);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ICMPv4Packet(ByteArraySegment bas)
        {
            Log.Debug("");

            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = ICMPv4Fields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment();
                result.ByteArraySegment = Header.EncapsulatedBytes();
                return result;
            });
        }

        /// <summary>
        /// Construct with parent packet
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public ICMPv4Packet
        (
            ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            ParentPacket = parentPacket;
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
                    buffer.AppendFormat("{0}[ICMPPacket: TypeCode={2}]{1}",
                                        color,
                                        colorEscape,
                                        TypeCode);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    var properties = new Dictionary<String, String>
                    {
                        {"type/code", TypeCode + " (0x" + TypeCode.ToString("x") + ")"},
                        // TODO: Implement checksum verification for ICMPv4
                        {"checksum", Checksum.ToString("x")},
                        {"identifier", "0x" + ID.ToString("x")},
                        {"sequence number", Sequence + " (0x" + Sequence.ToString("x") + ")"}
                    };

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("ICMP:  ******* ICMPv4 - \"Internet Control Message Protocol (Version 4)\" - offset=? length=" + TotalPacketLength);
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