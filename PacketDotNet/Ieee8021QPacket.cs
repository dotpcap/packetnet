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
using MiscUtil.Conversion;
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
        /// <value>
        /// Type of packet that this vlan packet encapsulates
        /// </value>
        public virtual EthernetPacketType Type
        {
            get
            {
                return (EthernetPacketType)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                          header.Offset + Ieee8021QFields.TypePosition);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + Ieee8021QFields.TypePosition);
            }
        }

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
                tci >>= (16 - 3); // priority is the upper 3 bits
                return (IeeeP8021PPriorities)tci;
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing Priority off and then back in from value
                ushort val = (ushort)value;
                tci = (ushort)((tci & 0x1FFF) | ((val & 0x7) << (16 - 3)));
                TagControlInformation = tci;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance canonical format indicator.
        /// </summary>
        /// <value>
        /// <c>true</c> if the mac address is in non-canonical format <c>false</c> if otherwise.
        /// </value>
        public bool CanonicalFormatIndicator
        {
            get
            {
                var tci = TagControlInformation;
                tci >>= 12;
                return ((tci & 0x1) == 1) ? true : false;
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing CFI off and then back in from value
                int val = ((value == true) ? 1 : 0);
                tci = (ushort)((tci & 0xEFFF) | (val << 12));
                TagControlInformation = tci;
            }
        }
 
        /// <summary>
        /// Gets or sets the VLAN identifier.
        /// </summary>
        /// <value>
        /// The VLAN identifier.
        /// </value>
        public ushort VLANIdentifier
        {
            get
            {
                var tci = TagControlInformation;
                return (ushort)(tci & 0xFFF);
            }

            set
            {
                var tci = TagControlInformation;

                // mask the existing vlan id off
                tci = (ushort)((tci & 0xF000) | (value & 0xFFF));
                TagControlInformation = tci;
            }
        }

        private ushort TagControlInformation
        {
            get
            {
                return (ushort)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                              header.Offset + Ieee8021QFields.TagControlInformationPosition);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + Ieee8021QFields.TagControlInformationPosition);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.LightCyan;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee8021QPacket(ByteArraySegment bas)
        {
            // set the header field, header field values are retrieved from this byte array
            header = new ByteArraySegment(bas);
            header.Length = Ieee8021QFields.HeaderLength;

            // parse the payload via an EthernetPacket method
            payloadPacketOrData = EthernetPacket.ParseEncapsulatedBytes(header,
                                                                        Type);
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[Ieee8021QPacket: PriorityControlPoint={2}, CanonicalFormatIndicator={3}, Type={4}]{1}",
                    color,
                    colorEscape,
                    PriorityControlPoint,
                    CanonicalFormatIndicator,
                    Type);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("priority", PriorityControlPoint + " (0x" + PriorityControlPoint.ToString("x") + ")");
                properties.Add("canonical format indicator", CanonicalFormatIndicator.ToString());
                properties.Add("type", Type.ToString() + " (0x" + Type.ToString("x") + ")");
                properties.Add ("VLANIdentifier", VLANIdentifier.ToString () + " (0x" + VLANIdentifier.ToString ("x") + ")");

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

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
