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
        /// <summary> Fetch the port number on the source host.</summary>
        virtual public int SourcePort
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes, header.Offset + UdpFields.SourcePortPosition);
            }

            set
            {
                var val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + UdpFields.SourcePortPosition);
            }
        }

        /// <summary> Fetch the port number on the target host.</summary>
        virtual public int DestinationPort
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + UdpFields.DestinationPortPosition);
            }

            set
            {
                var val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + UdpFields.DestinationPortPosition);
            }
        }

        /// <value>
        /// Retrieves the length field from this udp packet. Note that this is not
        /// necessarily the correct length 
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
        virtual public int UDPChecksum
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + UdpFields.ChecksumPosition);
            }

            set
            {
                var val = (Int16)value;
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

        /// <summary> Fetch the header checksum.</summary>
        public int Checksum
        {
            get
            {
                return UDPChecksum;
            }
            set
            {
                UDPChecksum = value;
            }
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Computes the UDP checksum, optionally updating the UDP checksum header.
        /// </summary>
        /// <param name="update">Specifies whether or not to update the UDP checksum header
        /// after computing the checksum. A value of true indicates the
        /// header should be updated, a value of false indicates it should
        /// not be updated.
        /// </param>
        /// <returns> The computed UDP checksum.
        /// </returns>
        public int ComputeUDPChecksum(bool update)
        {
            // make sure that the parent packet is the correct type
            if(!(ParentPacket is IpPacket))
            {
                throw new System.NotImplementedException("ParentPacket is not IpPacket, cannot compute udp checksum for parent packet");
            }

            var ipPacket = ParentPacket as IpPacket;

            // zero out the checksum field, we don't want its
            // value to affect the checksum calculation itself
            Checksum = 0;

            var dataAndPseudoIpHeader = ipPacket.AttachPseudoIPHeader(Bytes);

            // compute the one's complement sum of the udp header
            int cs = ChecksumUtils.OnesComplementSum(dataAndPseudoIpHeader);
            if (update)
            {
                UDPChecksum = cs;
            }

            return cs;
        }

        public int ComputeUDPChecksum()
        {
            return ComputeUDPChecksum(true);
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
    }
}
