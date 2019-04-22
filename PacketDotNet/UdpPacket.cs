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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using log4net;

#endif

namespace PacketDotNet
{
    /// <summary>
    /// User datagram protocol
    /// See http://en.wikipedia.org/wiki/Udp
    /// </summary>
    [Serializable]
    public sealed class UdpPacket : TransportPacket
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
        /// Create from values
        /// </summary>
        /// <param name="sourcePort">
        /// A <see cref="ushort" />
        /// </param>
        /// <param name="destinationPort">
        /// A <see cref="ushort" />
        /// </param>
        public UdpPacket(ushort sourcePort, ushort destinationPort)
        {
            Log.Debug("");

            // allocate memory for this packet
            var length = UdpFields.HeaderLength;
            var headerBytes = new byte[length];
            Header = new ByteArraySegment(headerBytes, 0, length);

            // set instance values
            SourcePort = sourcePort;
            DestinationPort = destinationPort;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public UdpPacket(ByteArraySegment byteArraySegment)
        {
            Log.DebugFormat("ByteArraySegment {0}", byteArraySegment);

            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = UdpFields.HeaderLength
            };

            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() =>
                                                                     {
                                                                         const int wakeOnLanPort0 = 0;
                                                                         const int wakeOnLanPort7 = 7;
                                                                         const int wakeOnLanPort9 = 9;
                                                                         const int l2TpPort = 1701;
                                                                         const int teredoPort = 3544;

                                                                         var result = new PacketOrByteArraySegment();
                                                                         var destinationPort = DestinationPort;
                                                                         var sourcePort = SourcePort;
                                                                         var payload = Header.NextSegment();

                                                                         // If this packet is going to port 0, 7 or 9, then it might be a WakeOnLan packet.
                                                                         if ((destinationPort == wakeOnLanPort0) || (destinationPort == wakeOnLanPort7) || (destinationPort == wakeOnLanPort9))
                                                                             if (WakeOnLanPacket.IsValid(payload))
                                                                             {
                                                                                 result.Packet = new WakeOnLanPacket(payload);
                                                                                 return result;
                                                                             }

                                                                         if ((destinationPort == l2TpPort) || (sourcePort == l2TpPort))
                                                                         {
                                                                             result.Packet = new L2tpPacket(payload, this);
                                                                             return result;
                                                                         }

                                                                         // Teredo encapsulates IPv6 traffic into UDP packets, parse out the bytes in the payload into packets.
                                                                         // If it contains a IPV6 packet, it to this current packet as a payload.
                                                                         // https://tools.ietf.org/html/rfc4380#section-5.1.1
                                                                         if ((destinationPort == teredoPort) || (sourcePort == teredoPort))
                                                                             if (ContainsIPv6Packet(payload))
                                                                             {
                                                                                 result.Packet = new IPv6Packet(payload);
                                                                                 return result;
                                                                             }

                                                                         // store the payload bytes
                                                                         result.ByteArraySegment = payload;
                                                                         return result;
                                                                     },
                                                                     LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        /// A <see cref="Packet" />
        /// </param>
        public UdpPacket
        (
            ByteArraySegment byteArraySegment,
            Packet parentPacket) :
            this(byteArraySegment)
        {
            ParentPacket = parentPacket;
        }

        /// <summary>Fetch the header checksum.</summary>
        public override ushort Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + UdpFields.ChecksumPosition);
            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + UdpFields.ChecksumPosition);
            }
        }

        /// <summary>Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override string Color => AnsiEscapeSequences.LightGreen;

        /// <summary>Fetch the port number on the target host.</summary>
        public override ushort DestinationPort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + UdpFields.DestinationPortPosition);
            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + UdpFields.DestinationPortPosition);
            }
        }

        /// <value>
        /// Length in bytes of the header and payload, minimum size of 8,
        /// the size of the Udp header
        /// </value>
        public int Length
        {
            get => EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                  Header.Offset + UdpFields.HeaderLengthPosition);
            internal set
            {
                // Internal because it is updated based on the payload when its bytes are retrieved.
                var val = (short) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + UdpFields.HeaderLengthPosition);
            }
        }

        /// <summary>Fetch the port number on the source host.</summary>
        public override ushort SourcePort
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + UdpFields.SourcePortPosition);
            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, Header.Bytes, Header.Offset + UdpFields.SourcePortPosition);
            }
        }

        /// <summary>Check if the UDP packet is valid, checksum-wise.</summary>
        public bool ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (ParentPacket is IPv6Packet)
                    return ValidUdpChecksum;
                // For IPv4 both the IP layer and the TCP layer contain checksums


                return ((IPv4Packet) ParentPacket).ValidIPChecksum && ValidUdpChecksum;
            }
        }

        /// <value>
        /// True if the UDP checksum is valid
        /// </value>
        public bool ValidUdpChecksum
        {
            get
            {
                Log.Debug("ValidUdpChecksum");
                var result = IsValidChecksum(TransportChecksumOption.IncludePseudoIPHeader);
                Log.DebugFormat("ValidUdpChecksum {0}", result);
                return result;
            }
        }

        /// <summary>
        /// Update the Udp length
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            // update the length field based on the length of this packet header
            // plus the length of all of the packets it contains
            Length = TotalPacketLength;
        }

        /// <summary>
        /// Calculates the UDP checksum, optionally updating the UDP checksum header.
        /// </summary>
        /// <returns>The calculated UDP checksum.</returns>
        public ushort CalculateUdpChecksum()
        {
            return (ushort) CalculateChecksum(TransportChecksumOption.IncludePseudoIPHeader);
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateUdpChecksum()
        {
            Checksum = CalculateUdpChecksum();
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

            if ((outputFormat == StringOutputType.Colored) || (outputFormat == StringOutputType.VerboseColored))
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if ((outputFormat == StringOutputType.Normal) || (outputFormat == StringOutputType.Colored))
                buffer.AppendFormat("{0}[UDPPacket: SourcePort={2}, DestinationPort={3}]{1}",
                                    color,
                                    colorEscape,
                                    SourcePort,
                                    DestinationPort);

            if ((outputFormat == StringOutputType.Verbose) || (outputFormat == StringOutputType.VerboseColored))
            {
                // collect the properties and their value
                var properties = new Dictionary<string, string>
                {
                    { "source", SourcePort.ToString() },
                    { "destination", DestinationPort.ToString() },
                    { "length", Length.ToString() },
                    { "checksum", "0x" + Checksum.ToString("x") + " [" + (ValidUdpChecksum ? "valid" : "invalid") + "]" }
                };

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("UDP:  ******* UDP - \"User Datagram Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("UDP:");
                foreach (var property in properties)
                    buffer.AppendLine("UDP: " + property.Key.PadLeft(padLength) + " = " + property.Value);

                buffer.AppendLine("UDP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="UdpPacket" />
        /// </returns>
        public static UdpPacket RandomPacket()
        {
            var rnd = new Random();
            var sourcePort = (ushort) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var destinationPort = (ushort) rnd.Next(UInt16.MinValue, UInt16.MaxValue);

            return new UdpPacket(sourcePort, destinationPort);
        }

        /// <summary>
        /// Determines whether the specified byte array segment contains an IPv6 packet.
        /// </summary>
        /// <param name="packetBytes">The packet bytes.</param>
        /// <returns>
        /// <c>true</c> if it contains an IPv6 packet; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsIPv6Packet(ByteArraySegment packetBytes)
        {
            // Packet bytes must be greater than or equal to the IPV6 header length, start with the version number, 
            // and be greater in length than the payload length + the header length.
            return (packetBytes.Length >= IPv6Fields.HeaderLength) &&
                   (packetBytes.Bytes[packetBytes.Offset] >> 4 == (int) RawIPPacketProtocol.IPv6) &&
                   (packetBytes.Length >= IPv6Fields.HeaderLength + packetBytes.Bytes[packetBytes.Offset + IPv6Fields.PayloadLengthPosition]);
        }
    }
}