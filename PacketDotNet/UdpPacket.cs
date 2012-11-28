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
﻿using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// User datagram protocol
    /// See http://en.wikipedia.org/wiki/Udp
    /// </summary>
    public class UdpPacket : TransportPacket
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

        /// <summary> Fetch the port number on the source host.</summary>
        virtual public ushort SourcePort
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + UdpFields.SourcePortPosition);
            }

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + UdpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetch the port number on the target host.</summary>
        virtual public ushort DestinationPort
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset + UdpFields.DestinationPortPosition);
            }

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + UdpFields.DestinationPortPosition);
            }
        }

        /// <value>
        /// Length in bytes of the header and payload, minimum size of 8,
        /// the size of the Udp header
        /// </value>
        virtual public int Length
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + UdpFields.HeaderLengthPosition);
            }

            // Internal because it is updated based on the payload when
            // its bytes are retrieved
            internal set
            {
                var val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + UdpFields.HeaderLengthPosition);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        override public ushort Checksum
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                       header.Offset + UdpFields.ChecksumPosition);
            }

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + UdpFields.ChecksumPosition);
            }
        }

        /// <summary> Check if the UDP packet is valid, checksum-wise.</summary>
        public bool ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (ParentPacket.GetType() == typeof(IPv6Packet))
                    return ValidUDPChecksum;
                // For IPv4 both the IP layer and the TCP layer contain checksums
                else
                    return ((IPv4Packet)ParentPacket).ValidIPChecksum && ValidUDPChecksum;
            }
        }

        /// <value>
        /// True if the udp checksum is valid
        /// </value>
        virtual public bool ValidUDPChecksum
        {
            get
            {
                log.Debug("ValidUDPChecksum");
                var retval = IsValidChecksum(TransportPacket.TransportChecksumOption.AttachPseudoIPHeader);
                log.DebugFormat("ValidUDPChecksum {0}", retval);
                return retval;
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.LightGreen;
            }
        }

        /// <summary>
        /// Update the Udp length
        /// </summary>
        public override void UpdateCalculatedValues ()
        {
            // update the length field based on the length of this packet header
            // plus the length of all of the packets it contains
            Length = TotalPacketLength;
        }

        /// <summary>
        /// Create from values
        /// </summary>
        /// <param name="SourcePort">
        /// A <see cref="System.UInt16"/>
        /// </param>
        /// <param name="DestinationPort">
        /// A <see cref="System.UInt16"/>
        /// </param>
        public UdpPacket(ushort SourcePort, ushort DestinationPort)
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = UdpFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set instance values
            this.SourcePort = SourcePort;
            this.DestinationPort = DestinationPort;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public UdpPacket(ByteArraySegment bas)
        {
            log.DebugFormat("bas {0}", bas.ToString());

            // set the header field, header field values are retrieved from this byte array
            header = new ByteArraySegment(bas);
            header.Length = UdpFields.HeaderLength;

            payloadPacketOrData = new PacketOrByteArraySegment();

            // is this packet going to port 7 or 9? if so it might be a WakeOnLan packet
            const int wakeOnLanPort0 = 7;
            const int wakeOnLanPort1 = 9;
            if(DestinationPort.Equals(wakeOnLanPort0) || DestinationPort.Equals (wakeOnLanPort1))
            {
                payloadPacketOrData.ThePacket = new WakeOnLanPacket(header.EncapsulatedBytes());
            } else
            {
                // store the payload bytes
                payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public UdpPacket(ByteArraySegment bas,
                         Packet ParentPacket) :
            this(bas)
        {
            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// Calculates the UDP checksum, optionally updating the UDP checksum header.
        /// </summary>
        /// <returns>The calculated UDP checksum.</returns>
        public int CalculateUDPChecksum()
        {
            var newChecksum = CalculateChecksum(TransportChecksumOption.AttachPseudoIPHeader);
            return newChecksum;
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateUDPChecksum()
        {
            this.Checksum = (ushort)CalculateUDPChecksum();
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
                buffer.AppendFormat("{0}[UDPPacket: SourcePort={2}, DestinationPort={3}]{1}",
                color,
                colorEscape,
                SourcePort,
                DestinationPort);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("source", SourcePort.ToString());
                properties.Add("destination", DestinationPort.ToString());
                properties.Add("length", Length.ToString());
                properties.Add("checksum", "0x" + Checksum.ToString("x") + " [" + (ValidUDPChecksum ? "valid" : "invalid") + "]");

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("UDP:  ******* UDP - \"User Datagram Protocol\" - offset=? length=" + TotalPacketLength);
                buffer.AppendLine("UDP:");
                foreach(var property in properties)
                {
                    buffer.AppendLine("UDP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }
                buffer.AppendLine("UDP:");
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Returns the UdpPacket inside of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="UdpPacket"/>
        /// </returns>
        [Obsolete("Use Packet.Extract() instead")]
        public static UdpPacket GetEncapsulated(Packet p)
        {
            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    var innerPayload = payload.PayloadPacket;
                    if(innerPayload is UdpPacket)
                    {
                        return (UdpPacket)innerPayload;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Generate a random packet
        /// </summary>
        /// <returns>
        /// A <see cref="UdpPacket"/>
        /// </returns>
        public static UdpPacket RandomPacket()
        {
            var rnd = new Random();
            var SourcePort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
            var DestinationPort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

            return new UdpPacket(SourcePort, DestinationPort);
        }
    }
}
