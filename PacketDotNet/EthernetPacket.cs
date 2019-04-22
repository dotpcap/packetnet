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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
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
    [Serializable]
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
            const int offset = 0;
            var length = EthernetFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

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
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseNextSegment(Header, Type), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.DarkGray;

        /// <summary>MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress DestinationHardwareAddress
        {
            get
            {
                var hwAddress = new byte[EthernetFields.MacAddressLength];

                for (var i = 0; i < EthernetFields.MacAddressLength; i++)
                    hwAddress[i] = Header.Bytes[Header.Offset + EthernetFields.DestinationMacPosition + i];

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                for (var i = 0; i < EthernetFields.MacAddressLength; i++)
                    Header.Bytes[Header.Offset + EthernetFields.DestinationMacPosition + i] = hwAddress[i];
            }
        }

        /// <value>
        /// Payload packet, overridden to set the 'Type' field based on
        /// the type of packet being used here if the PayloadPacket is being set
        /// </value>
        public override Packet PayloadPacket
        {
            get => base.PayloadPacket;
            set
            {
                base.PayloadPacket = value;

                switch (value)
                {
                    // set Type based on the type of the payload
                    case IPv4Packet _:
                    {
                        Type = EthernetType.IPv4;
                        break;
                    }
                    case IPv6Packet _:
                    {
                        Type = EthernetType.IPv6;
                        break;
                    }
                    case ArpPacket _:
                    {
                        Type = EthernetType.Arp;
                        break;
                    }
                    case LldpPacket _:
                    {
                        Type = EthernetType.Lldp;
                        break;
                    }
                    // NOTE: new types should be inserted here
                    case PppoePacket _:
                    {
                        Type = EthernetType.PppoeSessionStage;
                        break;
                    }
                    default:
                    {
                        Type = EthernetType.None;
                        break;
                    }
                }
            }
        }

        /// <summary>MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress SourceHardwareAddress
        {
            get
            {
                var hwAddress = new byte[EthernetFields.MacAddressLength];

                for (var i = 0; i < EthernetFields.MacAddressLength; i++)
                    hwAddress[i] = Header.Bytes[Header.Offset + EthernetFields.SourceMacPosition + i];

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                for (var i = 0; i < EthernetFields.MacAddressLength; i++)
                    Header.Bytes[Header.Offset + EthernetFields.SourceMacPosition + i] = hwAddress[i];
            }
        }

        /// <value>
        /// Type of packet that this ethernet packet encapsulates.
        /// </value>
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

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
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