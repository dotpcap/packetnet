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
        /// Initializes a new instance of the <see cref="IgmpV3MembershipQueryPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        public IgmpV3MembershipQueryPacket(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = IgmpV3MembershipQueryFields.HeaderLength;

            // store the payload bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => new PacketOrByteArraySegment { ByteArraySegment = Header.NextSegment() });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IgmpV3MembershipQueryPacket" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="parentPacket">The parent packet.</param>
        public IgmpV3MembershipQueryPacket(ByteArraySegment byteArraySegment, Packet parentPacket) : this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Gets or sets the IGMPv3 membership query header checksum.</summary>
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

        /// <summary>Gets or sets the ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Brown;

        /// <summary>Gets or sets the IGMPv3 membership query group address.</summary>
        public IPAddress GroupAddress => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                                               Header.Offset + IgmpV3MembershipQueryFields.GroupAddressPosition,
                                                               Header.Bytes);

        /// <summary>Gets or sets the IGMPv3 membership query max response time, in tenths of seconds.</summary>
        public ushort MaxResponseTime
        {
            get => CodeOrFloatingPointValue(MaxResponseCode);
            set
            {
                MaxResponseCode = ConvertFloatingPointToCode(value);
            }
        }

        /// <summary>
        /// Gets or sets the IGMPv3 membership query number of sources.
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
        /// Gets or sets theIGMPv3 membership query querier's query interval, in seconds.
        /// </summary>
        public ushort QueriersQueryInterval
        {
            get => CodeOrFloatingPointValue(QueriersQueryIntervalCode);
            set
            {
                QueriersQueryIntervalCode = ConvertFloatingPointToCode(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating IGMPv3 membership query querier's robustness variable.
        /// </summary>
        public byte QueriersRobustnessVariable
        {
            get => (byte) (ReservedSFlagAndQRV & 0x07);
            set
            {
                // read the original value
                byte field = ReservedSFlagAndQRV;

                // mask in the new field
                field = (byte) ((field & 0xF8) | value & 0x07);

                // write the updated value back
                ReservedSFlagAndQRV = field;
            }
        }

        /// <summary>
        /// Gets or sets a list of IGMPv3 membership query IP unicast source addresses.
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
                field = (byte) ((field & 0xF7) | (value ? 1 : 0) & 0x08);

                // write the updated value back
                ReservedSFlagAndQRV = field;
            }
        }

        /// <summary>
        /// Gets or sets the type of IGMP message.
        /// </summary>
        public override IgmpMessageType Type
        {
            get => (IgmpMessageType) Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.TypePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.TypePosition] = (byte) value;
        }

        /// <summary>Gets or sets the IGMPv3 membership query max response code.</summary>
        private byte MaxResponseCode
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.MaxResponseCodePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.MaxResponseCodePosition] = value;
        }

        /// <summary>
        /// Gets or sets the IGMPv3 membership query querier's query interval code.
        /// </summary>
        private byte QueriersQueryIntervalCode
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.QueriersQueryIntervalCodePosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.QueriersQueryIntervalCodePosition] = value;
        }

        /// <summary>
        /// Gets or sets the IGMPv3 membership query Reserved, Suppress Router-Side Processing Flag, and Querier's Roubustness Variable.
        /// </summary>
        private byte ReservedSFlagAndQRV
        {
            get => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.ReservedSFlagAndQRVPosition];
            set => Header.Bytes[Header.Offset + IgmpV3MembershipQueryFields.ReservedSFlagAndQRVPosition] = value;
        }

        private byte CodeOrFloatingPointValue(byte code)
        {
            if (code < 128)
            {
                return code;
            }

            int exp = (code & 0x70) >> 4;
            int mant = code & 0x0F;

            return (byte) ((mant | 0x10) << (exp + 3));
        }

        private int getHighestOneBit(int n)
        {
            // Below steps set bits after
            // MSB (including MSB)

            // Suppose n is 273 (binary
            // is 100010001). It does following
            // 100010001 | 010001000 = 110011001
            n |= n >> 1;

            // This makes sure 4 bits
            // (From MSB and including MSB)
            // are set. It does following
            // 110011001 | 001100110 = 111111111
            n |= n >> 2;

            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;

            // Increment n by 1 so that
            // there is only one set bit
            // which is just before original
            // MSB. n now becomes 1000000000
            n = n + 1;

            // Return original MSB after shifting.
            // n now becomes 100000000
            return (n >> 1);
        }

        private byte ConvertFloatingPointToCode(ushort floatValue)
        {
            byte exp = (byte)(Math.Log(getHighestOneBit(floatValue)) / Math.Log(2) - 7);
            byte mant = (byte)((floatValue >> (exp + 3)) & 0x0f);

            return (byte)(((exp << 4 | mant) ) | 0x80);
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
                buffer.Append("IGMP:  ******* IGMPv3 - \"Internet Group Management Protocol (Version 3) Membership Query\" - offset=? length=").Append(TotalPacketLength).AppendLine();
                buffer.AppendLine("IGMP:");
                foreach (var property in properties)
                {
                    buffer.Append("IGMP: ").Append(property.Key.PadLeft(padLength)).Append(" = ").AppendLine(property.Value);
                }

                buffer.AppendLine("IGMP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}
