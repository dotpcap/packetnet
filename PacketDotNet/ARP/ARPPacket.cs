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
using System.Text;
using PacketDotNet.Ethernet;
using PacketDotNet.IP;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.ARP
{
    /// <summary>
    ///     An ARP protocol packet.
    /// </summary>
    [Serializable]
    public class ARPPacket : InternetLinkLayerPacket
    {
#if DEBUG
        private static readonly log4net.ILog Log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <value>
        ///     Also known as HardwareType
        /// </value>
        public virtual LinkLayers HardwareAddressType
        {
            get => (LinkLayers) EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + ARPFields.HardwareAddressTypePosition);

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.HardwareAddressTypePosition);
            }
        }

        /// <value>
        ///     Also known as ProtocolType
        /// </value>
        public virtual EthernetPacketType ProtocolAddressType
        {
            get => (EthernetPacketType) EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + ARPFields.ProtocolAddressTypePosition);

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.ProtocolAddressTypePosition);
            }
        }

        /// <value>
        ///     Hardware address length field
        /// </value>
        public virtual Int32 HardwareAddressLength
        {
            get => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + ARPFields.HardwareAddressLengthPosition];

            set => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + ARPFields.HardwareAddressLengthPosition] = (Byte) value;
        }

        /// <value>
        ///     Protocol address length field
        /// </value>
        public virtual Int32 ProtocolAddressLength
        {
            get => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + ARPFields.ProtocolAddressLengthPosition];

            set => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + ARPFields.ProtocolAddressLengthPosition] = (Byte) value;
        }

        /// <summary>
        ///     Fetch the operation code.
        ///     Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        /// <summary>
        ///     Sets the operation code.
        ///     Usually one of ARPFields.{ARP_OP_REQ_CODE, ARP_OP_REP_CODE}.
        /// </summary>
        public virtual ARPOperation Operation
        {
            get => (ARPOperation) EndianBitConverter.Big.ToInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + ARPFields.OperationPosition);

            set
            {
                var theValue = (Int16) value;
                EndianBitConverter.Big.CopyBytes(theValue, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.OperationPosition);
            }
        }

        /// <value>
        ///     Upper layer protocol address of the sender, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public virtual IPAddress SenderProtocolAddress
        {
            get => IpPacket.GetIPAddress(AddressFamily.InterNetwork,
                this.HeaderByteArraySegment.Offset + ARPFields.SenderProtocolAddressPosition,
                this.HeaderByteArraySegment.Bytes);

            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Family != IPv4, ARP is used for IPv4, NDP for IPv6");

                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.SenderProtocolAddressPosition,
                    address.Length);
            }
        }

        /// <value>
        ///     Upper layer protocol address of the target, arp is used for IPv4, IPv6 uses NDP
        /// </value>
        public virtual IPAddress TargetProtocolAddress
        {
            get => IpPacket.GetIPAddress(AddressFamily.InterNetwork,
                this.HeaderByteArraySegment.Offset + ARPFields.TargetProtocolAddressPosition,
                this.HeaderByteArraySegment.Bytes);

            set
            {
                // check that the address family is ipv4
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new InvalidOperationException("Family != IPv4, ARP is used for IPv4, NDP for IPv6");

                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.TargetProtocolAddressPosition,
                    address.Length);
            }
        }

        /// <value>
        ///     Sender hardware address, usually an ethernet mac address
        /// </value>
        public virtual PhysicalAddress SenderHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                Byte[] hwAddress = new Byte[this.HardwareAddressLength];
                Array.Copy(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.SenderHardwareAddressPosition,
                    hwAddress, 0, hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }

            set
            {
                Byte[] hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("expected physical address length of "
                                                        + EthernetFields.MacAddressLength
                                                        + " but it was "
                                                        + hwAddress.Length);
                }

                Array.Copy(hwAddress, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.SenderHardwareAddressPosition,
                    hwAddress.Length);
            }
        }

        /// <value>
        ///     Target hardware address, usually an ethernet mac address
        /// </value>
        public virtual PhysicalAddress TargetHardwareAddress
        {
            get
            {
                //FIXME: this code is broken because it assumes that the address position is
                // a fixed position
                Byte[] hwAddress = new Byte[this.HardwareAddressLength];
                Array.Copy(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.TargetHardwareAddressPosition,
                    hwAddress, 0,
                    hwAddress.Length);
                return new PhysicalAddress(hwAddress);
            }
            set
            {
                Byte[] hwAddress = value.GetAddressBytes();

                // for now we only support ethernet addresses even though the arp protocol
                // makes provisions for varying length addresses
                if (hwAddress.Length != EthernetFields.MacAddressLength)
                {
                    throw new InvalidOperationException("expected physical address length of "
                                                        + EthernetFields.MacAddressLength
                                                        + " but it was "
                                                        + hwAddress.Length);
                }

                Array.Copy(hwAddress, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + ARPFields.TargetHardwareAddressPosition,
                    hwAddress.Length);
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.Purple;

        /// <summary>
        ///     Create an ARPPacket from values
        /// </summary>
        /// <param name="operation">
        ///     A <see cref="ARPOperation" />
        /// </param>
        /// <param name="targetHardwareAddress">
        ///     A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="targetProtocolAddress">
        ///     A <see cref="System.Net.IPAddress" />
        /// </param>
        /// <param name="senderHardwareAddress">
        ///     A <see cref="PhysicalAddress" />
        /// </param>
        /// <param name="senderProtocolAddress">
        ///     A <see cref="System.Net.IPAddress" />
        /// </param>
        public ARPPacket(ARPOperation operation,
            PhysicalAddress targetHardwareAddress,
            IPAddress targetProtocolAddress,
            PhysicalAddress senderHardwareAddress,
            IPAddress senderProtocolAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = ARPFields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            this.Operation = operation;
            this.TargetHardwareAddress = targetHardwareAddress;
            this.TargetProtocolAddress = targetProtocolAddress;
            this.SenderHardwareAddress = senderHardwareAddress;
            this.SenderProtocolAddress = senderProtocolAddress;

            // set some internal properties to fully define the packet
            this.HardwareAddressType = LinkLayers.Ethernet;
            this.HardwareAddressLength = EthernetFields.MacAddressLength;

            this.ProtocolAddressType = EthernetPacketType.IpV4;
            this.ProtocolAddressLength = IPv4Fields.AddressLength;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public ARPPacket(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = ARPFields.HeaderLength
            };

            // NOTE: no need to set the payloadPacketOrData field, arp packets have
            //       no payload
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override String ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            String color = "";
            String colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = this.Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat(
                    "{0}[ARPPacket: Operation={2}, SenderHardwareAddress={3}, TargetHardwareAddress={4}, SenderProtocolAddress={5}, TargetProtocolAddress={6}]{1}",
                    color,
                    colorEscape, this.Operation,
                    HexPrinter.PrintMACAddress(this.SenderHardwareAddress),
                    HexPrinter.PrintMACAddress(this.TargetHardwareAddress), this.SenderProtocolAddress,
                    this.TargetProtocolAddress);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<String, String> properties = new Dictionary<String, String>
                {
                    {"hardware type", this.HardwareAddressType + " (0x" + this.HardwareAddressType.ToString("x") + ")"},
                    {"protocol type", this.ProtocolAddressType + " (0x" + this.ProtocolAddressType.ToString("x") + ")"},
                    {"operation", this.Operation + " (0x" + this.Operation.ToString("x") + ")"},
                    {"source hardware address", HexPrinter.PrintMACAddress(this.SenderHardwareAddress)},
                    {"destination hardware address", HexPrinter.PrintMACAddress(this.TargetHardwareAddress)},
                    {"source protocol address", this.SenderProtocolAddress.ToString()},
                    {"destination protocol address", this.TargetProtocolAddress.ToString()}
                };

                // calculate the padding needed to right-justify the property names
                Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("ARP:  ******* ARP - \"Address Resolution Protocol\" - offset=? length=" +
                                  this.TotalPacketLength);
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