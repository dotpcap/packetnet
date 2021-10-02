/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet
{
    /// <summary>
    /// See http://en.wikipedia.org/wiki/Ethernet#Ethernet_frame_types_and_the_EtherType_field
    /// </summary>
    public sealed class EthernetPacket : InternetLinkLayerPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket
        (
            PhysicalAddress sourceHardwareAddress,
            PhysicalAddress destinationHardwareAddress,
            EthernetType ethernetType)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = EthernetFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            // set the instance values
            SourceHardwareAddress = sourceHardwareAddress;
            DestinationHardwareAddress = destinationHardwareAddress;
            Type = ethernetType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public EthernetPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(byteArraySegment);
            Header.Length = EthernetFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => ParseNextSegment(Header, Type));
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        /// <summary>MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress DestinationHardwareAddress
        {
            get
            {
                var hwAddress = new byte[EthernetFields.MacAddressLength];

                var start = Header.Offset + EthernetFields.DestinationMacPosition;

                Unsafe.WriteUnaligned(ref hwAddress[0], Unsafe.As<byte, int>(ref Header.Bytes[start]));
                Unsafe.WriteUnaligned(ref hwAddress[4], Unsafe.As<byte, short>(ref Header.Bytes[start + 4]));

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);
                
                var start = Header.Offset + EthernetFields.DestinationMacPosition;
                Unsafe.WriteUnaligned(ref Header.Bytes[start], Unsafe.As<byte, int>(ref hwAddress[0]));
                Unsafe.WriteUnaligned(ref Header.Bytes[start + 4], Unsafe.As<byte, short>(ref hwAddress[4]));
            }
        }

        /// <summary>
        /// Payload packet, overridden to set the 'Type' field based on
        /// the type of packet being used here if the PayloadPacket is being set
        /// </summary>
        public override Packet PayloadPacket
        {
            get => base.PayloadPacket;
            set
            {
                base.PayloadPacket = value;

                // set Type based on the type of the payload
                // NOTE: new types should be inserted here

                Type = value switch
                {
                    IPv4Packet _ => EthernetType.IPv4,
                    IPv6Packet _ => EthernetType.IPv6,
                    ArpPacket _ => EthernetType.Arp,
                    LldpPacket _ => EthernetType.Lldp,
                    PppoePacket _ => EthernetType.PppoeSessionStage,
                    _ => Type
                };
            }
        }
        
        /// <summary>MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress SourceHardwareAddress
        {
            get 
            {
                var hwAddress = new byte[EthernetFields.MacAddressLength];
                var start = Header.Offset + EthernetFields.SourceMacPosition;

                Unsafe.WriteUnaligned(ref hwAddress[0], Unsafe.As<byte, int>(ref Header.Bytes[start]));
                Unsafe.WriteUnaligned(ref hwAddress[4], Unsafe.As<byte, short>(ref Header.Bytes[start + 4]));

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                var start = Header.Offset + EthernetFields.SourceMacPosition;
                Unsafe.WriteUnaligned(ref Header.Bytes[start], Unsafe.As<byte, int>(ref hwAddress[0]));
                Unsafe.WriteUnaligned(ref Header.Bytes[start + 4], Unsafe.As<byte, short>(ref hwAddress[4]));
            }
        }

        /// <summary>
        /// Type of packet that this ethernet packet encapsulates.
        /// </summary>
        public EthernetType Type
        {
            get => (EthernetType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + EthernetFields.TypePosition);
            set
            {
                var val = (short) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + EthernetFields.TypePosition);
            }
        }

        /// <summary>
        /// Used by the EthernetPacket constructor. Located here because the LinuxSll constructor
        /// also needs to perform the same operations as it contains an ethernet type
        /// </summary>
        /// <param name="header">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="type">
        /// A <see cref="EthernetType" />
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment" />
        /// </returns>
        internal static PacketOrByteArraySegment ParseNextSegment
        (
            ByteArraySegment header,
            EthernetType type)
        {
            // slice off the payload
            var payload = header.NextSegment();
            Log.DebugFormat("payload {0}", payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // parse the encapsulated bytes
            switch (type)
            {
                case EthernetType.IPv4:
                {
                    payloadPacketOrData.Packet = new IPv4Packet(payload);
                    break;
                }
                case EthernetType.IPv6:
                {
                    payloadPacketOrData.Packet = new IPv6Packet(payload);
                    break;
                }
                case EthernetType.Arp:
                {
                    payloadPacketOrData.Packet = new ArpPacket(payload);
                    break;
                }
                case EthernetType.Lldp:
                {
                    payloadPacketOrData.Packet = new LldpPacket(payload);
                    break;
                }
                case EthernetType.PppoeSessionStage:
                {
                    payloadPacketOrData.Packet = new PppoePacket(payload);
                    break;
                }
                case EthernetType.WakeOnLan:
                {
                    payloadPacketOrData.Packet = new WakeOnLanPacket(payload);
                    break;
                }
                case EthernetType.VLanTaggedFrame:
                {
                    payloadPacketOrData.Packet = new Ieee8021QPacket(payload);
                    break;
                }
                default: // consider the sub-packet to be a byte array
                {
                    payloadPacketOrData.ByteArraySegment = payload;
                    break;
                }
            }

            return payloadPacketOrData;
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

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    // build the output string
                    buffer.AppendFormat("{0}[EthernetPacket: SourceHardwareAddress={2}, DestinationHardwareAddress={3}, Type={4}]{1}",
                                        color,
                                        colorEscape,
                                        HexPrinter.PrintMACAddress(SourceHardwareAddress),
                                        HexPrinter.PrintMACAddress(DestinationHardwareAddress),
                                        Type.ToString());

                    break;
                }
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                {
                    // collect the properties and their value
                    var properties = new Dictionary<string, string>
                    {
                        { "destination", HexPrinter.PrintMACAddress(DestinationHardwareAddress) },
                        { "source", HexPrinter.PrintMACAddress(SourceHardwareAddress) },
                        { "type", Type + " (0x" + Type.ToString("x") + ")" }
                    };

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("Eth:  ******* Ethernet - \"Ethernet\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("Eth:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("Eth: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("Eth:");
                    break;
                }
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random EthernetPacket
        /// TODO: could improve this routine to set a random payload as well
        /// </summary>
        /// <returns>
        /// A <see cref="EthernetPacket" />
        /// </returns>
        public static EthernetPacket RandomPacket()
        {
            var rnd = new Random();

            var srcPhysicalAddress = new byte[EthernetFields.MacAddressLength];
            var dstPhysicalAddress = new byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(srcPhysicalAddress);
            rnd.NextBytes(dstPhysicalAddress);

            return new EthernetPacket(new PhysicalAddress(srcPhysicalAddress),
                                      new PhysicalAddress(dstPhysicalAddress),
                                      EthernetType.None);
        }
    }
}
