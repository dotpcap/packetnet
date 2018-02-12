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
using System.Text;
using PacketDotNet.IP;
using PacketDotNet.MiscUtil.Utils;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.ICMP
{
    /// <summary>
    /// An ICMP packet.
    /// See http://en.wikipedia.org/wiki/ICMPv6
    /// </summary>
    [Serializable]
    public class ICMPv6Packet : InternetPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        /// <value>
        /// The Type value
        /// </value>
        virtual public ICMPv6Types Type
        {
            get
            {
                var val = this.header.Bytes[this.header.Offset + ICMPv6Fields.TypePosition];

                if(Enum.IsDefined(typeof(ICMPv6Types), val))
                    return (ICMPv6Types)val;
                else
                    throw new ArgumentOutOfRangeException("Type of \"" + val + "\" is not defined in ICMPv6Types");
            }

            set => this.header.Bytes[this.header.Offset + ICMPv6Fields.TypePosition] = (byte)value;
        }

        /// <summary> Fetch the ICMP code </summary>
        virtual public byte Code
        {
            get => this.header.Bytes[this.header.Offset + ICMPv6Fields.CodePosition];

            set => this.header.Bytes[this.header.Offset + ICMPv6Fields.CodePosition] = (byte)value;
        }

        /// <value>
        /// Checksum value
        /// </value>
        public ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(this.header.Bytes, this.header.Offset + ICMPv6Fields.ChecksumPosition);

            set
            {
                var theValue = value;
                EndianBitConverter.Big.CopyBytes(theValue, this.header.Bytes, this.header.Offset + ICMPv6Fields.ChecksumPosition);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public ICMPv6Packet(ByteArraySegment bas)
        {
            log.Debug("");

            this.header = new ByteArraySegment(bas);
        }

        /// <summary>
        /// Constructor with parent packet
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>        
        public ICMPv6Packet(ByteArraySegment bas,
                            Packet ParentPacket) : this(bas)
        {
            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// Used to prevent a recursive stack overflow
        /// when recalculating in UpdateCalculatedValues()
        /// </summary>
        private bool skipUpdating = false;

        /// <summary>
        /// Recalculate the checksum
        /// </summary>
        public override void UpdateCalculatedValues ()
        {
            if(this.skipUpdating)
                return;

            // prevent us from entering this routine twice
            // by setting this flag, the act of retrieving the Bytes
            // property will cause this routine to be called which will
            // retrieve Bytes recursively and overflow the stack
            this.skipUpdating = true;

            // start with this packet with a zeroed out checksum field
            this.Checksum = 0;
            var originalBytes = this.Bytes;

            var ipv6Parent = this.ParentPacket as IPv6Packet;
            var bytesToChecksum = ipv6Parent.AttachPseudoIPHeader(originalBytes);

            // calculate the one's complement sum of the tcp header
            this.Checksum = (ushort)ChecksumUtils.OnesComplementSum(bytesToChecksum);

            // clear the skip variable
            this.skipUpdating = false;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public String Color => AnsiEscapeSequences.LightBlue;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[ICMPPacket: Type={2}, Code={3}]{1}",
                    color,
                    colorEscape, this.Type, this.Code);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("type", this.Type.ToString() + " (" + (int) this.Type + ")");
                properties.Add("code", this.Code.ToString());
                // TODO: Implement a checksum verification for ICMPv6
                properties.Add("checksum", "0x" + this.Checksum.ToString("x"));
                // TODO: Implement ICMPv6 Option fields here?

                // calculate the padding needed to right-justify the property names
                int padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("ICMP:  ******* ICMPv6 - \"Internet Control Message Protocol (Version 6)\"- offset=? length=" + this.TotalPacketLength);
                buffer.AppendLine("ICMP:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("ICMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }
                buffer.AppendLine("ICMP:");
            }

            // append the base string output
            buffer.Append((string) base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}
