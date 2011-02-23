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
 * Copyright 2009 David Bond <mokon@mokon.net>
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// IPv6 packet
    ///
    /// References
    /// ----------
    /// http://tools.ietf.org/html/rfc2460
    /// http://en.wikipedia.org/wiki/IPv6
    /// </summary>
    public class IPv6Packet : IpPacket
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
        /// Minimum number of bytes in an IPv6 header
        /// </value>
        public const int HeaderMinimumLength = 40;

        /// <value>
        /// The version of the IP protocol. The '6' in IPv6 indicates the version of the protocol
        /// </value>
        public static IpVersion ipVersion = IpVersion.IPv6;

        private Int32 VersionTrafficClassFlowLabel
        {
            get
            {
                return EndianBitConverter.Big.ToInt32(header.Bytes,
                                                      header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
            }
        }

        /// <summary>
        /// The version field of the IPv6 Packet.
        /// </summary>
        public override IpVersion Version
        {
            get
            {
                return (IpVersion)((VersionTrafficClassFlowLabel >> 28) & 0xF);
            }

            set
            {
                var theValue = (Int32)value;

                // read the existing value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // mask the new field into place
                field = (UInt32)((field & 0x0FFFFFFF) | ((theValue << 28) & 0xF0000000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The traffic class field of the IPv6 Packet.
        /// </summary>
        public virtual int TrafficClass
        {
            get
            {
                return ((VersionTrafficClassFlowLabel >> 20) & 0xFF);
            }

            set
            {
                // read the original value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // mask in the new field
                field = (UInt32)(((field & 0xF00FFFFF) | (((UInt32)value) << 20 ) & 0x0FF00000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The flow label field of the IPv6 Packet.
        /// </summary>
        public virtual int FlowLabel
        {
            get
            {
                return (VersionTrafficClassFlowLabel & 0xFFFFF);
            }

            set
            {
                // read the original value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // make the value in
                field = (UInt32)((field & 0xFFF00000) | ((UInt32)(value) & 0x000FFFFF));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The payload lengeth field of the IPv6 Packet
        /// NOTE: Differs from the IPv4 'Total length' field that includes the length of the header as
        ///       payload length is ONLY the size of the payload.
        /// </summary>
        public override ushort PayloadLength
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + IPv6Fields.PayloadLengthPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + IPv6Fields.PayloadLengthPosition);
            }
        }

        /// <value>
        /// Backwards compatibility property for IPv4.HeaderLength
        /// NOTE: This field is the number of 32bit words
        /// </value>
        public override int HeaderLength
        {
            get
            {
                return (IPv6Fields.HeaderLength / 4);
            }

            set
            {
                throw new System.NotImplementedException ();
            }
        }

        /// <value>
        /// Backwards compatibility property for IPv4.TotalLength
        /// </value>
        public override int TotalLength
        {
            get
            {
                return PayloadLength + (HeaderLength * 4);
            }

            set
            {
                PayloadLength = (ushort)(value - (HeaderLength * 4));
            }
        }

        /// <summary>
        /// Identifies the protocol encapsulated by this packet
        ///
        /// Replaces IPv4's 'protocol' field, has compatible values
        /// </summary>
        public override IPProtocolType NextHeader
        {
            get
            {
                return (IPProtocolType)(header.Bytes[header.Offset + IPv6Fields.NextHeaderPosition]);
            }

            set
            {
                header.Bytes[header.Offset + IPv6Fields.NextHeaderPosition] = (byte)value;
            }
        }

        /// <value>
        /// The protocol of the packet encapsulated in this ip packet
        /// </value>
        public override IPProtocolType Protocol
        {
            get { return NextHeader; }
            set { NextHeader = value; }
        }

        /// <summary>
        /// The hop limit field of the IPv6 Packet.
        /// NOTE: Replaces the 'time to live' field of IPv4
        ///
        /// 8-bit value
        /// </summary>
        public override int HopLimit
        {
            get
            {
                return header.Bytes[header.Offset + IPv6Fields.HopLimitPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv6Fields.HopLimitPosition] = (byte)value;
            }
        }

        /// <value>
        /// Helper alias for 'HopLimit'
        /// </value>
        public override int TimeToLive
        {
            get { return HopLimit; }
            set { HopLimit = value; }
        }

        /// <summary>
        /// The source address field of the IPv6 Packet.
        /// </summary>
        public override System.Net.IPAddress SourceAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             header.Offset + IPv6Fields.SourceAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0,
                                  header.Bytes, header.Offset + IPv6Fields.SourceAddressPosition,
                                  address.Length);
            }
        }

        /// <summary>
        /// The destination address field of the IPv6 Packet.
        /// </summary>
        public override System.Net.IPAddress DestinationAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             header.Offset + IPv6Fields.DestinationAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0,
                                  header.Bytes, header.Offset + IPv6Fields.DestinationAddressPosition,
                                  address.Length);
            }
        }

        /// <summary>
        /// Create an IPv6 packet from values
        /// </summary>
        /// <param name="SourceAddress">
        /// A <see cref="System.Net.IPAddress"/>
        /// </param>
        /// <param name="DestinationAddress">
        /// A <see cref="System.Net.IPAddress"/>
        /// </param>
        public IPv6Packet(System.Net.IPAddress SourceAddress,
                          System.Net.IPAddress DestinationAddress)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = IPv6Fields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set some default values to make this packet valid
            PayloadLength = 0;
            TimeToLive = DefaultTimeToLive;

            // set instance values
            this.SourceAddress = SourceAddress;
            this.DestinationAddress = DestinationAddress;
            this.Version = ipVersion;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public IPv6Packet(ByteArraySegment bas)
        {
            log.Debug(bas.ToString());

            // slice off the header
            header = new ByteArraySegment(bas);
            header.Length = IPv6Packet.HeaderMinimumLength;

            // set the actual length, we need to do this because we need to set
            // header to something valid above before we can retrieve the PayloadLength
            log.DebugFormat("PayloadLength: {0}", PayloadLength);
            header.Length = bas.Length - PayloadLength;

            // parse the payload
            var payload = header.EncapsulatedBytes(PayloadLength);
            payloadPacketOrData = IpPacket.ParseEncapsulatedBytes(payload,
                                                                  NextHeader,
                                                                  this);
        }

        /// <summary>
        /// Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        /// generating an tcp checksum
        ///
        /// http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv6
        /// http://tools.ietf.org/html/rfc2460#page-27
        /// </summary>
        /// <param name="origHeader">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte"/>
        /// </returns>
        internal override byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // 0-16: ip src addr
            bw.Write(header.Bytes, header.Offset + IPv6Fields.SourceAddressPosition,
                     IPv6Fields.AddressLength);

            // 17-32: ip dst addr
            bw.Write(header.Bytes, header.Offset + IPv6Fields.DestinationAddressPosition,
                     IPv6Fields.AddressLength);

            // 33-36: TCP length
            bw.Write((UInt32)System.Net.IPAddress.HostToNetworkOrder((Int32)origHeader.Length));

            // 37-39: 3 bytes of zeros
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((byte)0);

            // 40: Next header
            bw.Write((byte)NextHeader);

            // prefix the pseudoHeader to the header+data
            byte[] pseudoHeader = ms.ToArray();
            int headerSize = pseudoHeader.Length + origHeader.Length;
            bool odd = origHeader.Length % 2 != 0;
            if (odd)
                headerSize++;

            byte[] finalData = new byte[headerSize];

            // copy the pseudo header in
            Array.Copy(pseudoHeader, 0, finalData, 0, pseudoHeader.Length);

            // copy the origHeader in
            Array.Copy(origHeader, 0, finalData, pseudoHeader.Length, origHeader.Length);

            //if not even length, pad with a zero
            if (odd)
                finalData[finalData.Length - 1] = 0;

            return finalData;
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
                buffer.AppendFormat("{0}[IPv6Packet: SourceAddress={2}, DestinationAddress={3}, NextHeader={4}]{1}",
                    color,
                    colorEscape,
                    SourceAddress,
                    DestinationAddress,
                    NextHeader);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                string ipVersion = Convert.ToString((int)Version, 2).PadLeft(4, '0');
                properties.Add("version", ipVersion + " .... .... .... .... .... .... .... = " + (int)Version);
                string trafficClass = Convert.ToString(TrafficClass, 2).PadLeft(8, '0').Insert(4, " ");
                properties.Add("traffic class", ".... " + trafficClass + " .... .... .... .... .... = 0x" + TrafficClass.ToString("x").PadLeft(8, '0'));
                string flowLabel = Convert.ToString(FlowLabel, 2).PadLeft(20, '0').Insert(16, " ").Insert(12, " ").Insert(8, " ").Insert(4, " ");
                properties.Add("flow label", ".... .... .... " + flowLabel + " = 0x" + FlowLabel.ToString("x").PadLeft(8, '0'));
                properties.Add("payload length", PayloadLength.ToString());
                properties.Add("next header", NextHeader.ToString() + " (0x" + NextHeader.ToString("x") + ")");
                properties.Add("hop limit", HopLimit.ToString());
                properties.Add("source", SourceAddress.ToString());
                properties.Add("destination", DestinationAddress.ToString());

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("IP:  ******* IP - \"Internet Protocol (Version 6)\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("IP:");
                foreach(var property in properties)
                {
                    if(property.Key.Trim() != "")
                    {
                        buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }
                    else
                    {
                        buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                    }
                }
                buffer.AppendLine("IP");
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public String Color
        {
            get
            {
                return AnsiEscapeSequences.White;
            }
        }

        /// <summary>
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static IPv6Packet RandomPacket()
        {
            var srcAddress = RandomUtils.GetIPAddress(ipVersion);
            var dstAddress = RandomUtils.GetIPAddress(ipVersion);
            return new IPv6Packet(srcAddress, dstAddress);
        }
    }
}
