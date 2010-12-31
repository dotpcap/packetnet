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
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// IPv4 packet
    /// See http://en.wikipedia.org/wiki/IPv4 for into
    /// </summary>
    public class IPv4Packet : IpPacket
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
        /// Number of bytes in the smallest valid ipv4 packet
        /// </value>
        public const int HeaderMinimumLength = 20;

        /// <summary> Type of service code constants for IP. Type of service describes
        /// how a packet should be handled.
        /// <p>
        /// TOS is an 8-bit record in an IP header which contains a 3-bit
        /// precendence field, 4 TOS bit fields and a 0 bit.
        /// </p>
        /// <p>
        /// The following constants are bit masks which can be logically and'ed
        /// with the 8-bit IP TOS field to determine what type of service is set.
        /// </p>
        /// <p>
        /// Taken from TCP/IP Illustrated V1 by Richard Stevens, p34.
        /// </p>
        /// </summary>
        public struct TypesOfService_Fields
        {
#pragma warning disable 1591
            public readonly static int MINIMIZE_DELAY = 0x10;
            public readonly static int MAXIMIZE_THROUGHPUT = 0x08;
            public readonly static int MAXIMIZE_RELIABILITY = 0x04;
            public readonly static int MINIMIZE_MONETARY_COST = 0x02;
            public readonly static int UNUSED = 0x01;
#pragma warning restore 1591
        }

        /// <value>
        /// Version number of the IP protocol being used
        /// </value>
        public static IpVersion ipVersion = IpVersion.IPv4;

        /// <summary> Get the IP version code.</summary>
        public override IpVersion Version
        {
            get
            {
                return (IpVersion)((header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] >> 4) & 0x0F);
            }

            set
            {
                // read the original value
                var theByte = header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition];

                // mask in the version bits
                theByte = (byte)((theByte & 0x0F) | (((byte)value << 4) & 0xF0));

                // write back the modified value
                header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] = theByte;
            }
        }

        /// <value>
        /// Forwards compatibility IPv6.PayloadLength property
        /// </value>
        public override ushort PayloadLength
        {
            get
            {
                return (ushort)(TotalLength - (HeaderLength * 4));
            }

            set
            {
                TotalLength = value + (HeaderLength * 4);
            }
        }

        /// <summary>
        /// The IP header length field.  At most, this can be a
        /// four-bit value.  The high order bits beyond the fourth bit
        /// will be ignored.
        /// </summary>
        /// <param name="length">The length of the IP header in 32-bit words.
        /// </param>
        public override int HeaderLength
        {
            get
            {
                return (header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition]) & 0x0F;
            }

            set
            {
                // read the original value
                var theByte = header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition];

                // mask in the header length bits
                theByte = (byte)((theByte & 0xF0) | (((byte)value) & 0x0F));

                // write back the modified value
                header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] = theByte;
            }
        }

        /// <summary>
        /// The unique ID of this IP datagram. The ID normally
        /// increments by one each time a datagram is sent by a host.
        /// A 16-bit unsigned integer.
        /// </summary>
        virtual public ushort Id
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + IPv4Fields.IdPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.IdPosition);
            }
        }

        /// <summary>
        /// Fragmentation offset
        /// The offset specifies a number of octets (i.e., bytes).
        /// A 13-bit unsigned integer.
        /// </summary>
        virtual public int FragmentOffset
        {
            get
            {
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask off the high flag bits
                return (fragmentOffsetAndFlags & 0x1FFF);
            }

            set
            {
                // retrieve the value
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask the fragementation offset in
                fragmentOffsetAndFlags = (short)((fragmentOffsetAndFlags & 0xE000) | (value & 0x1FFF));

                EndianBitConverter.Big.CopyBytes(fragmentOffsetAndFlags,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);
            }
        }

        /// <summary> Fetch the IP address of the host where the packet originated from.</summary>
        public override System.Net.IPAddress SourceAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + IPv4Fields.SourcePosition, header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + IPv4Fields.SourcePosition,
                           address.Length);
            }
        }

        /// <summary> Fetch the IP address of the host where the packet is destined.</summary>
        public override System.Net.IPAddress DestinationAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetwork,
                                             header.Offset + IPv4Fields.DestinationPosition, header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                           header.Bytes, header.Offset + IPv4Fields.DestinationPosition,
                           address.Length);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        virtual public ushort Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + IPv4Fields.ChecksumPosition);
            }

            set
            {
                var val = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.ChecksumPosition);
            }
        }

        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        virtual public bool ValidChecksum
        {
            get
            {
                return ValidIPChecksum;
            }

        }

        /// <summary>
        /// Check if the IP packet header is valid, checksum-wise.
        /// </summary>
        public bool ValidIPChecksum
        {
            get
            {
                log.Debug("");

                // first validate other information about the packet. if this stuff
                // is not true, the packet (and therefore the checksum) is invalid
                // - ip_hl >= 5 (ip_hl is the length in 4-byte words)
                if (Header.Length < IPv4Fields.HeaderLength)
                {
                    log.DebugFormat("invalid length, returning false");
                    return false;
                }
                else
                {
                    var headerOnesSum = ChecksumUtils.OnesSum(Header);
                    log.DebugFormat(HexPrinter.GetString(Header, 0, Header.Length));
                    const int expectedHeaderOnesSum = 0xffff;
                    var retval = (headerOnesSum == expectedHeaderOnesSum);
                    log.DebugFormat("headerOnesSum: {0}, expectedHeaderOnesSum {1}, returning {2}",
                                    headerOnesSum,
                                    expectedHeaderOnesSum,
                                    retval);
                    log.DebugFormat("Header.Length {0}", Header.Length);
                    return retval;
                }
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.White;
            }
        }

        /// <summary> Fetch the type of service. </summary>
        public int DifferentiatedServices
        {
            get
            {
                return header.Bytes[header.Offset + IPv4Fields.DifferentiatedServicesPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv4Fields.DifferentiatedServicesPosition] = (byte)value;
            }
        }

        /// <value>
        /// Renamed to DifferentiatedServices in IPv6 but present here
        /// for backwards compatibility
        /// </value>
        public int TypeOfService
        {
            get { return DifferentiatedServices; }
            set { DifferentiatedServices = value; }
        }

        /// <value>
        /// The entire datagram size including header and data
        /// </value>
        public override int TotalLength
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + IPv4Fields.TotalLengthPosition);
            }

            set
            {
                var theValue = (UInt16)value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.TotalLengthPosition);
            }
        }

        /// <summary> Fetch fragment flags.</summary>
        /// <param name="flags">A 3-bit unsigned integer.</param>
        public virtual int FragmentFlags
        {
            get
            {
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // shift off the fragment offset bits
                return fragmentOffsetAndFlags >> (16 - 3);
            }

            set
            {
                // retrieve the value
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask the flags in
                fragmentOffsetAndFlags = (short)((fragmentOffsetAndFlags & 0x1FFF) | ((value & 0x07) << (16 - 3)));

                EndianBitConverter.Big.CopyBytes(fragmentOffsetAndFlags,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);
            }
        }

        /// <summary> Fetch the time to live. TTL sets the upper limit on the number of
        /// routers through which this IP datagram is allowed to pass.
        /// Originally intended to be the number of seconds the packet lives it is now decremented
        /// by one each time a router passes the packet on
        ///
        /// 8-bit value
        /// </summary>
        public override int TimeToLive
        {
            get
            {
                return header.Bytes[header.Offset + IPv4Fields.TtlPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv4Fields.TtlPosition] = (byte)value;
            }
        }

        /// <summary> Fetch the code indicating the type of protocol embedded in the IP</summary>
        /// <seealso cref="IPProtocolType">
        /// </seealso>
        public override IPProtocolType Protocol
        {
            get
            {
                return (IPProtocolType)header.Bytes[header.Offset + IPv4Fields.ProtocolPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv4Fields.ProtocolPosition] = (byte)value;
            }
        }

        /// <summary>
        /// Calculates the IP checksum, optionally updating the IP checksum header.
        /// </summary>
        /// <returns> The calculated IP checksum.
        /// </returns>
        public ushort CalculateIPChecksum()
        {
            //copy the ip header
            var theHeader = Header;
            byte[] ip = new byte[theHeader.Length];
            Array.Copy(theHeader, ip, theHeader.Length);

            //reset the checksum field (checksum is calculated when this field is zeroed)
            var theValue = (UInt16)0;
            EndianBitConverter.Big.CopyBytes(theValue, ip, IPv4Fields.ChecksumPosition);

            //calculate the one's complement sum of the ip header
            int cs = ChecksumUtils.OnesComplementSum(ip, 0, ip.Length);

            return (ushort)cs;
        }

        /// <summary>
        /// Update the checksum value
        /// </summary>
        public void UpdateIPChecksum ()
        {
            this.Checksum = CalculateIPChecksum();
        }

        /// <summary>
        /// Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        /// generating an tcp checksum
        ///
        /// http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv4
        /// http://tools.ietf.org/html/rfc793
        /// </summary>
        /// <param name="origHeader">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Byte"/>
        /// </returns>
        internal override byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
            log.DebugFormat("origHeader.Length {0}",
                            origHeader.Length);

            bool odd = origHeader.Length % 2 != 0;
            int numberOfBytesFromIPHeaderUsedToGenerateChecksum = 12;
            int headerSize = numberOfBytesFromIPHeaderUsedToGenerateChecksum + origHeader.Length;
            if (odd)
                headerSize++;

            byte[] headerForChecksum = new byte[headerSize];
            // 0-7: ip src+dest addr
            Array.Copy(header.Bytes,
                       header.Offset + IPv4Fields.SourcePosition,
                       headerForChecksum,
                       0,
                       IPv4Fields.AddressLength * 2);
            // 8: always zero
            headerForChecksum[8] = 0;
            // 9: ip protocol
            headerForChecksum[9] = (byte)Protocol;
            // 10-11: header+data length
            var length = (Int16)origHeader.Length;
            EndianBitConverter.Big.CopyBytes(length, headerForChecksum,
                                             10);

            // prefix the pseudoHeader to the header+data
            Array.Copy(origHeader, 0,
                       headerForChecksum, numberOfBytesFromIPHeaderUsedToGenerateChecksum,
                       origHeader.Length);

            //if not even length, pad with a zero
            if (odd)
                headerForChecksum[headerForChecksum.Length - 1] = 0;

            return headerForChecksum;
        }

        /// <summary>
        /// Construct an instance by values
        /// </summary>
        public IPv4Packet(System.Net.IPAddress SourceAddress,
                          System.Net.IPAddress DestinationAddress)
        {
            // allocate memory for this packet
            int offset = 0;
            int length = IPv4Fields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set some default values to make this packet valid
            PayloadLength = 0;
            HeaderLength = (HeaderMinimumLength / 4); // NOTE: HeaderLength is the number of 32bit words in the header
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
        public IPv4Packet(ByteArraySegment bas)
        {
            log.Debug("");

            header = new ByteArraySegment(bas);

            // Check that the TotalLength is valid, at least HeaderMinimumLength long
            if(TotalLength < HeaderMinimumLength)
            {
                throw new System.InvalidOperationException("TotalLength " + TotalLength + " < HeaderMinimumLength " + HeaderMinimumLength);
            }

            // update the header length with the correct value
            // NOTE: we take care to convert from 32bit words into bytes
            // NOTE: we do this *after* setting header because we need header to be valid
            //       before we can retrieve the HeaderLength property
            header.Length = HeaderLength * 4;

            log.DebugFormat("IPv4Packet HeaderLength {0}", HeaderLength);
            log.DebugFormat("header {0}", header);

            // parse the payload
            var payload = header.EncapsulatedBytes(PayloadLength);
            payloadPacketOrData = IpPacket.ParseEncapsulatedBytes(payload,
                                                                  NextHeader,
                                                                  Timeval,
                                                                  this);
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
                buffer.AppendFormat("{0}[IPv4Packet: SourceAddress={2}, DestinationAddress={3}, HeaderLength={4}, Protocol={5}, TimeToLive={6}]{1}",
                    color,
                    colorEscape,
                    SourceAddress,
                    DestinationAddress,
                    HeaderLength,
                    Protocol,
                    TimeToLive);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("version", Version.ToString());
                // FIXME: Header length output is incorrect
                properties.Add("header length", HeaderLength + " bytes");
                string diffServices =  Convert.ToString(DifferentiatedServices, 2).PadLeft(8, '0').Insert(4, " ");
                properties.Add("differentiated services", "0x" + DifferentiatedServices.ToString("x").PadLeft(2, '0'));
                properties.Add("", diffServices.Substring(0, 7) + ".. = [" + (DifferentiatedServices >> 2) + "] code point");
                properties.Add(" ",".... .." + diffServices[6] + ". = [" + diffServices[6] + "] ECN");
                properties.Add("  ",".... ..." + diffServices[7] + " = [" + diffServices[7] + "] ECE");
                properties.Add("total length", TotalLength.ToString());
                properties.Add("identification", "0x" + Id.ToString("x") + " (" + Id + ")");
                string flags = Convert.ToString(FragmentFlags, 2).PadLeft(8, '0').Substring(5, 3);
                properties.Add("flags", "0x" + FragmentFlags.ToString("x").PadLeft(2, '0'));
                properties.Add("   ", flags[0] + ".. = [" +  flags[0] + "] reserved");
                properties.Add("    ", "." + flags[1] + ". = [" + flags[1] + "] don't fragment");
                properties.Add("     ", ".." + flags[2] + " = [" + flags[2] + "] more fragments");
                properties.Add("fragment offset", FragmentOffset.ToString());
                properties.Add("time to live", TimeToLive.ToString());
                properties.Add("protocol", Protocol.ToString() + " (0x" + Protocol.ToString("x") + ")");
                properties.Add("header checksum", "0x" + Checksum.ToString("x") + " [" + (ValidChecksum ? "valid" : "invalid") + "]");
                properties.Add("source", SourceAddress.ToString());
                properties.Add("destination", DestinationAddress.ToString());

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("IP:  ******* IPv4 - \"Internet Protocol (Version 4)\" - offset=? length=" + TotalPacketLength);
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
                buffer.AppendLine("IP:");
            }

            // append the base class output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="Packet"/>
        /// </returns>
        public static IPv4Packet RandomPacket()
        {
            var srcAddress = RandomUtils.GetIPAddress(ipVersion);
            var dstAddress = RandomUtils.GetIPAddress(ipVersion);
            return new IPv4Packet(srcAddress, dstAddress);
        }

        /// <summary>
        /// Update the length fields
        /// </summary>
        public override void UpdateCalculatedValues ()
        {
            // update the length field based on the length of this packet header
            // plus the length of all of the packets it contains
            TotalLength = TotalPacketLength;
        }
    }
}
