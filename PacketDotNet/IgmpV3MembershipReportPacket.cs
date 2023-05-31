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

namespace PacketDotNet;

    /// <summary>
    /// An IGMP v3 membership report packet.
    /// </summary>
    public sealed class IgmpV3MembershipReportPacket : IgmpPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV3MembershipReportPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public IgmpV3MembershipReportPacket(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = IgmpV3MembershipReportFields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment { ByteArraySegment = Header.NextSegment() });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV3MembershipReportPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="parentPacket">The parent packet.</param>
        public IgmpV3MembershipReportPacket(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Gets or sets the IGMPv3 membership report header checksum.</summary>
        public short Checksum
        {
            get => BitConverter.ToInt16(Header.Bytes,
                                        Header.Offset + IgmpV3MembershipReportFields.ChecksumPosition);
            set
            {
                var v = BitConverter.GetBytes(value);
                Array.Copy(v, 0, Header.Bytes, Header.Offset + IgmpV3MembershipReportFields.ChecksumPosition, 2);
            }
        }

        /// <summary>Gets or sets ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Brown;

        /// <summary>
        /// Gets or sets a list of IGMPv3 membership query group records.
        /// </summary>
        public List<IgmpV3MembershipReportGroupRecord> GroupRecords
        {
            get
            {
                List<IgmpV3MembershipReportGroupRecord> groupRecords = new List<IgmpV3MembershipReportGroupRecord>();
                var offset = Header.Offset + IgmpV3MembershipReportFields.GroupRecordStart;

                for (int i = 0; i < NumberOfGroupRecords; i++)
                {
                    IgmpV3MembershipReportGroupRecord groupRecord = new IgmpV3MembershipReportGroupRecord(Header, offset);
                    groupRecords.Add(groupRecord);
                    offset += (IgmpV3MembershipReportGroupRecordFields.IgmpV3MembershipReportGroupRecordHeaderLength + groupRecord.NumberOfSources * IPv4Fields.AddressLength);
                }

                return groupRecords;
            }
            set
            {
                if (value.Count > NumberOfGroupRecords)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                var offset = Header.Offset + IgmpV3MembershipReportFields.GroupRecordStart;

                foreach (IgmpV3MembershipReportGroupRecord groupRecord in value)
                {
                    // Record type
                    Header.Bytes[offset + IgmpV3MembershipReportGroupRecordFields.RecordTypePosition] = (byte) groupRecord.RecordType;

                    // Auxiliary data length
                    Header.Bytes[offset + IgmpV3MembershipReportGroupRecordFields.AuxiliaryDataLengthPosition] = groupRecord.AuxiliaryDataLength;

                    // Number of sources
                    var v = groupRecord.NumberOfSources;
                    EndianBitConverter.Big.CopyBytes(v,
                                                     Header.Bytes,
                                                     offset + IgmpV3MembershipReportGroupRecordFields.NumberOfSourcesPosition);

                    // Multicast address
                    // check that the address family is ipv4
                    if (groupRecord.MulticastAddress.AddressFamily != AddressFamily.InterNetwork)
                        ThrowHelper.ThrowInvalidAddressFamilyException(groupRecord.MulticastAddress.AddressFamily);

                    var address = groupRecord.MulticastAddress.GetAddressBytes();
                    Array.Copy(address,
                               0,
                               Header.Bytes,
                               offset + IgmpV3MembershipReportGroupRecordFields.MulticastAddressPosition,
                               address.Length);

                    // Source addresses
                    var addressOffset = 0;
                    foreach (IPAddress sourceAddress in groupRecord.SourceAddresses)
                    {
                        // check that the address family is ipv4
                        if (sourceAddress.AddressFamily != AddressFamily.InterNetwork)
                            ThrowHelper.ThrowInvalidAddressFamilyException(sourceAddress.AddressFamily);

                        address = sourceAddress.GetAddressBytes();
                        Array.Copy(address,
                                   0,
                                   Header.Bytes,
                                   offset + IgmpV3MembershipReportGroupRecordFields.SourceAddressStart + addressOffset,
                                   address.Length);

                        addressOffset += address.Length;
                    }

                    offset += (IgmpV3MembershipReportGroupRecordFields.IgmpV3MembershipReportGroupRecordHeaderLength + groupRecord.NumberOfSources * IPv4Fields.AddressLength);
                }
            }
        }

        /// <summary>
        /// Gets or sets the membership report number of group records.
        /// </summary>
        public ushort NumberOfGroupRecords
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IgmpV3MembershipReportFields.NumberOfGroupRecordsPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + IgmpV3MembershipReportFields.NumberOfGroupRecordsPosition);
            }
        }

        /// <summary>
        /// Gets or sets the type of IGMP message.
        /// </summary>
        public override IgmpMessageType Type
        {
            get => (IgmpMessageType) Header.Bytes[Header.Offset + IgmpV3MembershipReportFields.TypePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipReportFields.TypePosition] = (byte) value;
        }

        /// <inheritdoc cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
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
                buffer.AppendFormat("{0}[IgmpV3Packet (Membership Report): Type={2}, NumberOfGroupRecords={3}]{1}",
                                    color,
                                    colorEscape,
                                    Type,
                                    NumberOfGroupRecords);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "type", Type + " (0x" + Type.ToString("x") + ")" },
                    // TODO: Implement checksum validation for IGMPv3
                    { "header checksum", "0x" + Checksum.ToString("x") },
                    { "number of group records", NumberOfGroupRecords.ToString() }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.Append("IGMP:  ******* IGMPv3 - \"Internet Group Management Protocol (Version 3) Membership Report\" - offset=? length=").Append(TotalPacketLength).AppendLine();
                buffer.AppendLine("IGMP:");
                foreach (var property in properties)
                {
                    buffer.Append("IGMP: ").Append(property.Key.PadLeft(padLength)).Append(" = ").AppendLine(property.Value);
                }

                foreach (IgmpV3MembershipReportGroupRecord group in GroupRecords)
                {
                    buffer.Append(group.ToString(outputFormat));
                }

                buffer.AppendLine("IGMP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }