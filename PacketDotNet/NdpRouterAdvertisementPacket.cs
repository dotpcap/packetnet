﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Ndp;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet
{
    public class NdpRouterAdvertisementPacket : NdpPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NdpRouterAdvertisementPacket"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public NdpRouterAdvertisementPacket(ByteArraySegment header) : base(header)
        { }

        /// <summary>
        /// Gets or sets the current hop limit.
        /// </summary>
        public byte CurrentHopLimit
        {
            get => Header.Bytes[Header.Offset + NdpFields.RouterAdvertisementCurrentHopLimitOffset];
            set => Header.Bytes[Header.Offset + NdpFields.RouterAdvertisementCurrentHopLimitOffset] = value;
        }

        /// <summary>
        /// Gets or sets the managed address configuration flag.
        /// </summary>
        public bool ManagedAddressConfiguration
        {
            get => (Header[NdpFields.RouterAdvertisementExtOffset] & 0b_1000_0000) != 0;
            set
            {
                if (value)
                    Header[NdpFields.RouterAdvertisementExtOffset] |= 0b_1000_0000;
                else
                    Header[NdpFields.RouterAdvertisementExtOffset] = (byte)(Header[NdpFields.RouterAdvertisementExtOffset] & ~0b_1000_0000);
            }
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public List<NdpOption> OptionsCollection
        {
            get => ParseOptions(OptionsSegment);
            set
            {
                var optionsOffset = NdpFields.RouterAdvertisementOptionsOffset;

                foreach (var option in value)
                {
                    var optionBytes = option.Bytes;
                    Array.Copy(optionBytes, 0, Header.Bytes, Header.Offset + optionsOffset, optionBytes.Length);
                    optionsOffset += optionBytes.Length;
                }
            }
        }

        /// <summary>
        /// Gets the options as a <see cref="ByteArraySegment" />.
        /// </summary>
        public ByteArraySegment OptionsSegment
        {
            get
            {
                var optionsOffset = NdpFields.RouterAdvertisementOptionsOffset;
                var optionsLength = Header.Length - optionsOffset;

                return new ByteArraySegment(Header.Bytes, Header.Offset + optionsOffset, optionsLength);
            }
        }

        /// <summary>
        /// Gets or sets the other configuration flag.
        /// </summary>
        public bool OtherConfiguration
        {
            get => (Header[NdpFields.RouterAdvertisementExtOffset] & 0b_0100_0000) != 0;
            set
            {
                if (value)
                    Header[NdpFields.RouterAdvertisementExtOffset] |= 0b_0100_0000;
                else
                    Header[NdpFields.RouterAdvertisementExtOffset] = (byte)(Header[NdpFields.RouterAdvertisementExtOffset] & ~0b_0100_0000);
            }
        }

        /// <summary>
        /// Gets or sets the reachable time.
        /// </summary>
        public uint ReachableTime
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + NdpFields.RouterAdvertisementReachableTimeOffset);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + NdpFields.RouterAdvertisementReachableTimeOffset);
        }

        /// <summary>
        /// Gets or sets the retransmit timer.
        /// </summary>
        public uint RetransmitTimer
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + NdpFields.RouterAdvertisementRetransmitTimerOffset);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + NdpFields.RouterAdvertisementRetransmitTimerOffset);
        }

        /// <summary>
        /// Gets or sets the router lifetime.
        /// </summary>
        public ushort RouterLifetime
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + NdpFields.RouterAdvertisementRouterLifetimeOffset);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + NdpFields.RouterAdvertisementRouterLifetimeOffset);
        }

        /// <inheritdoc />
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

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[NdpRouterAdvertisementPacket: ReachableTime={2}, RetransmitTimer={3}, RouterLifetime={4}, ManagedAddressConfiguration={5}, OtherConfiguration={6}]{1}",
                                        color,
                                        colorEscape,
                                        ReachableTime,
                                        RetransmitTimer,
                                        RouterLifetime,
                                        ManagedAddressConfiguration,
                                        OtherConfiguration);

                    break;

                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    var properties = new Dictionary<string, string>
                    {
                        { "reachableTime", ReachableTime.ToString() },
                        { "retransmitTimer", RetransmitTimer.ToString() },
                        { "routerLifetime", RouterLifetime.ToString() },
                        { "managedAddressConfiguration", ManagedAddressConfiguration.ToString() },
                        { "otherConfiguration", OtherConfiguration.ToString() },
                    };

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("NDP:  ******* NDP - \"Router Advertisement\"- offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("NDP:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("NDP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("NDP:");
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}