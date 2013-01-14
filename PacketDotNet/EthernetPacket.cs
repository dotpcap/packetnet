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
﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// See http://en.wikipedia.org/wiki/Ethernet#Ethernet_frame_types_and_the_EtherType_field
    /// </summary>
    public class EthernetPacket : InternetLinkLayerPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        /// <value>
        /// Payload packet, overridden to set the 'Type' field based on
        /// the type of packet being used here if the PayloadPacket is being set
        /// </value>
        public override Packet PayloadPacket
        {
            get
            {
                return base.PayloadPacket;
            }

            set
            {
                base.PayloadPacket = value;

                // set Type based on the type of the payload
                if(value is IPv4Packet)
                {
                    Type = EthernetPacketType.IpV4;
                } else if(value is IPv6Packet)
                {
                    Type = EthernetPacketType.IpV6;
                } else if(value is ARPPacket)
                {
                    Type = EthernetPacketType.Arp;
                }
                else if(value is LLDPPacket)
                {
                    Type = EthernetPacketType.LLDP;
                }
                else if(value is PPPoEPacket)
                {
                    Type = EthernetPacketType.PointToPointProtocolOverEthernetSessionStage;
                }
                else // NOTE: new types should be inserted here
                {
                    Type = EthernetPacketType.None;
                }
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress SourceHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[EthernetFields.MacAddressLength];
                Array.Copy(header.Bytes, header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress, 0, header.Bytes, header.Offset + EthernetFields.SourceMacPosition,
                           hwAddress.Length);
            }
        }

        /// <summary> MAC address of the host where the packet originated from.</summary>
        public virtual PhysicalAddress DestinationHwAddress
        {
            get
            {
                byte[] hwAddress = new byte[EthernetFields.MacAddressLength];
                Array.Copy(header.Bytes, header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                byte[] hwAddress = value.GetAddressBytes();
                if(hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + EthernetFields.MacAddressLength);
                }

                Array.Copy(hwAddress, 0, header.Bytes, header.Offset + EthernetFields.DestinationMacPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Type of packet that this ethernet packet encapsulates
        /// </value>
        public virtual EthernetPacketType Type
        {
            get
            {
                return (EthernetPacketType)EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                          header.Offset + EthernetFields.TypePosition);
            }

            set
            {
                Int16 val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + EthernetFields.TypePosition);
            }
        }

        /// <summary>
        /// Construct a new ethernet packet from source and destination mac addresses
        /// </summary>
        public EthernetPacket(PhysicalAddress SourceHwAddress,
                              PhysicalAddress DestinationHwAddress,
                              EthernetPacketType ethernetPacketType)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = EthernetFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set the instance values
            this.SourceHwAddress = SourceHwAddress;
            this.DestinationHwAddress = DestinationHwAddress;
            this.Type = ethernetPacketType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public EthernetPacket(ByteArraySegment bas)
        {
            log.Debug("");

            // slice off the header portion
            header = new ByteArraySegment(bas);
            header.Length = EthernetFields.HeaderLength;

            // parse the encapsulated bytes
            payloadPacketOrData = ParseEncapsulatedBytes(header, Type);
        }

        /// <summary>
        /// Used by the EthernetPacket constructor. Located here because the LinuxSLL constructor
        /// also needs to perform the same operations as it contains an ethernet type
        /// </summary>
        /// <param name="Header">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="Type">
        /// A <see cref="EthernetPacketType"/>
        /// </param>
        /// <returns>
        /// A <see cref="PacketOrByteArraySegment"/>
        /// </returns>
        internal static PacketOrByteArraySegment ParseEncapsulatedBytes(ByteArraySegment Header,
                                                                        EthernetPacketType Type)
        {
            // slice off the payload
            var payload = Header.EncapsulatedBytes();
            log.DebugFormat("payload {0}", payload.ToString());

            var payloadPacketOrData = new PacketOrByteArraySegment();

            // parse the encapsulated bytes
            switch(Type)
            {
            case EthernetPacketType.IpV4:
                payloadPacketOrData.ThePacket = new IPv4Packet(payload);
                break;
            case EthernetPacketType.IpV6:
                payloadPacketOrData.ThePacket = new IPv6Packet(payload);
                break;
            case EthernetPacketType.Arp:
                payloadPacketOrData.ThePacket = new ARPPacket(payload);
                break;
            case EthernetPacketType.LLDP:
                payloadPacketOrData.ThePacket = new LLDPPacket(payload);
                break;
            case EthernetPacketType.PointToPointProtocolOverEthernetSessionStage:
                payloadPacketOrData.ThePacket = new PPPoEPacket(payload);
                break;
            case EthernetPacketType.WakeOnLan:
                payloadPacketOrData.ThePacket = new WakeOnLanPacket(payload);
                break;
            case EthernetPacketType.VLanTaggedFrame:
                payloadPacketOrData.ThePacket = new Ieee8021QPacket(payload);
                break;
            default: // consider the sub-packet to be a byte array
                payloadPacketOrData.TheByteArraySegment = payload;
                break;
            }

            return payloadPacketOrData;
        }

        /// <summary>
        /// Returns the EthernetPacket inside of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="EthernetPacket"/>
        /// </returns>
        [Obsolete("Use Packet.Extract() instead")]
        public static EthernetPacket GetEncapsulated(Packet p)
        {
            log.Debug("");

            if(p is EthernetPacket)
            {
                return (EthernetPacket)p;
            }

            return null;
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override System.String Color
        {
            get
            {
                return AnsiEscapeSequences.DarkGray;
            }
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[EthernetPacket: SourceHwAddress={2}, DestinationHwAddress={3}, Type={4}]{1}",
                    color,
                    colorEscape,
                    HexPrinter.PrintMACAddress(SourceHwAddress),
                    HexPrinter.PrintMACAddress(DestinationHwAddress),
                    Type.ToString());
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("destination", HexPrinter.PrintMACAddress(DestinationHwAddress));
                properties.Add("source", HexPrinter.PrintMACAddress(SourceHwAddress));
                properties.Add("type", Type.ToString() + " (0x" + Type.ToString("x") + ")");

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("Eth:  ******* Ethernet - \"Ethernet\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("Eth:");
                foreach(var property in properties)
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
        /// A <see cref="EthernetPacket"/>
        /// </returns>
        public static EthernetPacket RandomPacket()
        {
            var rnd = new Random();

            byte[] srcPhysicalAddress = new byte[EthernetFields.MacAddressLength];
            byte[] dstPhysicalAddress = new byte[EthernetFields.MacAddressLength];

            rnd.NextBytes(srcPhysicalAddress);
            rnd.NextBytes(dstPhysicalAddress);

            return new EthernetPacket(new PhysicalAddress(srcPhysicalAddress),
                                      new PhysicalAddress(dstPhysicalAddress),
                                      EthernetPacketType.None);
        }
    }
}
