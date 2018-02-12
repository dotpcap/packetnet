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
using System.Text;
using PacketDotNet.Interfaces;
using PacketDotNet.IP;
using PacketDotNet.L2TP;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Udp
{
    /// <summary>
    ///     User datagram protocol
    ///     See http://en.wikipedia.org/wiki/Udp
    /// </summary>
    [Serializable]
    public class UdpPacket : TransportPacket, ISourceDestinationPort
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

        /// <summary> Fetch the port number on the source host.</summary>
        public virtual UInt16 SourcePort
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + UdpFields.SourcePortPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + UdpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetch the port number on the target host.</summary>
        public virtual UInt16 DestinationPort
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + UdpFields.DestinationPortPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + UdpFields.DestinationPortPosition);
            }
        }

        /// <value>
        ///     Length in bytes of the header and payload, minimum size of 8,
        ///     the size of the Udp header
        /// </value>
        public virtual Int32 Length
        {
            get => EndianBitConverter.Big.ToInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + UdpFields.HeaderLengthPosition);

            // Internal because it is updated based on the payload when
            // its bytes are retrieved
            internal set
            {
                var val = (Int16) value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + UdpFields.HeaderLengthPosition);
            }
        }

        /// <summary> Fetch the header checksum.</summary>
        public override UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + UdpFields.ChecksumPosition);

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + UdpFields.ChecksumPosition);
            }
        }

        /// <summary> Check if the UDP packet is valid, checksum-wise.</summary>
        public Boolean ValidChecksum
        {
            get
            {
                // IPv6 has no checksum so only the TCP checksum needs evaluation
                if (this.ParentPacket.GetType() == typeof(IPv6Packet))
                    return this.ValidUDPChecksum;
                // For IPv4 both the IP layer and the TCP layer contain checksums
                return ((IPv4Packet) this.ParentPacket).ValidIPChecksum && this.ValidUDPChecksum;
            }
        }

        /// <value>
        ///     True if the udp checksum is valid
        /// </value>
        public virtual Boolean ValidUDPChecksum
        {
            get
            {
                Log.Debug("ValidUDPChecksum");
                var retval = this.IsValidChecksum(TransportChecksumOption.AttachPseudoIPHeader);
                Log.DebugFormat("ValidUDPChecksum {0}", retval);
                return retval;
            }
        }

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        public override String Color => AnsiEscapeSequences.LightGreen;

        /// <summary>
        ///     Update the Udp length
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            // update the length field based on the length of this packet header
            // plus the length of all of the packets it contains
            this.Length = this.TotalPacketLength;
        }

        /// <summary>
        ///     Create from values
        /// </summary>
        /// <param name="sourcePort">
        ///     A <see cref="System.UInt16" />
        /// </param>
        /// <param name="destinationPort">
        ///     A <see cref="System.UInt16" />
        /// </param>
        public UdpPacket(UInt16 sourcePort, UInt16 destinationPort)
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = UdpFields.HeaderLength;
            var headerBytes = new Byte[length];
            this.HeaderByteArraySegment = new ByteArraySegment(headerBytes, offset, length);

            // set instance values
            this.SourcePort = sourcePort;
            this.DestinationPort = destinationPort;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public UdpPacket(ByteArraySegment bas)
        {
            Log.DebugFormat("bas {0}", bas.ToString());

            // set the header field, header field values are retrieved from this byte array
            this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = UdpFields.HeaderLength
            };

            this.PayloadPacketOrData = new PacketOrByteArraySegment();

            // is this packet going to port 7 or 9? if so it might be a WakeOnLan packet
            const Int32 wakeOnLanPort0 = 7;
            const Int32 wakeOnLanPort1 = 9;
            if (this.DestinationPort.Equals(wakeOnLanPort0) || this.DestinationPort.Equals(wakeOnLanPort1))
            {
                this.PayloadPacketOrData.ThePacket =
                    new WakeOnLanPacket(this.HeaderByteArraySegment.EncapsulatedBytes());
            }
            else
            {
                // store the payload bytes
                this.PayloadPacketOrData.TheByteArraySegment = this.HeaderByteArraySegment.EncapsulatedBytes();
            }

            const Int32 l2TPport = 1701;
            if (this.DestinationPort.Equals(l2TPport) && this.DestinationPort.Equals(l2TPport))
            {
                var payload = this.HeaderByteArraySegment.EncapsulatedBytes();
                this.PayloadPacketOrData.ThePacket = new L2TPPacket(payload, this);
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        /// <param name="parentPacket">
        ///     A <see cref="Packet" />
        /// </param>
        public UdpPacket(ByteArraySegment bas,
            Packet parentPacket) :
            this(bas)
        {
            this.ParentPacket = parentPacket;
        }

        /// <summary>
        ///     Calculates the UDP checksum, optionally updating the UDP checksum header.
        /// </summary>
        /// <returns>The calculated UDP checksum.</returns>
        public Int32 CalculateUDPChecksum()
        {
            var newChecksum = this.CalculateChecksum(TransportChecksumOption.AttachPseudoIPHeader);
            return newChecksum;
        }

        /// <summary>
        ///     Update the checksum value.
        /// </summary>
        public void UpdateUDPChecksum()
        {
            this.Checksum = (UInt16) this.CalculateUDPChecksum();
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
                    buffer.AppendFormat("{0}[UDPPacket: SourcePort={2}, DestinationPort={3}]{1}",
                        color,
                        colorEscape, this.SourcePort, this.DestinationPort);
                    break;
                case StringOutputType.Verbose:
                case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>
                    {
                        {"source", this.SourcePort.ToString()},
                        {"destination", this.DestinationPort.ToString()},
                        {"length", this.Length.ToString()},
                        {
                            "checksum",
                            "0x" + this.Checksum.ToString("x") + " [" + (this.ValidUDPChecksum ? "valid" : "invalid") +
                            "]"
                        }
                    };

                    // calculate the padding needed to right-justify the property names
                    Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                    // build the output string
                    buffer.AppendLine("UDP:  ******* UDP - \"User Datagram Protocol\" - offset=? length=" +
                                      this.TotalPacketLength);
                    buffer.AppendLine("UDP:");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine("UDP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                    }

                    buffer.AppendLine("UDP:");
                    break;
            }

            // append the base string output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        ///     Generate a random packet
        /// </summary>
        /// <returns>
        ///     A <see cref="UdpPacket" />
        /// </returns>
        public static UdpPacket RandomPacket()
        {
            var rnd = new Random();
            var sourcePort = (UInt16) rnd.Next(UInt16.MinValue, UInt16.MaxValue);
            var destinationPort = (UInt16) rnd.Next(UInt16.MinValue, UInt16.MaxValue);

            return new UdpPacket(sourcePort, destinationPort);
        }
    }
}