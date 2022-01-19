/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet
{
    /// <summary>
    /// IGMP v3 Membership Report Group Record.
    /// </summary>
    public class IgmpV3MembershipReportGroupRecord
    {
        /// <summary>
        /// ByteArraySegment representing the header passed in to the Construtor.
        /// </summary>
        private ByteArraySegment header;

        /// <summary>
        /// recordOffset will be set in the constructor to be used for this instance of the record.
        /// </summary>
        private int recordOffset;

        /// <summary>
        /// Constructor that takes an offset for this instance.
        /// </summary>
        /// <param name="offset"></param>
        public IgmpV3MembershipReportGroupRecord(ByteArraySegment header, int offset)
        {
            this.header = header;
            this.recordOffset = offset;
        }

        /// <value>
        /// Fetch the IGMP membership report group record auxiliary data length. 
        /// </value>
        public byte AuxiliaryDataLength
        {
            get => this.header.Bytes[this.recordOffset + IgmpV3MembershipReportGroupRecordFields.AuxiliaryDataLengthPosition];
            set => this.header.Bytes[this.recordOffset + IgmpV3MembershipReportGroupRecordFields.AuxiliaryDataLengthPosition] = value;
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public string Color => AnsiEscapeSequences.Brown;

        /// <summary>Fetch the IGMP membership report group record multicast address.</summary>
        public IPAddress MulticastAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                               this.recordOffset + IgmpV3MembershipReportGroupRecordFields.MulticastAddressPosition,
                                                               this.header.Bytes);

        /// <summary>
        /// Fetch the membership report group record number of sources.
        /// </summary>
        public ushort NumberOfSources
        {
            get => EndianBitConverter.Big.ToUInt16(this.header.Bytes,
                                                 this.recordOffset + IgmpV3MembershipReportGroupRecordFields.NumberOfSourcesPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 this.header.Bytes,
                                                 this.recordOffset + IgmpV3MembershipReportGroupRecordFields.NumberOfSourcesPosition);
            }
        }

        /// <value>
        /// Fetch the IGMP membership report group record type. 
        /// </value>
        public IgmpV3MembershipReportGroupRecordType RecordType
        {
            get => (IgmpV3MembershipReportGroupRecordType)this.header.Bytes[this.recordOffset + IgmpV3MembershipReportGroupRecordFields.RecordTypePosition];
            set => this.header.Bytes[this.recordOffset + IgmpV3MembershipReportGroupRecordFields.RecordTypePosition] = (byte)value;
        }

        private List<IPAddress> sourceAddresses = null;

        /// <summary>
        /// List of IGMPv3 membership report group record IP unicast source addresses.
        /// </summary>
        public List<IPAddress> SourceAddresses
        {
            get
            {
                if (this.sourceAddresses == null)
                {
                    this.sourceAddresses = new List<IPAddress>();
                    var offset = this.recordOffset + IgmpV3MembershipReportGroupRecordFields.SourceAddressStart;

                    for (int i = 0; i < NumberOfSources; i++)
                    {
                        long address = EndianBitConverter.Little.ToUInt32(this.header.Bytes, offset);
                        this.sourceAddresses.Add(new IPAddress(address));
                        offset += 4;
                    }
                }

                return this.sourceAddresses;
            }
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if (outputFormat is StringOutputType.Colored or StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat is StringOutputType.Normal or StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[IgmpV3 Membership Report Group Record: RecordType={2}, AuxDataLen={3}, NumberOfSources={4}, MulticastAddress={5}]{1}",
                                    color,
                                    colorEscape,
                                    RecordType,
                                    AuxiliaryDataLength,
                                    NumberOfSources,
                                    MulticastAddress);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "record type", RecordType + " (0x" + RecordType.ToString("x") + ")" },
                    { "auxiliary data length", AuxiliaryDataLength.ToString() },
                    { "number of sources", NumberOfSources.ToString() },
                    { "multicast address", MulticastAddress.ToString() },
                };

                foreach (IPAddress ip in SourceAddresses)
                {
                    properties.Add("source address", ip.ToString());
                }

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("IGMP:");
                buffer.AppendLine("IGMP:  ******* \"Group Record\" ");
                foreach (var property in properties)
                {
                    buffer.AppendLine("IGMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            return buffer.ToString();
        }
    }
}
