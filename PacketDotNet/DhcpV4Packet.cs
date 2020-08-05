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
 *  This file is licensed under the Apache License, Version 2.0.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
using PacketDotNet.DhcpV4;

namespace PacketDotNet
{
    public sealed class DhcpV4Packet : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DhcpV4Packet" /> class.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="parentPacket">The parent packet.</param>
        public DhcpV4Packet(ByteArraySegment byteArraySegment, Packet parentPacket)
        {
            Header = new ByteArraySegment(byteArraySegment);

            ParentPacket = parentPacket;
        }

        /// <summary>
        /// Gets or sets if broadcasting should occur.
        /// </summary>
        public bool Broadcast
        {
            get => (Flags & DhcpV4Fields.BroadcastMask) != 0;
            set => SetFlag(value, DhcpV4Fields.BroadcastMask);
        }

        /// <summary>
        /// Gets or sets the client address.
        /// </summary>
        public IPAddress ClientAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + DhcpV4Fields.CiAddrPosition,
                                         Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();

                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + DhcpV4Fields.CiAddrPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets or sets the client hardware address.
        /// </summary>
        public PhysicalAddress ClientHardwareAddress
        {
            get
            {
                var address = new byte[EthernetFields.MacAddressLength];
                Array.Copy(Header.Bytes, Header.Offset + DhcpV4Fields.ChAddrPosition, address, 0, address.Length);

                return new PhysicalAddress(address);
            }
            set
            {
                var addressBytes = value.GetAddressBytes();
                if (addressBytes.Length != EthernetFields.MacAddressLength)
                    throw new InvalidOperationException("Address length " + addressBytes.Length + " not equal to the expected length of " + EthernetFields.MacAddressLength + ".");


                Array.Copy(addressBytes, 0, Header.Bytes, Header.Offset + DhcpV4Fields.ChAddrPosition, addressBytes.Length);
            }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public ushort Flags
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DhcpV4Fields.FlagsPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + DhcpV4Fields.FlagsPosition);
        }

        /// <summary>
        /// Gets or sets the gateway address.
        /// </summary>
        public IPAddress GatewayAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + DhcpV4Fields.GiAddrPosition,
                                         Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();

                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + DhcpV4Fields.GiAddrPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets or sets the length of the hardware.
        /// </summary>
        public byte HardwareLength
        {
            get => Header.Bytes[Header.Offset + DhcpV4Fields.HardwareLengthPosition];
            set => Header.Bytes[Header.Offset + DhcpV4Fields.HardwareLengthPosition] = value;
        }

        /// <summary>
        /// Gets or sets the type of the hardware.
        /// </summary>
        public DhcpV4HardwareType HardwareType
        {
            get => (DhcpV4HardwareType) Header.Bytes[Header.Offset + DhcpV4Fields.HardwareTypePosition];
            set => Header.Bytes[Header.Offset + DhcpV4Fields.HardwareTypePosition] = (byte) value;
        }

        /// <summary>
        /// Gets or sets the hops count.
        /// </summary>
        public byte Hops
        {
            get => Header.Bytes[Header.Offset + DhcpV4Fields.HopsPosition];
            set => Header.Bytes[Header.Offset + DhcpV4Fields.HopsPosition] = value;
        }

        /// <summary>
        /// Gets or sets the magic number.
        /// </summary>
        public uint MagicNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + DhcpV4Fields.MagicNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + DhcpV4Fields.MagicNumberPosition);
        }

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        public DhcpV4MessageType MessageType
        {
            get
            {
                var options = Header.Bytes[Header.Offset + DhcpV4Fields.OptionsPosition];
                // The message type must be the first DHCP option, the option type for that is 53.
                if (options != (int) DhcpV4OptionType.DHCPMsgType)
                    return DhcpV4MessageType.End;


                return (DhcpV4MessageType) Header.Bytes[Header.Offset + DhcpV4Fields.OptionsPosition + 2];
            }
            set
            {
                var options = Header.Bytes[Header.Offset + DhcpV4Fields.OptionsPosition];
                // The message block must be the first DHCP option, the option type for that is 53.
                if (options != (int) DhcpV4OptionType.DHCPMsgType)
                    return;


                Header.Bytes[Header.Offset + DhcpV4Fields.OptionsPosition + 2] = (byte) value;
            }
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public DhcpV4Operation Operation
        {
            get => (DhcpV4Operation) Header.Bytes[Header.Offset + DhcpV4Fields.OperationPosition];
            set => Header.Bytes[Header.Offset + DhcpV4Fields.OperationPosition] = (byte) value;
        }

        /// <summary>
        /// Gets or sets the secs.
        /// </summary>
        public ushort Secs
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + DhcpV4Fields.SecsPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + DhcpV4Fields.SecsPosition);
        }

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        public IPAddress ServerAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + DhcpV4Fields.SiAddrPosition,
                                         Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();

                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + DhcpV4Fields.SiAddrPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets or sets the transaction id / xid.
        /// </summary>
        public uint TransactionId
        {
            get => Xid;
            set => Xid = value;
        }

        /// <summary>
        /// Gets or sets the transaction id / xid.
        /// </summary>
        public uint Xid
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + DhcpV4Fields.XidPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + DhcpV4Fields.XidPosition);
        }

        /// <summary>
        /// Gets or sets your address.
        /// </summary>
        public IPAddress YourAddress
        {
            get => IPPacket.GetIPAddress(AddressFamily.InterNetwork,
                                         Header.Offset + DhcpV4Fields.YiAddrPosition,
                                         Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();

                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + DhcpV4Fields.YiAddrPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Gets the current options.
        /// </summary>
        /// <remarks>You need to use <see cref="SetOptions" /> to make changes to the DhcpV4 options/.</remarks>
        public List<DhcpV4Option> GetOptions()
        {
            var options = new List<DhcpV4Option>();
            if (MessageType == DhcpV4MessageType.End)
                return options;


            var currentPosition = Header.Offset + DhcpV4Fields.OptionsPosition;
            var buffer = Header.Bytes;

            while (currentPosition < buffer.Length)
            {
                var optionType = (DhcpV4OptionType) buffer[currentPosition++];
                if (optionType == DhcpV4OptionType.End || currentPosition >= buffer.Length)
                    break;

                
                var optionLength = optionType == DhcpV4OptionType.Pad ? (byte)0 : buffer[currentPosition++];
                if (currentPosition + optionLength > buffer.Length)
                    break;


                var dhcpOption = GetDhcpV4Option(optionType, optionLength, buffer, currentPosition);
                if (dhcpOption != null)
                    options.Add(dhcpOption);

                currentPosition += optionLength;
            }

            return options;
        }

        /// <summary>
        /// Sets the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        public void SetOptions(IList<DhcpV4Option> options)
        {
            if (!(ParentPacket is UdpPacket udpPacket))
                return;


            var currentOptions = GetOptions();
            
            // Update the payload size of the UDP packet.
            // + 1 for the OptionType and + 1 for the Length of the option, if not a PadOption.
            var previousOptionsSize = currentOptions.Sum(x => 1 + x.Length + (x is PadOption ? 0 : 1));
            udpPacket.PayloadDataSegment.Length -= previousOptionsSize;
            var optionsSize = options.Sum(x => 1 + x.Length + (x is PadOption ? 0 : 1));
            udpPacket.PayloadDataSegment.Length += optionsSize;

            // Write the new options.
            var currentPosition = Header.Offset + DhcpV4Fields.OptionsPosition;

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];

                Header.Bytes[currentPosition++] = (byte) option.OptionType;
                if (option.OptionType == DhcpV4OptionType.Pad)
                    continue;


                Header.Bytes[currentPosition++] = (byte) option.Length;

                Array.Copy(option.Data, 0, Header.Bytes, currentPosition, option.Length);

                currentPosition += option.Length;
            }

            Header.Bytes[currentPosition] = (byte) DhcpV4OptionType.End;

            // Update the calculated values; otherwise the IP and UDP packet don't know about the new size.
            udpPacket.UpdateCalculatedValues();
            if (udpPacket.ParentPacket is IPPacket ipPacket)
                ipPacket.UpdateCalculatedValues();
        }

        /// <summary>
        /// Gets the DhcpV4 option.
        /// </summary>
        /// <param name="optionType">The option number.</param>
        /// <param name="optionLength">Length of the option.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><see cref="DhcpV4Option" />.</returns>
        private static DhcpV4Option GetDhcpV4Option(DhcpV4OptionType optionType, int optionLength, byte[] buffer, int offset)
        {
            switch (optionType)
            {
                case DhcpV4OptionType.AddressRequest:
                {
                    return new AddressRequestOption(buffer, offset);
                }
                case DhcpV4OptionType.AddressTime:
                {
                    return new AddressTimeOption(buffer, offset);
                }
                case DhcpV4OptionType.BroadcastAddress:
                {
                    return new BroadcastAddressOption(buffer, offset);
                }
                case DhcpV4OptionType.ClassId:
                {
                    return new ClassIdOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.ClientId:
                {
                    return new ClientIdOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.DHCPMaxMsgSize:
                {
                    return new MaxMessageSizeOption(buffer, offset);
                }
                case DhcpV4OptionType.DHCPMessage:
                {
                    return new MessageOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.DHCPMsgType:
                {
                    return new MessageTypeOption(buffer, offset);
                }
                case DhcpV4OptionType.DHCPServerId:
                {
                    return new ServerIdOption(buffer, offset);
                }
                case DhcpV4OptionType.DomainName:
                {
                    return new DomainNameOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.DomainServer:
                {
                    return new DomainNameServerOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.HostName:
                {
                    return new HostNameOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.NTPServers:
                {
                    return new NTPServersOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.Pad:
                {
                    return new PadOption();
                }
                case DhcpV4OptionType.ParameterList:
                {
                    return new ParameterListOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.RebindingTime:
                {
                    return new RebindingTimeOption(buffer, offset);
                }
                case DhcpV4OptionType.RenewalTime:
                {
                    return new RenewalTimeOption(buffer, offset);
                }
                case DhcpV4OptionType.Router:
                {
                    return new RouterOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.ServerName:
                {
                    return new TFTPServerNameOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.SubnetMask:
                {
                    return new SubnetMaskOption(buffer, offset);
                }
                case DhcpV4OptionType.TimeOffset:
                {
                    return new TimeOffsetOption(buffer, offset);
                }
                case DhcpV4OptionType.TimeServer:
                {
                    return new TimeServerOption(buffer, offset, optionLength);
                }
                case DhcpV4OptionType.VendorSpecific:
                {
                    return new VendorSpecificOption(buffer, offset, optionLength);
                }
                default:
                {
                    return new UnsupportedOption(buffer, offset, optionType, optionLength);
                }
            }
        }

        /// <summary>
        /// Sets the flag.
        /// </summary>
        /// <param name="enable"><c>true</c> to enable.</param>
        /// <param name="mask">The mask.</param>
        private void SetFlag(bool enable, int mask)
        {
            if (enable)
                Flags = (ushort) (Flags | mask);
            else
                Flags = (ushort) (Flags & ~mask);
        }

        /// <inheritdoc />
        public override string ToString(StringOutputType outputFormat)
        {
            var str = String.Empty;

            str += $"Transaction Id: {TransactionId}\r\n";
            str += $"Client Hardware Address: {ClientHardwareAddress}\r\n";
            str += $"Client Address: {ClientAddress}\r\n";
            str += $"Your Address: {YourAddress}\r\n";
            str += $"Gateway Address: {GatewayAddress}\r\n";
            str += $"Server Address: {ServerAddress}\r\n";
            str += $"Operation: {Operation}\r\n";
            str += $"Broadcast: {Broadcast}\r\n";
            str += $"Message Type: {MessageType}\r\n";
            str += "Options:\r\n";

            foreach (var option in GetOptions())
                str += $"{option.OptionType}: {option}\r\n";

            return str.Trim();
        }
    }
}