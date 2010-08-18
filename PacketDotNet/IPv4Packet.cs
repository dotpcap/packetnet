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
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
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
        virtual public int Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + IPv4Fields.ChecksumPosition);
            }

            set
            {
                var val = (Int16)value;
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
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + IPv4Fields.TotalLengthPosition);
            }

            set
            {
                var theValue = (Int16)value;
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
        public int CalculateIPChecksum()
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

            return cs;
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
            log.Debug("");

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
            : base(new PosixTimeval())
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
        /// Parse bytes into an IP packet
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public IPv4Packet(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        {
            log.Debug("");
        }

        /// <summary>
        /// Parse bytes into an IP packet
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public IPv4Packet(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            log.Debug("");

            header = new ByteArraySegment(Bytes, Offset, Bytes.Length - Offset);

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
            payloadPacketOrData = IpPacket.ParseEncapsulatedBytes(header,
                                                                  NextHeader,
                                                                  Timeval,
                                                                  this);
        }

        /// <summary> Convert this IP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this IP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IPv4Packet");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" HeaderLength=" + HeaderLength);
            buffer.Append(" Protocol=" + Protocol);
            buffer.Append(" TimeToLive=" + TimeToLive);            
            // FIXME: what would we use for Length?
//            buffer.Append(" l=" + HeaderLength + "," + Length);
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("IPv4Packet");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            buffer.Append("version=" + Version + ", ");
            buffer.Append("hlen=" + HeaderLength + ", ");
            buffer.Append("tos=" + TypeOfService + ", ");
            //FIXME: what to use for length here?
//            buffer.Append("length=" + Length + ", ");
            buffer.Append("id=" + Id + ", ");
            buffer.Append("flags=0x" + System.Convert.ToString(FragmentFlags, 16) + ", ");
            buffer.Append("offset=" + FragmentOffset + ", ");
            buffer.Append("ttl=" + TimeToLive + ", ");
            buffer.Append("proto=" + Protocol + ", ");
            buffer.Append("sum=0x" + System.Convert.ToString(Checksum, 16));
#if false
            if (this.ValidChecksum)
                buffer.Append(" (correct), ");
            else
                buffer.Append(" (incorrect, should be " + ComputeIPChecksum(false) + "), ");
#endif
            buffer.Append("src=" + SourceAddress + ", ");
            buffer.Append("dest=" + DestinationAddress);
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredVerboseString(colored));

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
