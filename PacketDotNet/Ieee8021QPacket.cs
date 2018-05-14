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
 *  Copyright 2013 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// 802.1Q vlan packet
    /// http://en.wikipedia.org/wiki/IEEE_802.1Q
    /// </summary>
    [Serializable]
    public class Ieee8021QPacket : InternetPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public Ieee8021QPacket(ByteArraySegment bas)
        {
            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(bas);
            Header.Length = Ieee8021QFields.HeaderLength;

            // parse the payload via an EthernetPacket method
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => EthernetPacket.ParseEncapsulatedBytes(Header,
                                                                                                                 Type));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance canonical format indicator.
        /// </summary>
        /// <value>
        /// <c>true</c> if the mac address is in non-canonical format <c>false</c> if otherwise.
        /// </value>
        public Boolean CanonicalFormatIndicator
        {
            get
            {
                var tci = TagControlInformation;
                tci >>= 12;
                return (tci & 0x1) == 1 ? true : false;
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing CFI off and then back in from value
                var val = value ? 1 : 0;
                tci = (UInt16) ((tci & 0xEFFF) | (val << 12));
                TagControlInformation = tci;
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.LightCyan;

        /// <summary>
        /// Gets or sets the priority control point.
        /// </summary>
        /// <value>
        /// The priority control point.
        /// </value>
        public IeeeP8021PPriorities PriorityControlPoint
        {
            get
            {
                var tci = TagControlInformation;
                tci >>= 16 - 3; // priority is the upper 3 bits
                return (IeeeP8021PPriorities) tci;
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing Priority off and then back in from value
                var val = (UInt16) value;
                tci = (UInt16) ((tci & 0x1FFF) | ((val & 0x7) << (16 - 3)));
                TagControlInformation = tci;
            }
        }

        /// <value>
        /// Type of packet that this vlan packet encapsulates
        /// </value>
        public virtual EthernetPacketType Type
        {
            get => (EthernetPacketType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                       Header.Offset + Ieee8021QFields.TypePosition);

            set
            {
                var val = (Int16) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + Ieee8021QFields.TypePosition);
            }
        }

        /// <summary>
        /// Gets or sets the VLAN identifier.
        /// </summary>
        /// <value>
        /// The VLAN identifier.
        /// </value>
        public UInt16 VLANIdentifier
        {
            get
            {
                var tci = TagControlInformation;
                return (UInt16) (tci & 0xFFF);
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing vlan id off
                tci = (UInt16) ((tci & 0xF000) | (value & 0xFFF));
                TagControlInformation = tci;
            }
        }

        private UInt16 TagControlInformation
        {
            get => (UInt16) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                           Header.Offset + Ieee8021QFields.TagControlInformationPosition);

            set
            {
                var val = (Int16) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + Ieee8021QFields.TagControlInformationPosition);
            }
        }

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

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[Ieee8021QPacket: PriorityControlPoint={2}, CanonicalFormatIndicator={3}, Type={4}]{1}",
                                    color,
                                    colorEscape,
                                    PriorityControlPoint,
                                    CanonicalFormatIndicator,
                                    Type);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"priority", PriorityControlPoint + " (0x" + PriorityControlPoint.ToString("x") + ")"},
                    {"canonical format indicator", CanonicalFormatIndicator.ToString()},
                    {"type", Type + " (0x" + Type.ToString("x") + ")"},
                    {"VLANIdentifier", VLANIdentifier + " (0x" + VLANIdentifier.ToString("x") + ")"}
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("Ieee802.1Q:  ******* Ieee802.1Q - \"VLan tag\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("Ieee802.1Q:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("Ieee802.1Q: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("Ieee802.1Q:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}