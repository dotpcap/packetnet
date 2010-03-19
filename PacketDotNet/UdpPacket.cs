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
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
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
        virtual public ushort Checksum
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
            : base(new PosixTimeval())
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = UdpFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArrayAndOffset(headerBytes, offset, length);

            // set instance values
            this.SourcePort = SourcePort;
            this.DestinationPort = DestinationPort;
        }

        /// <summary>
        /// byte[]/int offset constructor, timeval defaults to the current time
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public UdpPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        { }

        /// <summary>
        /// byte[]/int offset/PosixTimeval constructor
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
        public UdpPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            // set the header field, header field values are retrieved from this byte array
            header = new ByteArrayAndOffset(Bytes, Offset, UdpFields.HeaderLength);

            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArray();
            payloadPacketOrData.TheByteArray = header.EncapsulatedBytes();
        }

        /// <summary>
        /// Constructor when this packet is encapsulated in another packet
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
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public UdpPacket(byte[] Bytes, int Offset, PosixTimeval Timeval,
                         Packet ParentPacket) :
            this(Bytes, Offset, Timeval)
        {
            this.ParentPacket = ParentPacket;
        }

        /// <summary>
        /// Calculates the UDP checksum, optionally updating the UDP checksum header.
        /// </summary>
        /// <returns>The calculated UDP checksum.</returns>
        public int CalculateUDPChecksum()
        {
            // make sure that the parent packet is the correct type
            if(!(ParentPacket is IpPacket))
            {
                throw new System.NotImplementedException("ParentPacket is not IpPacket, cannot calculate udp checksum for parent packet");
            }

            var ipPacket = ParentPacket as IpPacket;

            // zero out the checksum field, we don't want its
            // value to affect the checksum calculation itself
            Checksum = 0;

            var dataAndPseudoIpHeader = ipPacket.AttachPseudoIPHeader(Bytes);

            // calculate the one's complement sum of the udp header
            int cs = ChecksumUtils.OnesComplementSum(dataAndPseudoIpHeader);

            return cs;
        }

        /// <summary>
        /// Update the checksum value.
        /// </summary>
        public void UpdateUDPChecksum()
        {
            this.Checksum = (ushort)CalculateUDPChecksum();
        }

        /// <summary> Convert this UDP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this UDP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("UDPPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.Append(": ");
            if(Enum.IsDefined(typeof(IpPort), SourcePort))
            {
                buffer.Append((IpPort)SourcePort);
            } else
            {
                buffer.Append(SourcePort);
            }
            buffer.Append(" -> ");
            if(Enum.IsDefined(typeof(IpPort), DestinationPort))
            {
                buffer.Append((IpPort)DestinationPort);
            } else
            {
                buffer.Append(DestinationPort);
            }
            buffer.Append(" l=" + UdpFields.HeaderLengthLength + "," + (Length - UdpFields.HeaderLengthLength));
            buffer.Append(']');

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
        public static UdpPacket GetType(Packet p)
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
