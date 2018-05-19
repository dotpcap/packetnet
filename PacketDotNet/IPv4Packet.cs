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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// IPv4 packet
    /// See http://en.wikipedia.org/wiki/IPv4 for into
    /// </summary>
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public sealed class IPv4Packet : IPPacket
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
        /// Number of bytes in the smallest valid ipv4 packet
        /// </value>
        public const Int32 HeaderMinimumLength = 20;

        /// <value>
        /// Version number of the IP protocol being used
        /// </value>
        public static IPVersion IPVersion = IPVersion.IPv4;

        /// <summary> Get the IP version code.</summary>
        public override IPVersion Version
        {
            get => (IPVersion) ((Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] >> 4) & 0x0F);

            set
            {
                // read the original value
                var theByte = Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition];

                // mask in the version bits
                theByte = (Byte) ((theByte & 0x0F) | (((Byte) value << 4) & 0xF0));

                // write back the modified value
                Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] = theByte;
            }
        }

        /// <value>
        /// Forwards compatibility IPv6.PayloadLength property
        /// </value>
        public override UInt16 PayloadLength
        {
            get => (UInt16) (TotalLength - (HeaderLength * 4));

            set => TotalLength = value + (HeaderLength * 4);
        }

        /// <summary>
        /// The IP header length field.  At most, this can be a
        /// four-bit value.  The high order bits beyond the fourth bit
        /// will be ignored.
        /// </summary>
        public override Int32 HeaderLength
        {
            get => Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] & 0x0F;

            set
            {
                // read the original value
                var theByte = Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition];

                // mask in the header length bits
                theByte = (Byte) ((theByte & 0xF0) | ((Byte) value & 0x0F));

                // write back the modified value
                Header.Bytes[Header.Offset + IPv4Fields.VersionAndHeaderLengthPosition] = theByte;
            }
        }

        /// <summary>
        /// The unique ID of this IP datagram. The ID normally
        /// increments by one each time a datagram is sent by a host.
        /// A 16-bit unsigned integer.
        /// </summary>
        public UInt16 Id
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv4Fields.IdPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + IPv4Fields.IdPosition);
        }

        /// <summary>
        /// Fragmentation offset
        /// The offset specifies a number of octets (i.e., bytes).
        /// A 13-bit unsigned integer.
        /// </summary>
        public Int32 FragmentOffset
        {
            get
            {
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                            Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask off the high flag bits
                return fragmentOffsetAndFlags & 0x1FFF;
            }

            set
            {
                // retrieve the value
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                            Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask the fragementation offset in
                fragmentOffsetAndFlags = (Int16) ((fragmentOffsetAndFlags & 0xE000) | (value & 0x1FFF));

                EndianBitConverter.Big.CopyBytes(fragmentOffsetAndFlags,
                                                 Header.Bytes,
                                                 Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);
            }
        }

        /// <summary> Fetch the IP address of the host where the packet originated from.</summary>
        public override IPAddress SourceAddress
        {
            get => GetIPAddress(AddressFamily.InterNetwork,
                                Header.Offset + IPv4Fields.SourcePosition,
                                Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + IPv4Fields.SourcePosition,
                           address.Length);
            }
        }

        /// <summary> Fetch the IP address of the host where the packet is destined.</summary>
        public override IPAddress DestinationAddress
        {
            get => GetIPAddress(AddressFamily.InterNetwork,
                                Header.Offset + IPv4Fields.DestinationPosition,
                                Header.Bytes);

            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + IPv4Fields.DestinationPosition,
                           address.Length);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        public UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv4Fields.ChecksumPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + IPv4Fields.ChecksumPosition);
            }
        }

        /// <summary> Check if the IP packet is valid, checksum-wise.</summary>
        public Boolean ValidChecksum => ValidIPChecksum;

        /// <summary>
        /// Check if the IP packet header is valid, checksum-wise.
        /// </summary>
        public Boolean ValidIPChecksum
        {
            get
            {
                Log.Debug("");


                // first validate other information about the packet. if this stuff
                // is not true, the packet (and therefore the checksum) is invalid
                // - ip_hl >= 5 (ip_hl is the length in 4-byte words)
                if (Header.Length < IPv4Fields.HeaderLength)
                {
                    Log.DebugFormat("invalid length, returning false");
                    return false;
                }

                var headerOnesSum = ChecksumUtils.OnesSum(Header, new byte[0]);

                Log.DebugFormat(HexPrinter.GetString(Header.ActualBytes(), 0, Header.Length));

                const Int32 expectedHeaderOnesSum = 0xFFFF;
                var retval = headerOnesSum == expectedHeaderOnesSum;

                Log.DebugFormat("headerOnesSum: {0}, expectedHeaderOnesSum {1}, returning {2}", headerOnesSum, expectedHeaderOnesSum, retval);
                Log.DebugFormat("Header.Length {0}", Header.Length);

                return retval;
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.White;

        /// <summary> Fetch the type of service. </summary>
        public Int32 DifferentiatedServices
        {
            get => Header.Bytes[Header.Offset + IPv4Fields.DifferentiatedServicesPosition];

            set => Header.Bytes[Header.Offset + IPv4Fields.DifferentiatedServicesPosition] = (Byte) value;
        }

        /// <value>
        /// Renamed to DifferentiatedServices in IPv6 but present here
        /// for backwards compatibility
        /// </value>
        public Int32 TypeOfService
        {
            get => DifferentiatedServices;
            set => DifferentiatedServices = value;
        }

        /// <value>
        /// The entire datagram size including header and data
        /// </value>
        public override Int32 TotalLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv4Fields.TotalLengthPosition);

            set
            {
                var theValue = (UInt16) value;
                EndianBitConverter.Big.CopyBytes(theValue,
                                                 Header.Bytes,
                                                 Header.Offset + IPv4Fields.TotalLengthPosition);
            }
        }

        /// <summary> Fetch fragment flags.</summary>
        public Int32 FragmentFlags
        {
            get
            {
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                            Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // shift off the fragment offset bits
                return fragmentOffsetAndFlags >> (16 - 3);
            }

            set
            {
                // retrieve the value
                var fragmentOffsetAndFlags = EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                            Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);

                // mask the flags in
                fragmentOffsetAndFlags = (Int16) ((fragmentOffsetAndFlags & 0x1FFF) | ((value & 0x07) << (16 - 3)));

                EndianBitConverter.Big.CopyBytes(fragmentOffsetAndFlags,
                                                 Header.Bytes,
                                                 Header.Offset + IPv4Fields.FragmentOffsetAndFlagsPosition);
            }
        }

        /// <summary>
        /// Fetch the time to live. TTL sets the upper limit on the number of
        /// routers through which this IP datagram is allowed to pass.
        /// Originally intended to be the number of seconds the packet lives it is now decremented
        /// by one each time a router passes the packet on
        /// 8-bit value
        /// </summary>
        public override Int32 TimeToLive
        {
            get => Header.Bytes[Header.Offset + IPv4Fields.TtlPosition];

            set => Header.Bytes[Header.Offset + IPv4Fields.TtlPosition] = (Byte) value;
        }

        /// <summary> Fetch the code indicating the type of protocol embedded in the IP</summary>
        /// <seealso cref="IPProtocolType">
        /// </seealso>
        public override IPProtocolType Protocol
        {
            get => (IPProtocolType) Header.Bytes[Header.Offset + IPv4Fields.ProtocolPosition];

            set => Header.Bytes[Header.Offset + IPv4Fields.ProtocolPosition] = (Byte) value;
        }

        /// <summary>
        /// Calculates the IP checksum, optionally updating the IP checksum header.
        /// </summary>
        /// <returns>The calculated IP checksum.</returns>
        public UInt16 CalculateIPChecksum()
        {
            var originalChecksum = Checksum;

            Checksum = 0; // This needs to be reset first to calculate the checksum.
            var calculatedChecksum = ChecksumUtils.OnesComplementSum(Header.Bytes, Header.Offset, Header.Length);

            Checksum = originalChecksum;
            return (UInt16) calculatedChecksum;
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateIPChecksum()
        {
            Checksum = CalculateIPChecksum();
        }

        internal override byte[] GetPseudoIPHeader(int originalHeaderLength)
        {
            Log.DebugFormat("origHeader.Length {0}",
                            originalHeaderLength);

            const int headerSize = 12;
            var headerForChecksum = new Byte[headerSize];

            // 0-7: ip src+dest addr
            Array.Copy(Header.Bytes,
                       Header.Offset + IPv4Fields.SourcePosition,
                       headerForChecksum,
                       0,
                       IPv4Fields.AddressLength * 2);

            // 8: always zero
            headerForChecksum[8] = 0;
            // 9: ip protocol
            headerForChecksum[9] = (Byte) Protocol;
            // 10-11: header+data length
            var length = (Int16) originalHeaderLength;
            EndianBitConverter.Big.CopyBytes(length,
                                             headerForChecksum,
                                             10);

            return headerForChecksum;
        }

        /// <summary>
        /// Construct an instance by values
        /// </summary>
        public IPv4Packet
        (
            IPAddress sourceAddress,
            IPAddress destinationAddress)
        {
            // allocate memory for this packet
            const int offset = 0;
            var length = IPv4Fields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            // set some default values to make this packet valid
            PayloadLength = 0;
            HeaderLength = HeaderMinimumLength / 4; // NOTE: HeaderLength is the number of 32bit words in the header
            TimeToLive = DefaultTimeToLive;

            // set instance values
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            Version = IPVersion;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public IPv4Packet(ByteArraySegment bas)
        {
            Log.Debug("");

            Header = new ByteArraySegment(bas);

            // TOS? See http://en.wikipedia.org/wiki/TCP_offload_engine
            var totalLength = TotalLength;
            if (totalLength == 0)
            {
                totalLength = Header.Length;
                TotalLength = totalLength;
            }

            // Check that the TotalLength is valid, at least HeaderMinimumLength long
            if (totalLength < HeaderMinimumLength)
            {
                throw new InvalidOperationException("TotalLength " + TotalLength + " < HeaderMinimumLength " + HeaderMinimumLength);
            }

            // update the header length with the correct value
            // NOTE: we take care to convert from 32bit words into bytes
            // NOTE: we do this *after* setting header because we need header to be valid
            //       before we can retrieve the HeaderLength property
            Header.Length = HeaderLength * 4;

            Log.DebugFormat("IPv4Packet HeaderLength {0}", HeaderLength);
            Log.DebugFormat("header {0}", Header);

            // parse the payload
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
            {
                var payload = Header.EncapsulatedBytes(PayloadLength);
                return ParseEncapsulatedBytes(payload,
                                              NextHeader,
                                              this);
            });
        }


        /// <summary>
        /// Constructor with parent
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public IPv4Packet
        (
            ByteArraySegment bas,
            Packet parentPacket) : this(bas)
        {
            ParentPacket = parentPacket;
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
                buffer.AppendFormat("{0}[IPv4Packet: SourceAddress={2}, DestinationAddress={3}, HeaderLength={4}, Protocol={5}, TimeToLive={6}]{1}",
                                    color,
                                    colorEscape,
                                    SourceAddress,
                                    DestinationAddress,
                                    HeaderLength,
                                    Protocol,
                                    TimeToLive);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                var properties = new Dictionary<String, String>
                {
                    {"version", Version.ToString()},
                    // FIXME: Header length output is incorrect
                    {"header length", HeaderLength + " bytes"}
                };
                var diffServices = Convert.ToString(DifferentiatedServices, 2).PadLeft(8, '0').Insert(4, " ");
                properties.Add("differentiated services", "0x" + DifferentiatedServices.ToString("x").PadLeft(2, '0'));
                properties.Add("", diffServices.Substring(0, 7) + ".. = [" + (DifferentiatedServices >> 2) + "] code point");
                properties.Add(" ", ".... .." + diffServices[6] + ". = [" + diffServices[6] + "] ECN");
                properties.Add("  ", ".... ..." + diffServices[7] + " = [" + diffServices[7] + "] ECE");
                properties.Add("total length", TotalLength.ToString());
                properties.Add("identification", "0x" + Id.ToString("x") + " (" + Id + ")");
                var flags = Convert.ToString(FragmentFlags, 2).PadLeft(8, '0').Substring(5, 3);
                properties.Add("flags", "0x" + FragmentFlags.ToString("x").PadLeft(2, '0'));
                properties.Add("   ", flags[0] + ".. = [" + flags[0] + "] reserved");
                properties.Add("    ", "." + flags[1] + ". = [" + flags[1] + "] don't fragment");
                properties.Add("     ", ".." + flags[2] + " = [" + flags[2] + "] more fragments");
                properties.Add("fragment offset", FragmentOffset.ToString());
                properties.Add("time to live", TimeToLive.ToString());
                properties.Add("protocol", Protocol + " (0x" + Protocol.ToString("x") + ")");
                properties.Add("header checksum", "0x" + Checksum.ToString("x") + " [" + (ValidChecksum ? "valid" : "invalid") + "]");
                properties.Add("source", SourceAddress.ToString());
                properties.Add("destination", DestinationAddress.ToString());

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                // build the output string
                buffer.AppendLine("IP:  ******* IPv4 - \"Internet Protocol (Version 4)\" - offset=? length=" + TotalPacketLength);
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
        /// A <see cref="Packet" />
        /// </returns>
        public static IPv4Packet RandomPacket()
        {
            var srcAddress = RandomUtils.GetIPAddress(IPVersion);
            var dstAddress = RandomUtils.GetIPAddress(IPVersion);
            return new IPv4Packet(srcAddress, dstAddress);
        }

        /// <summary>
        /// Update the length fields
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            // update the length field based on the length of this packet header
            // plus the length of all of the packets it contains
            TotalLength = TotalPacketLength;
        }
    }
}