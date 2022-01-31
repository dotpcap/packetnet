/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
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
        private readonly ByteArraySegment _header;
        private readonly int _recordOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV3MembershipReportGroupRecord" /> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="offset">The offset.</param>
        public IgmpV3MembershipReportGroupRecord(ByteArraySegment header, int offset)
        {
            _header = header;
            _recordOffset = offset;
        }

        /// <summary>
        /// Gets or sets the IGMP membership report group record auxiliary data length.
        /// </summary>
        public byte AuxiliaryDataLength
        {
            get => _header.Bytes[_recordOffset + IgmpV3MembershipReportGroupRecordFields.AuxiliaryDataLengthPosition];
            set => _header.Bytes[_recordOffset + IgmpV3MembershipReportGroupRecordFields.AuxiliaryDataLengthPosition] = value;
        }

        /// <summary>Gets or sets ascii escape sequence of the color associated with this packet type.</summary>
        public string Color => AnsiEscapeSequences.Brown;

        /// <summary>Gets or sets the IGMP membership report group record multicast address.</summary>
        public IPAddress MulticastAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                                   _recordOffset + IgmpV3MembershipReportGroupRecordFields.MulticastAddressPosition,
                                                                   _header.Bytes);

        /// <summary>
        /// Gets or sets the membership report group record number of sources.
        /// </summary>
        public ushort NumberOfSources
        {
            get => EndianBitConverter.Big.ToUInt16(_header.Bytes,
                                                   _recordOffset + IgmpV3MembershipReportGroupRecordFields.NumberOfSourcesPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 _header.Bytes,
                                                 _recordOffset + IgmpV3MembershipReportGroupRecordFields.NumberOfSourcesPosition);
            }
        }

        /// <summary>
        /// Gets or sets the IGMP membership report group record type.
        /// </summary>
        public IgmpV3MembershipReportGroupRecordType RecordType
        {
            get => (IgmpV3MembershipReportGroupRecordType) _header.Bytes[_recordOffset + IgmpV3MembershipReportGroupRecordFields.RecordTypePosition];
            set => _header.Bytes[_recordOffset + IgmpV3MembershipReportGroupRecordFields.RecordTypePosition] = (byte) value;
        }

        /// <summary>
        /// Gets or sets a list of IGMPv3 membership report group record IP unicast source addresses.
        /// </summary>
        public List<IPAddress> SourceAddresses
        {
            get
            {
                List<IPAddress> sourceAddresses = new List<IPAddress>();
                var offset = _recordOffset + IgmpV3MembershipReportGroupRecordFields.SourceAddressStart;

                for (int i = 0; i < NumberOfSources; i++)
                {
                    sourceAddresses.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, _header.Bytes));
                    offset += 4;
                }

                return sourceAddresses;
            }
            set
            {
                if (value.Count > NumberOfSources)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                var offset = _recordOffset + IgmpV3MembershipReportGroupRecordFields.SourceAddressStart;

                foreach (IPAddress ipAddress in value)
                {
                    // check that the address family is ipv4
                    if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
                        ThrowHelper.ThrowInvalidAddressFamilyException(ipAddress.AddressFamily);

                    var address = ipAddress.GetAddressBytes();
                    Array.Copy(address,
                               0,
                               _header.Bytes,
                               offset,
                               address.Length);

                    offset += address.Length;
                }
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
                    buffer.Append("IGMP: ").Append(property.Key.PadLeft(padLength)).Append(" = ").AppendLine(property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            return buffer.ToString();
        }
    }
}