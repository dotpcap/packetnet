/*
This file is part of Packet.Net

Packet.Net is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Packet.Net is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Packet.Net.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using MiscUtil.Conversion;
using Packet.Net.Utils;

namespace Packet.Net
{
    /// <summary>
    /// IPv4 packet
    /// See http://en.wikipedia.org/wiki/IPv4 for into
    /// </summary>
    public class IPv4Packet : IpPacket
    {
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
            public readonly static int MINIMIZE_DELAY = 0x10;
            public readonly static int MAXIMIZE_THROUGHPUT = 0x08;
            public readonly static int MAXIMIZE_RELIABILITY = 0x04;
            public readonly static int MINIMIZE_MONETARY_COST = 0x02;
            public readonly static int UNUSED = 0x01;
        }

        public static int ipVersion = 4;

        /// <summary> Get the IP version code.</summary>
        public override int Version
        {
            get
            {
                return (header.Bytes[header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] >> 4) & 0x0F;
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

        /// <summary> Fetch the IP header length in bytes. </summary>
        /// <summary> Sets the IP header length field.  At most, this can be a 
        /// four-bit value.  The high order bits beyond the fourth bit
        /// will be ignored.
        /// 
        /// </summary>
        /// <param name="length">The length of the IP header in 32-bit words.
        /// </param>
        public int HeaderLength
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

        /// <summary> Fetch the unique ID of this IP datagram. The ID normally 
        /// increments by one each time a datagram is sent by a host.
        /// </summary>
        /// <summary> Sets the IP identification header value.
        /// 
        /// </summary>
        /// <param name="id">A 16-bit unsigned integer.
        /// </param>
        virtual public int Id
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + IPv4Fields.IdPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + IPv4Fields.IdPosition);
            }
        }

        /// <summary> Fetch fragmentation offset.</summary>
        /// <summary> Sets the fragment offset header value.  The offset specifies a
        /// number of octets (i.e., bytes).
        /// 
        /// </summary>
        /// <param name="offset">A 13-bit unsigned integer.
        /// </param>
        virtual public int FragmentOffset
        {
            get
            {
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask off the high flag bits
                return (fragmentOffsetAndFlags & 0x1FFFF);
            }

            set
            {
                // retrieve the value
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(header.Bytes,
                                                                           header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask the fragementation offset in
                fragmentOffsetAndFlags = (short)((fragmentOffsetAndFlags & 0xE000) | (value & 0x1FFFF));

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

#if false
        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        virtual public bool ValidChecksum
        {
            get
            {
                return ValidIPChecksum;
            }

        }

        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        virtual public bool ValidIPChecksum
        {
            get
            {
                // first validate other information about the packet. if this stuff
                // is not true, the packet (and therefore the checksum) is invalid
                // - ip_hl >= 5 (ip_hl is the length in 4-byte words)
                if (IPHeaderLength < IPv4Fields_Fields.IP_HEADER_LEN)
                {
                    return false;
                }
                else
                {
                    return (ChecksumUtils.OnesSum(Bytes, _ethPayloadOffset, IPHeaderLength) == 0xffff);
                }
            }
        }
#endif

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.WHITE;
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
        /// Renamed to DifferentiatedServices but present here
        /// for backwards compatibility
        /// </value>
        public int TypeOfService
        {
            get { return DifferentiatedServices; }
            set { DifferentiatedServices = value; }
        }

        /// <summary> Fetch the IP length in bytes.</summary>
        public virtual int TotalLength
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
                fragmentOffsetAndFlags = (short)((fragmentOffsetAndFlags & 0x1FFF) | ((value & 0xE0) << (16 - 3)));

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
        public virtual int TimeToLive
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
        /// <seealso cref="IPProtocol.IPProtocolType">
        /// </seealso>
        public virtual IPProtocol.IPProtocolType Protocol
        {
            get
            {
                return (IPProtocol.IPProtocolType)header.Bytes[header.Offset + IPv4Fields.ProtocolPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv4Fields.ProtocolPosition] = (byte)value;
            }
        }

#if false
        /// <summary> Sets the IP header checksum.</summary>
        protected internal virtual void SetChecksum(int cs, int checkSumOffset)
        {
            ArrayHelper.insertLong(Bytes, cs, checkSumOffset, 2);
        }

        protected internal virtual void SetTransportLayerChecksum(int cs, int csPos)
        {
            SetChecksum(cs, _ipOffset + csPos);
        }
#endif

#if false
        /// <summary> Computes the IP checksum, optionally updating the IP checksum header.
        /// 
        /// </summary>
        /// <param name="update">Specifies whether or not to update the IP checksum
        /// header after computing the checksum.  A value of true indicates
        /// the header should be updated, a value of false indicates it
        /// should not be updated.
        /// </param>
        /// <returns> The computed IP checksum.
        /// </returns>
        public int ComputeIPChecksum(bool update)
        {
            //copy the ip header
            byte[] ip = ArrayHelper.copy(Bytes, _ethPayloadOffset, IPHeaderLength);
            //reset the checksum field (checksum is calculated when this field is zeroed)
            ArrayHelper.insertLong(ip, 0, IPv4Fields_Fields.IP_CSUM_POS, 2);
            //compute the one's complement sum of the ip header
            int cs = ChecksumUtils.OnesComplementSum(ip, 0, ip.Length);
            if (update)
            {
                IPChecksum = cs;
            }

            return cs;
        }

        /// <summary> Same as <code>computeIPChecksum(true);</code>
        /// 
        /// </summary>
        /// <returns> The computed IP checksum value.
        /// </returns>
        public int ComputeIPChecksum()
        {
            return ComputeIPChecksum(true);
        }
#endif

        // Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        // generating an tcp checksum
        //
        // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv4
        // http://tools.ietf.org/html/rfc793
        internal override byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
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
            var length = (Int32)origHeader.Length;
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

#if false
        public override bool IsValid(out string errorString)
        {
            errorString = string.Empty;

            // validate the base class(es)
            bool baseValid = base.IsValid(out errorString);

            // perform some quick validation
            if(IPTotalLength < IPHeaderLength)
            {
                errorString += string.Format("IPTotalLength {0} < IPHeaderLength {1}",
                                            IPTotalLength, IPHeaderLength);
                return false;
            }

            return baseValid;
        }

        public virtual bool IsValidTransportLayerChecksum(bool pseudoIPHeader)
        {
            byte[] upperLayer = IPData;
            if (pseudoIPHeader)
                upperLayer = AttachPseudoIPHeader(upperLayer);
            int onesSum = ChecksumUtils.OnesSum(upperLayer);
            return (onesSum == 0xffff);
        }
#endif

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
                buffer.Append(AnsiEscapeSequences.RESET);
            buffer.Append(": ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" proto=" + Protocol);
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
                buffer.Append(AnsiEscapeSequences.RESET);
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

#if false
        /// <summary> This inner class provides access to private methods for unit testing.</summary>
        public class TestProbe
        {
            public TestProbe(IPv4Packet enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            private void InitBlock(IPv4Packet enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            private IPv4Packet enclosingInstance;
            virtual public int ComputedReceiverIPChecksum
            {
                get
                {
                    return ChecksumUtils.OnesSum(Enclosing_Instance.Bytes,
                                                 Enclosing_Instance._ethPayloadOffset,
                                                 Enclosing_Instance.IPHeaderLength);
                }
            }

            virtual public int ComputedSenderIPChecksum()
            {
                return Enclosing_Instance.ComputeIPChecksum(false);
            }

            public IPv4Packet Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
            }
        }       
#endif
    }
}
