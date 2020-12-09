/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet
{
    /// <summary>
    /// 802.1Q vlan packet
    /// http://en.wikipedia.org/wiki/IEEE_802.1Q
    /// </summary>
    public class Ieee8021QPacket : InternetPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public Ieee8021QPacket(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment) { Length = Ieee8021QFields.HeaderLength };

            // parse the payload via an EthernetPacket method
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => EthernetPacket.ParseNextSegment(Header,
                                                                                                           Type));
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
                return (tci & 0x1) == 1;
            }
            set
            {
                var tci = TagControlInformation;

                // mask the existing CFI off and then back in from value
                var val = value ? 1 : 0;
                tci = (ushort) ((tci & 0xEFFF) | (val << 12));
                TagControlInformation = tci;
            }
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.LightCyan;

        /// <summary>
        /// Gets or sets the priority control point.
        /// </summary>
        /// <value>
        /// The priority control point.
        /// </value>
        public IeeeP8021PPriority PriorityControlPoint
        {
            get
            {
                var tci = TagControlInformation;
                tci >>= 16 - 3; // priority is the upper 3 bits
                return (IeeeP8021PPriority) tci;
            }
            set
            {
                var tci = TagControlInformation;

                // mask the existing Priority off and then back in from value
                var val = (ushort) value;
                tci = (ushort) ((tci & 0x1FFF) | ((val & 0x7) << (16 - 3)));
                TagControlInformation = tci;
            }
        }

        /// <value>
        /// Type of packet that this vlan packet encapsulates
        /// </value>
        public virtual EthernetType Type
        {
            get => (EthernetType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + Ieee8021QFields.TypePosition);
            set
            {
                var val = (short) value;
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
        public ushort VlanIdentifier
        {
            get
            {
                var tci = TagControlInformation;
                return (ushort) (tci & 0xFFF);
            }
            set
            {
                var tci = TagControlInformation;

                // mask the existing vlan id off
                tci = (ushort) ((tci & 0xF000) | (value & 0xFFF));
                TagControlInformation = tci;
            }
        }

        private ushort TagControlInformation
        {
            get => (ushort) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                           Header.Offset + Ieee8021QFields.TagControlInformationPosition);
            set
            {
                var val = (short) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + Ieee8021QFields.TagControlInformationPosition);
            }
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
                var properties = new Dictionary<string, string>
                {
                    { "priority", PriorityControlPoint + " (0x" + PriorityControlPoint.ToString("x") + ")" },
                    { "canonical format indicator", CanonicalFormatIndicator.ToString() },
                    { "type", Type + " (0x" + Type.ToString("x") + ")" },
                    { "VlanIdentifier", VlanIdentifier + " (0x" + VlanIdentifier.ToString("x") + ")" }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

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