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
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

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
                        Type = EthernetPacketType.IPv4;
                        break;
                    case IPv6Packet _:
                        Type = EthernetPacketType.IPv6;
                        break;
                    case ARPPacket _:
                        Type = EthernetPacketType.Arp;
                        break;
                    case LLDPPacket _:
                        Type = EthernetPacketType.LLDP;
                        break;
                    // NOTE: new types should be inserted here
                    case PPPoEPacket _:
                        Type = EthernetPacketType.PointToPointProtocolOverEthernetSessionStage;
                        break;
                    default:
                        Type = EthernetPacketType.None;
                        break;
                }
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress SourceHwAddress
        {
            get
            {
                var hwAddress = new Byte[EthernetFields.MacAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                var hwAddress = value.GetAddressBytes();
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("address length " + hwAddress.Length + " not equal to the expected length of " + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress.Length);
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public PhysicalAddress DestinationHwAddress
        {
            get
            {
                var hwAddress = new Byte[EthernetFields.MacAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                var hwAddress = value.GetAddressBytes();
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("address length " + hwAddress.Length + " not equal to the expected length of " + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Type of packet that this ethernet packet encapsulates
        /// </value>
        public EthernetPacketType Type
        {
            get => (EthernetPacketType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                       Header.Offset + EthernetFields.TypePosition);

            set
            {
                var val = (Int16) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + EthernetFields.TypePosition);
            }
        }

        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket
        (
            PhysicalAddress sourceHwAddress,
            PhysicalAddress destinationHwAddress,
            EthernetPacketType ethernetPacketType)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = EthernetFields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            SourceHwAddress = sourceHwAddress;
            DestinationHwAddress = destinationHwAddress;
            Type = ethernetPacketType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public EthernetPacket(ByteArraySegment bas)
        {
            Log.Debug("");

            // slice off the header portion
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = EthernetFields.HeaderLength;

            // parse the encapsulated bytes
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseEncapsulatedBytes(Header, Type));
        }

        /// <summary>
        /// Used by the EthernetPacket constructor. Located here because the LinuxSLL constructor
        /// also needs to perform the same operations as it contains an ethernet type
        /// </summary>
        /// <param name="header">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="type">
        /// A <see cref="EthernetPacketType" />
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment" />
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes
        (
            ByteArraySegment header,
            EthernetPacketType type)
        {
            // slice off the payload
            var payload = header.EncapsulatedBytes();
            Log.DebugFormat("payload {0}", payload);

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // parse the encapsulated bytes
            switch (type)
            {
                case EthernetPacketType.IPv4:
                    payloadPacketOrData.Packet = new IPv4Packet(payload);
                    break;
                case EthernetPacketType.IPv6:
                    payloadPacketOrData.Packet = new IPv6Packet(payload);
                    break;
                case EthernetPacketType.Arp:
                    payloadPacketOrData.Packet = new ARPPacket(payload);
                    break;
                case EthernetPacketType.LLDP:
                    payloadPacketOrData.Packet = new LLDPPacket(payload);
                    break;
                case EthernetPacketType.PointToPointProtocolOverEthernetSessionStage:
                    payloadPacketOrData.Packet = new PPPoEPacket(payload);
                    break;
                case EthernetPacketType.WakeOnLan:
                    payloadPacketOrData.Packet = new WakeOnLanPacket(payload);
                    break;
                case EthernetPacketType.VLanTaggedFrame:
                    payloadPacketOrData.Packet = new Ieee8021QPacket(payload);
                    break;
                default: // consider the sub-packet to be a byte array
                    payloadPacketOrData.ByteArraySegment = payload;
                    break;
            }

            return payloadPacketOrData;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.DarkGray;

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
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
                buffer.AppendFormat("{0}[EthernetPacket: SourceHwAddress={2}, DestinationHwAddress={3}, Type={4}]{1}",
                                    color,
                                    colorEscape,
                                    HexPrinter.PrintMACAddress(SourceHwAddress),
                                    HexPrinter.PrintMACAddress(DestinationHwAddress),
                                    Type.ToString());
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"destination", HexPrinter.PrintMACAddress(DestinationHwAddress)},
                    {"source", HexPrinter.PrintMACAddress(SourceHwAddress)},
                    {"type", Type + " (0x" + Type.ToString("x") + ")"}
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("Eth:  ******* Ethernet - \"Ethernet\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("Eth:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("Eth: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("Eth:");
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

            var srcPhysicalAddress = new Byte[EthernetFields.MacAddressLength];
            var dstPhysicalAddress = new Byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(srcPhysicalAddress);
            rnd.NextBytes(dstPhysicalAddress);

            return new EthernetPacket(new PhysicalAddress(srcPhysicalAddress),
                                      new PhysicalAddress(dstPhysicalAddress),
                                      EthernetPacketType.None);
        }
    }
}