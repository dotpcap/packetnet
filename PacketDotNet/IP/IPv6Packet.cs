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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.IP
{
    /// <summary>
    ///     IPv6 packet
    ///     References
    ///     ----------
    ///     http://tools.ietf.org/html/rfc2460
    ///     http://en.wikipedia.org/wiki/IPv6
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class IPv6Packet : IpPacket
    {
#if DEBUG
        private static readonly log4net.ILog log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <value>
        ///     Minimum number of bytes in an IPv6 header
        /// </value>
        public const Int32 HeaderMinimumLength = 40;

        /// <value>
        ///     The version of the IP protocol. The '6' in IPv6 indicates the version of the protocol
        /// </value>
        public static IpVersion IPVersion = IpVersion.IPv6;

        private Int32 VersionTrafficClassFlowLabel
        {
            get => EndianBitConverter.Big.ToInt32(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);

            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
        }

        /// <summary>
        ///     The version field of the IPv6 Packet.
        /// </summary>
        public override IpVersion Version
        {
            get => (IpVersion) ((this.VersionTrafficClassFlowLabel >> 28) & 0xF);

            set
            {
                var theValue = (Int32) value;

                // read the existing value
                var field = (UInt32) this.VersionTrafficClassFlowLabel;

                // mask the new field into place
                field = (UInt32) ((field & 0x0FFFFFFF) | ((theValue << 28) & 0xF0000000));

                // write the updated value back
                this.VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        ///     The traffic class field of the IPv6 Packet.
        /// </summary>
        public virtual Int32 TrafficClass
        {
            get => ((this.VersionTrafficClassFlowLabel >> 20) & 0xFF);

            set
            {
                // read the original value
                var field = (UInt32) this.VersionTrafficClassFlowLabel;

                // mask in the new field
                field = (field & 0xF00FFFFF) | (((UInt32) value) << 20) & 0x0FF00000;

                // write the updated value back
                this.VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        ///     The flow label field of the IPv6 Packet.
        /// </summary>
        public virtual Int32 FlowLabel
        {
            get => (this.VersionTrafficClassFlowLabel & 0xFFFFF);

            set
            {
                // read the original value
                var field = (UInt32) this.VersionTrafficClassFlowLabel;

                // make the value in
                field = (field & 0xFFF00000) | ((UInt32) (value) & 0x000FFFFF);

                // write the updated value back
                this.VersionTrafficClassFlowLabel = (Int32) field;
            }
        }

        /// <summary>
        ///     The payload lengeth field of the IPv6 Packet
        ///     NOTE: Differs from the IPv4 'Total length' field that includes the length of the header as
        ///     payload length is ONLY the size of the payload.
        /// </summary>
        public override UInt16 PayloadLength
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.PayloadLengthPosition);

            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.PayloadLengthPosition);
        }

        /// <value>
        ///     Backwards compatibility property for IPv4.HeaderLength
        ///     NOTE: This field is the number of 32bit words
        /// </value>
        public override Int32 HeaderLength
        {
            get => (IPv6Fields.HeaderLength / 4);

            set => throw new NotImplementedException();
        }

        /// <value>
        ///     Backwards compatibility property for IPv4.TotalLength
        /// </value>
        public override Int32 TotalLength
        {
            get => this.PayloadLength + (this.HeaderLength * 4);

            set => this.PayloadLength = (UInt16) (value - (this.HeaderLength * 4));
        }

        /// <summary>
        ///     Identifies the protocol encapsulated by this packet
        ///     Replaces IPv4's 'protocol' field, has compatible values
        /// </summary>
        public override IPProtocolType NextHeader
        {
            get => (IPProtocolType) (this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + IPv6Fields.NextHeaderPosition]);

            set =>
                this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + IPv6Fields.NextHeaderPosition] =
                    (Byte) value;
        }

        /// <value>
        ///     The protocol of the packet encapsulated in this ip packet
        /// </value>
        public override IPProtocolType Protocol
        {
            get => this.NextHeader;
            set => this.NextHeader = value;
        }

        /// <summary>
        ///     The hop limit field of the IPv6 Packet.
        ///     NOTE: Replaces the 'time to live' field of IPv4
        ///     8-bit value
        /// </summary>
        public override Int32 HopLimit
        {
            get => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + IPv6Fields.HopLimitPosition];

            set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + IPv6Fields.HopLimitPosition] =
                (Byte) value;
        }

        /// <value>
        ///     Helper alias for 'HopLimit'
        /// </value>
        public override Int32 TimeToLive
        {
            get => this.HopLimit;
            set => this.HopLimit = value;
        }

        /// <summary>
        ///     The source address field of the IPv6 Packet.
        /// </summary>
        public override IPAddress SourceAddress
        {
            get => GetIPAddress(AddressFamily.InterNetworkV6,
                this.HeaderByteArraySegment.Offset + IPv6Fields.SourceAddressPosition,
                this.HeaderByteArraySegment.Bytes);

            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + IPv6Fields.SourceAddressPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     The destination address field of the IPv6 Packet.
        /// </summary>
        public override IPAddress DestinationAddress
        {
            get => GetIPAddress(AddressFamily.InterNetworkV6,
                this.HeaderByteArraySegment.Offset + IPv6Fields.DestinationAddressPosition,
                this.HeaderByteArraySegment.Bytes);

            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + IPv6Fields.DestinationAddressPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     Create an IPv6 packet from values
        /// </summary>
        /// <param name="sourceAddress">
        ///     A <see cref="System.Net.IPAddress" />
        /// </param>
        /// <param name="destinationAddress">
        ///     A <see cref="System.Net.IPAddress" />
        /// </param>
        public IPv6Packet(IPAddress sourceAddress,
            IPAddress destinationAddress)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = IPv6Fields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            // set some default values to make this packet valid
            this.PayloadLength = 0;
            this.TimeToLive = this.DefaultTimeToLive;

            // set instance values
            this.SourceAddress = sourceAddress;
            this.DestinationAddress = destinationAddress;
            this.Version = IPVersion;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public IPv6Packet(ByteArraySegment bas)
        {
            Log.Debug(bas.ToString());

            // slice off the header
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = HeaderMinimumLength
            };

            // set the actual length, we need to do this because we need to set
            // header to something valid above before we can retrieve the PayloadLength
            Log.DebugFormat("PayloadLength: {0}", this.PayloadLength);
            this.HeaderByteArraySegment.Length = bas.Length - this.PayloadLength;

            // parse the payload
            var payload = this.HeaderByteArraySegment.EncapsulatedBytes(this.PayloadLength);
            this.PayloadPacketOrData = ParseEncapsulatedBytes(payload, this.NextHeader,
                this);
        }

        /// <summary>
        ///     Constructor with parent
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        ///     A <see cref="Packet" />
        /// </param>
        public IPv6Packet(ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            this.ParentPacket = parentPacket;
        }


        /// <summary>
        ///     Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        ///     generating an tcp checksum
        ///     http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv6
        ///     http://tools.ietf.org/html/rfc2460#page-27
        /// </summary>
        /// <param name="origHeader">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <returns>
        ///     A <see cref="System.Byte" />
        /// </returns>
        internal override Byte[] AttachPseudoIPHeader(Byte[] origHeader)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // 0-16: ip src addr
            bw.Write(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.SourceAddressPosition,
                IPv6Fields.AddressLength);

            // 17-32: ip dst addr
            bw.Write(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + IPv6Fields.DestinationAddressPosition,
                IPv6Fields.AddressLength);

            // 33-36: TCP length
            bw.Write((UInt32) IPAddress.HostToNetworkOrder(origHeader.Length));

            // 37-39: 3 bytes of zeros
            bw.Write((Byte) 0);
            bw.Write((Byte) 0);
            bw.Write((Byte) 0);

            // 40: Next header
            bw.Write((Byte) this.NextHeader);

            // prefix the pseudoHeader to the header+data
            Byte[] pseudoHeader = ms.ToArray();
            Int32 headerSize = pseudoHeader.Length + origHeader.Length;
            Boolean odd = origHeader.Length % 2 != 0;
            if (odd)
                headerSize++;

            Byte[] finalData = new Byte[headerSize];

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

            switch (outputFormat)
            {
                case StringOutputType.Normal:
                case StringOutputType.Colored:
                    // build the output string
                    buffer.AppendFormat("{0}[IPv6Packet: SourceAddress={2}, DestinationAddress={3}, NextHeader={4}]{1}",
                        color,
                        colorEscape, this.SourceAddress, this.DestinationAddress, this.NextHeader);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>();
                    String ipVersion = Convert.ToString((Int32) this.Version, 2).PadLeft(4, '0');
                    properties.Add("version",
                        ipVersion + " .... .... .... .... .... .... .... = " + (Int32) this.Version);
                    String trafficClass = Convert.ToString(this.TrafficClass, 2).PadLeft(8, '0').Insert(4, " ");
                    properties.Add("traffic class",
                        ".... " + trafficClass + " .... .... .... .... .... = 0x" +
                        this.TrafficClass.ToString("x").PadLeft(8, '0'));
                    String flowLabel = Convert.ToString(this.FlowLabel, 2).PadLeft(20, '0').Insert(16, " ")
                        .Insert(12, " ").Insert(8, " ").Insert(4, " ");
                    properties.Add("flow label",
                        ".... .... .... " + flowLabel + " = 0x" + this.FlowLabel.ToString("x").PadLeft(8, '0'));
                    properties.Add("payload length", this.PayloadLength.ToString());
                    properties.Add("next header", this.NextHeader + " (0x" + this.NextHeader.ToString("x") + ")");
                    properties.Add("hop limit", this.HopLimit.ToString());
                    properties.Add("source", this.SourceAddress.ToString());
                    properties.Add("destination", this.DestinationAddress.ToString());

                    // calculate the padding needed to right-justify the property names
                    Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("IP:  ******* IP - \"Internet Protocol (Version 6)\" - offset=? length=" +
                                      this.TotalPacketLength);
                    buffer.AppendLine("IP:");
                    foreach (var property in properties)
                    {
                        if (property.Key.Trim() != "")
                        {
                            buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                        }
                        else
                        {
                            buffer.AppendLine("IP: " + property.Key.PadLeft(padLength) + "   " + property.Value);
                        }
                    }

                    buffer.AppendLine("IP");
                    break;
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.White;

        /// <summary>
        ///     Generate a random packet
        /// </summary>
        /// <returns>
        ///     A <see cref="Packet" />
        /// </returns>
        public static IPv6Packet RandomPacket()
        {
            var srcAddress = RandomUtils.GetIPAddress(IPVersion);
            var dstAddress = RandomUtils.GetIPAddress(IPVersion);
            return new IPv6Packet(srcAddress, dstAddress);
        }
    }
}