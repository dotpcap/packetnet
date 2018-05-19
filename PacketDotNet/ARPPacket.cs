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
 *  Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// An ARP protocol packet.
    /// </summary>
    [Serializable]
    public sealed class ARPPacket : InternetLinkLayerPacket
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
        /// Also known as HardwareType
        /// </value>
        public LinkLayers HardwareAddressType
        {
            get => (LinkLayers) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                Header.Offset + ARPFields.HardwareAddressTypePosition);

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ARPFields.HardwareAddressTypePosition);
            }
        }

        /// <value>
        /// Also known as ProtocolType
        /// </value>
        public EthernetPacketType ProtocolAddressType
        {
            get => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                        Header.Offset + ARPFields.ProtocolAddressTypePosition);

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ARPFields.ProtocolAddressTypePosition);
            }
        }

        /// <value>
        /// Hardware address length field
        /// </value>
        public Int32 HardwareAddressLength
        {
            get => Header.Bytes[Header.Offset + ARPFields.HardwareAddressLengthPosition];

            set => Header.Bytes[Header.Offset + ARPFields.HardwareAddressLengthPosition] = (Byte) value;
        }

        /// <value>
        /// Protocol address length field
        /// </value>
        public Int32 ProtocolAddressLength
        {
            get => Header.Bytes[Header.Offset + ARPFields.ProtocolAddressLengthPosition];

            set => Header.Bytes[Header.Offset + ARPFields.ProtocolAddressLengthPosition] = (Byte) value;
        }

        /// <summary>
        /// Fetch the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        /// <summary>
        /// Sets the operation code.
        /// Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        public ARPOperation Operation
        {
            get => (ARPOperation) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + ARPFields.OperationPosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + ARPFields.OperationPosition);
            }
        }

        /// <value>
        /// Upper layer protocol address of the sender, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public IPAddress SenderProtocolAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + ARPFields.SenderProtocolAddressPosition,
                                         Header.Bytes);

            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Family != IPv4, ARP is used for IPv4, NDP for IPv6");


                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ARPFields.SenderProtocolAddressPosition,
                           address.Length);
            }
        }

        /// <value>
        /// Upper layer protocol address of the target, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public IPAddress TargetProtocolAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + ARPFields.TargetProtocolAddressPosition,
                                         Header.Bytes);

            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Family != IPv4, ARP is used for IPv4, NDP for IPv6");


                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ARPFields.TargetProtocolAddressPosition,
                           address.Length);
            }
        }

        /// <value>
        /// Sender hardware address, usually an ethernet mac address
        /// </value>
        public PhysicalAddress SenderHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                var hwAddress = new Byte[HardwareAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + ARPFields.SenderHardwareAddressPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                var hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("expected physical address length of " + EthernetFields.MacAddressLength + " but it was " + hwAddress.Length);
                }

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + ARPFields.SenderHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Target hardware address, usually an ethernet mac address
        /// </value>
        public PhysicalAddress TargetHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                var hwAddress = new Byte[HardwareAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + ARPFields.TargetHardwareAddressPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("expected physical address length of " + EthernetFields.MacAddressLength + " but it was " + hwAddress.Length);
                }

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + ARPFields.TargetHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.Purple;

        /// <summary>
        /// Create an ARPPacket from values
        /// </summary>
        /// <param name="operation">
        /// A <see cref="ARPOperation" />
        /// </param>
        /// <param name="targetHardwareAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="targetProtocolAddress">
        /// A <see cref="System.Net.IPAddress" />
        /// </param>
        /// <param name="senderHardwareAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="senderProtocolAddress">
        /// A <see cref="System.Net.IPAddress" />
        /// </param>
        public ARPPacket
        (
            ARPOperation operation,
            PhysicalAddress targetHardwareAddress,
            IPAddress targetProtocolAddress,
            PhysicalAddress senderHardwareAddress,
            IPAddress senderProtocolAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            const int offset = 0;
            var length = ARPFields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            Operation = operation;
            TargetHardwareAddress = targetHardwareAddress;
            TargetProtocolAddress = targetProtocolAddress;
            SenderHardwareAddress = senderHardwareAddress;
            SenderProtocolAddress = senderProtocolAddress;

            // set some internal properties to fully define the packet
            HardwareAddressType = LinkLayers.Ethernet;
            HardwareAddressLength = EthernetFields.MacAddressLength;

            ProtocolAddressType = EthernetPacketType.IPv4;
            ProtocolAddressLength = IPv4Fields.AddressLength;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ARPPacket(ByteArraySegment bas)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            Header = new ByteArraySegment(bas);
            Header.Length = ARPFields.HeaderLength;

            // NOTE: no need to set the payloadPacketOrData field, arp packets have
            //       no payload
        }

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
                buffer.AppendFormat("{0}[ARPPacket: Operation={2}, SenderHardwareAddress={3}, TargetHardwareAddress={4}, SenderProtocolAddress={5}, TargetProtocolAddress={6}]{1}",
                                    color,
                                    colorEscape,
                                    Operation,
                                    HexPrinter.PrintMACAddress(SenderHardwareAddress),
                                    HexPrinter.PrintMACAddress(TargetHardwareAddress),
                                    SenderProtocolAddress,
                                    TargetProtocolAddress);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"hardware type", HardwareAddressType + " (0x" + HardwareAddressType.ToString("x") + ")"},
                    {"protocol type", ProtocolAddressType + " (0x" + ProtocolAddressType.ToString("x") + ")"},
                    {"operation", Operation + " (0x" + Operation.ToString("x") + ")"},
                    {"source hardware address", HexPrinter.PrintMACAddress(SenderHardwareAddress)},
                    {"destination hardware address", HexPrinter.PrintMACAddress(TargetHardwareAddress)},
                    {"source protocol address", SenderProtocolAddress.ToString()},
                    {"destination protocol address", TargetProtocolAddress.ToString()}
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("ARP:  ******* ARP - \"Address Resolution Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("ARP:");
                foreach (var property in properties)
                {
                    buffer.AppendLine("ARP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }

                buffer.AppendLine("ARP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}