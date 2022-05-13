/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet
{
    /// <summary>
    /// An ARP protocol packet.
    /// </summary>
    public sealed class ArpPacket : InternetLinkLayerPacket
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
        /// Create an ArpPacket from values
        /// </summary>
        /// <param name="operation">
        /// A <see cref="ArpOperation" />
        /// </param>
        /// <param name="targetHardwareAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="targetProtocolAddress">
        /// A <see cref="IPAddress" />
        /// </param>
        /// <param name="senderHardwareAddress">
        /// A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="senderProtocolAddress">
        /// A <see cref="IPAddress" />
        /// </param>
        public ArpPacket
        (
            ArpOperation operation,
            PhysicalAddress targetHardwareAddress,
            IPAddress targetProtocolAddress,
            PhysicalAddress senderHardwareAddress,
            IPAddress senderProtocolAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = ArpFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            Operation = operation;
            TargetHardwareAddress = targetHardwareAddress;
            TargetProtocolAddress = targetProtocolAddress;
            SenderHardwareAddress = senderHardwareAddress;
            SenderProtocolAddress = senderProtocolAddress;

            // set some internal properties to fully define the packet
            HardwareAddressType = LinkLayers.Ethernet;
            HardwareAddressLength = EthernetFields.MacAddressLength;

            ProtocolAddressType = EthernetType.IPv4;
            ProtocolAddressLength = IPv4Fields.AddressLength;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ArpPacket(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = ArpFields.HeaderLength
            };

            // NOTE: no need to set the payloadPacketOrData field, arp packets have
            //       no payload
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.Purple;

        /// <value>
        /// Hardware address length field
        /// </value>
        public int HardwareAddressLength
        {
            get => Header.Bytes[Header.Offset + ArpFields.HardwareAddressLengthPosition];
            set => Header.Bytes[Header.Offset + ArpFields.HardwareAddressLengthPosition] = (byte) value;
        }

        /// <value>
        /// Also known as HardwareType
        /// </value>
        public LinkLayers HardwareAddressType
        {
            get => (LinkLayers) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                Header.Offset + ArpFields.HardwareAddressTypePosition);
            set
            {
                var v = (ushort) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + ArpFields.HardwareAddressTypePosition);
            }
        }

        /// <summary>
        /// Gets or sets the operation code.
        /// </summary>
        public ArpOperation Operation
        {
            get => (ArpOperation) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + ArpFields.OperationPosition);
            set
            {
                var v = (short) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + ArpFields.OperationPosition);
            }
        }

        /// <value>
        /// Protocol address length field
        /// </value>
        public int ProtocolAddressLength
        {
            get => Header.Bytes[Header.Offset + ArpFields.ProtocolAddressLengthPosition];
            set => Header.Bytes[Header.Offset + ArpFields.ProtocolAddressLengthPosition] = (byte) value;
        }

        /// <value>
        /// Also known as ProtocolType
        /// </value>
        public EthernetType ProtocolAddressType
        {
            get => (EthernetType) EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                  Header.Offset + ArpFields.ProtocolAddressTypePosition);
            set
            {
                var v = (ushort) value;
                EndianBitConverter.Big.CopyBytes(v,
                                                 Header.Bytes,
                                                 Header.Offset + ArpFields.ProtocolAddressTypePosition);
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
                var hwAddress = new byte[HardwareAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + ArpFields.SenderHardwareAddressPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + ArpFields.SenderHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Upper layer protocol address of the sender, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public IPAddress SenderProtocolAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + ArpFields.SenderProtocolAddressPosition,
                                         Header.Bytes);
            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    ThrowHelper.ThrowInvalidAddressFamilyException(value.AddressFamily);

                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ArpFields.SenderProtocolAddressPosition,
                           address.Length);
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
                var hwAddress = new byte[HardwareAddressLength];
                Array.Copy(Header.Bytes,
                           Header.Offset + ArpFields.TargetHardwareAddressPosition,
                           hwAddress,
                           0,
                           hwAddress.Length);

                return new PhysicalAddress(hwAddress);
            }
            set
            {
                var hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value);

                Array.Copy(hwAddress,
                           0,
                           Header.Bytes,
                           Header.Offset + ArpFields.TargetHardwareAddressPosition,
                           hwAddress.Length);
            }
        }

        /// <value>
        /// Upper layer protocol address of the target, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public IPAddress TargetProtocolAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + ArpFields.TargetProtocolAddressPosition,
                                         Header.Bytes);
            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    ThrowHelper.ThrowInvalidAddressFamilyException(value.AddressFamily);

                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ArpFields.TargetProtocolAddressPosition,
                           address.Length);
            }
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

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                {
                    buffer.AppendFormat("{0}[ArpPacket: Operation={2}, SenderHardwareAddress={3}, TargetHardwareAddress={4}, SenderProtocolAddress={5}, TargetProtocolAddress={6}]{1}",
                                        color,
                                        colorEscape,
                                        Operation,
                                        HexPrinter.PrintMACAddress(SenderHardwareAddress),
                                        HexPrinter.PrintMACAddress(TargetHardwareAddress),
                                        SenderProtocolAddress,
                                        TargetProtocolAddress);

                    break;
                }
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                {
                    // collect the properties and their value
                    var properties = new Dictionary<string, string>
                    {
                        { "hardware type", HardwareAddressType + " (0x" + HardwareAddressType.ToString("x") + ")" },
                        { "protocol type", ProtocolAddressType + " (0x" + ProtocolAddressType.ToString("x") + ")" },
                        { "operation", Operation + " (0x" + Operation.ToString("x") + ")" },
                        { "source hardware address", HexPrinter.PrintMACAddress(SenderHardwareAddress) },
                        { "destination hardware address", HexPrinter.PrintMACAddress(TargetHardwareAddress) },
                        { "source protocol address", SenderProtocolAddress.ToString() },
                        { "destination protocol address", TargetProtocolAddress.ToString() }
                    };

                    // calculate the padding needed to right-justify the property names
                    var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("ARP:  ******* ARP - \"Address Resolution Protocol\" - offset=? length=" + TotalPacketLength);
                    buffer.AppendLine("ARP:");
                    foreach (var property in properties)
                        buffer.AppendLine("ARP: " + property.Key.PadLeft(padLength) + " = " + property.Value);

                    buffer.AppendLine("ARP:");
                    break;
                }
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }
    }
}