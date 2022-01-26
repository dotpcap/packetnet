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
    /// An IGMP v3 membership query packet.
    /// </summary>
    public sealed class IgmpV3MembershipQueryPacket : IgmpPacket
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IgmpV3MembershipQueryPacket(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = IgmpV3MembershipQueryFields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() =>
            {
                var result = new PacketOrByteArraySegment { ByteArraySegment = Header.NextSegment() };
                return result;
            });
        }

        /// <summary>
        /// Constructor with parent
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IgmpV3MembershipQueryPacket
        (
            ByteArraySegment byteArraySegment,
            Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Fetch the IGMPv3 membership query header checksum.</summary>
        public short Checksum
        {
            get => BitConverter.ToInt16(Header.Bytes,
                                        Header.Offset + IgmpV3MembershipQueryFields.ChecksumPosition);
            set
            {
                var v = BitConverter.GetBytes(value);
                Array.Copy(v, 0, Header.Bytes, Header.Offset + IgmpV3MembershipQueryFields.ChecksumPosition, 2);
            }
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Brown;

        /// <summary>Fetch the IGMPv3 membership query group address.</summary>
        public IPAddress GroupAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                               Header.Offset + IgmpV3MembershipQueryFields.GroupAddressPosition,
                                                               Header.Bytes);

        /// <summary>Fetch the IGMPv3 membership query max response time, in tenths of seconds.</summary>
        public byte MaxResponseTime
        {
            get
            {
                return CodeOrFloatingPointValue(MaxResponseCode);
            }
        }

        /// <summary>
        /// Fetch the IGMPv3 membership query number of sources.
        /// </summary>
        public ushort NumberOfSources
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                  Header.Offset + IgmpV3MembershipQueryFields.NumberOfSourcesPosition);
            set
            {
                var v = value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + IgmpV3MembershipQueryFields.NumberOfSourcesPosition);
            }
        }

        /// <summary>
        /// Fetch theIGMPv3 membership query querier's query interval, in seconds.
        /// </summary>
        public byte QueriersQueryInterval
        {
            get
            {
                return CodeOrFloatingPointValue(QueriersQueryIntervalCode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating IGMPv3 membership query querier's robustness variable.
        /// </summary>
        public byte QueriersRobustnessVariable
        {
            get => (byte)(ReservedSFlagAndQRV & 0x07);
            set
            {
                // read the original value
                byte field = ReservedSFlagAndQRV;

                // mask in the new field
                field = (byte)((field & 0xF8) | value & 0x07);

                // write the updated value back
                ReservedSFlagAndQRV = field;
            }
        }

        /// <summary>
        /// List of IGMPv3 membership query IP unicast source addresses.
        /// </summary>
        public List<IPAddress> SourceAddresses
        {
            get
            {
                List<IPAddress> sourceAddresses = new List<IPAddress>();
                var offset = Header.Offset + IgmpV3MembershipQueryFields.SourceAddressStart;

                for (int i = 0; i < NumberOfSources; i++)
                {
                    sourceAddresses.Add(IPPacket.GetIPAddress(AddressFamily.InterNetwork, offset, Header.Bytes));
                    offset += 4;
                }

                return sourceAddresses;
            }
            set
            {
                if (value.Count > NumberOfSources)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                var offset = Header.Offset + IgmpV3MembershipQueryFields.SourceAddressStart;

                foreach (IPAddress ipAddress in value)
                {
                    // check that the address family is ipv4
                    if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
                        ThrowHelper.ThrowInvalidAddressFamilyException(ipAddress.AddressFamily);

                    var address = ipAddress.GetAddressBytes();
                    Array.Copy(address,
                               0,
                               Header.Bytes,
                               offset,
                               address.Length);
                    offset += address.Length;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating IGMPv3 membership query suppress rounter-side processing flag.
        /// </summary>
        public bool SuppressRouterSideProcessingFlag
        {
            get => (ReservedSFlagAndQRV & 0x08) != 0;
            set
            {
                // read the original value
                byte field = ReservedSFlagAndQRV;

                // mask in the new field
                field = (byte)((field & 0xF7) | (value ? 1 : 0) & 0x08);

                // write the updated value back
                ReservedSFlagAndQRV = field;
            }
        }

        /// <value>
        /// The type of IGMP message
        /// </value>
        public override IgmpMessageType Type
        {
            get => (IgmpMessageType)Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.TypePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.TypePosition] = (byte)value;
        }

        private byte CodeOrFloatingPointValue(byte code)
        {
            if (code < 128)
            {
                return code;
            }
            else    // MaxResponseCode >= 128, calculate floating point value.
            {
                int exp = (code & 0x70) >> 4;
                int mant = code & 0x0F;

                return (byte)((mant | 0x10) << (exp + 3));
            }
        }

        /// <summary>Fetch the IGMPv3 membership query max response code.</summary>
        private byte MaxResponseCode
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.MaxResponseCodePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.MaxResponseCodePosition] = value;
        }

        /// <summary>
        /// Fetch the IGMPv3 membership query querier's query interval code.
        /// </summary>
        private byte QueriersQueryIntervalCode
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.QueriersQueryIntervalCodePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.QueriersQueryIntervalCodePosition] = value;
        }

        /// <summary>
        /// IGMPv3 membership query Reserved, Suppress Router-Side Processing Flag, and Querier's Roubustness Variable.
        /// </summary>
        private byte ReservedSFlagAndQRV
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.ReservedSFlagAndQRVPosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.ReservedSFlagAndQRVPosition] = value;
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
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
                buffer.AppendFormat("{0}[IgmpV3Packet (Membership Query): Type={2}, MaxResponseTime={3}, GroupAddress={4}, SFlag={5}, QRV={6}, QQI={7}, NumberOfSources={8}]{1}",
                                    color,
                                    colorEscape,
                                    Type,
                                    $"{MaxResponseTime / 10:0.0}",
                                    GroupAddress,
                                    SuppressRouterSideProcessingFlag,
                                    QueriersRobustnessVariable,
                                    QueriersQueryInterval,
                                    NumberOfSources);
            }

            if (outputFormat is StringOutputType.Verbose or StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "type", Type + " (0x" + Type.ToString("x") + ")" },
                    { "max response time", $"{MaxResponseTime / 10:0.0}" + " sec (0x" + MaxResponseTime.ToString("x") + ")" },
                    // TODO: Implement checksum validation for IGMPv3
                    { "header checksum", "0x" + Checksum.ToString("x") },
                    { "group address", GroupAddress.ToString() },
                    { "suppress router-side processing", SuppressRouterSideProcessingFlag.ToString() },
                    { "querier's robustness variable", QueriersRobustnessVariable.ToString() },
                    { "querier's query interval (tenths of seconds)", QueriersQueryInterval.ToString() },
                    { "number of sources", NumberOfSources.ToString() }
                };

                foreach (IPAddress ip in SourceAddresses)
                {
                    properties.Add("source address", ip.ToString());
                }

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("IGMP:  ******* IGMPv3 - \"Internet Group Management Protocol (Version 3) Membership Query\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("IGMP:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("IGMP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}
