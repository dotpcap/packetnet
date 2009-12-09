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
 * Copyright 2009 David Bond <mokon@mokon.net>
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.IO;
using MiscUtil.Conversion;
using Packet.Net.Utils;

namespace Packet.Net
{
    /// <summary>
    /// IPv6 packet
    /// 
    /// References
    /// ----------
    /// http://tools.ietf.org/html/rfc2460
    /// http://en.wikipedia.org/wiki/IPv6
    /// </summary>
    public class IPv6Packet : IpPacket
    {
        public const int HeaderMinimumLength = 40;

        public static int ipVersion = 6;

        private Int32 VersionTrafficClassFlowLabel
        {
            get
            {
                return EndianBitConverter.Big.ToInt32(header.Bytes,
                                                      header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value, header.Bytes, header.Offset + IPv6Fields.VersionTrafficClassFlowLabelPosition);
            }
        }

        /// <summary>
        /// The version field of the IPv6 Packet.
        /// </summary>
        public override int Version
        {
            get
            {
                return ((VersionTrafficClassFlowLabel >> 28) & 0xF);
            }

            set
            {
                // read the existing value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // mask the new field into place
                field = (UInt32)((field & 0x0FFFFFFF) | ((value << 28) & 0xF0000000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The traffic class field of the IPv6 Packet.
        /// </summary>
        public virtual int TrafficClass
        {
            get
            {
                return ((VersionTrafficClassFlowLabel >> 20) & 0xFF);
            }

            set
            {
                // read the original value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // mask in the new field
                field = (UInt32)(((field & 0xF00FFFFF) | (((UInt32)value) << 20 ) & 0x0FF00000));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The flow label field of the IPv6 Packet.
        /// </summary>
        public virtual int FlowLabel
        {
            get
            {
                return (VersionTrafficClassFlowLabel & 0xFFFFF);
            }

            set
            {
                // read the original value
                var field = (UInt32)VersionTrafficClassFlowLabel;

                // make the value in
                field = (UInt32)((field & 0xFFF00000) | ((UInt32)(value) & 0x000FFFFF));

                // write the updated value back
                VersionTrafficClassFlowLabel = (int)field;
            }
        }

        /// <summary>
        /// The payload lengeth field of the IPv6 Packet
        /// NOTE: Differs from the IPv4 'Total length' field that includes the length of the header as
        ///       payload length is ONLY the size of the payload.
        /// </summary>
        public int PayloadLength
        {
            get
            {
                return EndianBitConverter.Big.ToInt32(header.Bytes,
                                                      header.Offset + IPv6Fields.PayloadLengthPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + IPv6Fields.PayloadLengthPosition);
            }
        }

        /// <summary>
        /// The next header field of the IPv6 Packet.
        /// 
        /// Replaces IPv4's 'protocol' field, has compatible values
        /// </summary>
        public virtual IPProtocol.IPProtocolType NextHeader
        {
            get
            {
                return (IPProtocol.IPProtocolType)(header.Bytes[header.Offset + IPv6Fields.NextHeaderPosition]);
            }

            set
            {
                header.Bytes[header.Offset + IPv6Fields.NextHeaderPosition] = (byte)value;
            }
        }

        /// <summary>
        /// The hop limit field of the IPv6 Packet.
        /// NOTE: Replaces the 'time to live' field of IPv4
        /// 
        /// 8-bit value
        /// </summary>
        public virtual int HopLimit
        {
            get
            {
                return header.Bytes[header.Offset + IPv6Fields.HopLimitPosition];
            }

            set
            {
                header.Bytes[header.Offset + IPv6Fields.HopLimitPosition] = (byte)value;
            }
        }

        /// <summary>
        /// The source address field of the IPv6 Packet.
        /// </summary>
        public override System.Net.IPAddress SourceAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             header.Offset + IPv6Fields.SourceAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0,
                                  header.Bytes, header.Offset + IPv6Fields.SourceAddressPosition,
                                  address.Length);
            }
        }

        /// <summary>
        /// The destination address field of the IPv6 Packet.
        /// </summary>
        public override System.Net.IPAddress DestinationAddress
        {
            get
            {
                return IpPacket.GetIPAddress(System.Net.Sockets.AddressFamily.InterNetworkV6,
                                             header.Offset + IPv6Fields.DestinationAddressPosition,
                                             header.Bytes);
            }

            set
            {
                byte[] address = value.GetAddressBytes();
                System.Array.Copy(address, 0,
                                  header.Bytes, header.Offset + IPv6Fields.DestinationAddressPosition,
                                  address.Length);
            }
        }

#if false
        /// <summary>
        /// Returns the IP data.
        /// </summary>
        virtual public byte[] IPData
        {
            get
            {
                return PacketEncoding.extractData(_ethPayloadOffset,
                                                  IPv6Fields_Fields.IPv6_HEADER_LEN,
                                                  Bytes,
                                                  IPPayloadLength);
            }
            set
            {
                // retrieve the current payload length
                int currentIPPayloadLength = IPPayloadLength;

                // compute the difference between the current and the
                // requested payload lengths
                int changeInLength = value.Length - currentIPPayloadLength;

                // create a new buffer for the entire packet
                byte[] newByteArray = new Byte[Bytes.Length + changeInLength];

                // copy the old contents over to the new buffer, less
                // the payload
                Array.Copy(Bytes, newByteArray, Bytes.Length - currentIPPayloadLength);

                // copy the new payload into place
                Array.Copy(value, 0,
                           newByteArray, Bytes.Length - currentIPPayloadLength,
                           value.Length);

                // update the Bytes to the new byte array
                Bytes = newByteArray;

                // update the IP length
                IPPayloadLength = value.Length;
            }
        }
#endif

        // Prepend to the given byte[] origHeader the portion of the IPv6 header used for
        // generating an tcp checksum
        //
        // http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_checksum_using_IPv6
        // http://tools.ietf.org/html/rfc2460#page-27
        internal override byte[] AttachPseudoIPHeader(byte[] origHeader)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // 0-16: ip src addr
            bw.Write(header.Bytes, header.Offset + IPv6Fields.SourceAddressPosition,
                     IPv6Fields.AddressLength);

            // 17-32: ip dst addr
            bw.Write(header.Bytes, header.Offset + IPv6Fields.DestinationAddressPosition,
                     IPv6Fields.AddressLength);

            // 33-36: TCP length
            bw.Write((UInt32)System.Net.IPAddress.HostToNetworkOrder((UInt32)origHeader.Length));

            // 37-39: 3 bytes of zeros
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((byte)0);

            // 40: Next header
            bw.Write((byte)NextHeader);

            // prefix the pseudoHeader to the header+data
            byte[] pseudoHeader = ms.ToArray();
            int headerSize = pseudoHeader.Length + origHeader.Length; 
            bool odd = origHeader.Length % 2 != 0;
            if (odd)
                headerSize++;

            byte[] finalData = new byte[headerSize];

            // copy the pseudo header in
            Array.Copy(pseudoHeader, 0, finalData, 0, pseudoHeader.Length);

            // copy the origHeader in
            Array.Copy(origHeader, 0, finalData, pseudoHeader.Length, origHeader.Length);

            //if not even length, pad with a zero
            if (odd)
                finalData[finalData.Length - 1] = 0;

            return finalData;
        }

        /// <summary>
        /// Converts the packet to a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString( )
        {
            return base.ToString( ) + "\r\nIPv6 Packet [\r\n"
                   + "\tIPv6 Source Address: " + SourceAddress.ToString() + ", \r\n"
                   + "\tIPv6 Destination Address: " + DestinationAddress.ToString() + "\r\n"
                   + "]";
            // TODO Implement Better ToString
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
            buffer.Append("IPv6Packet");
            if (colored)
                buffer.Append(AnsiEscapeSequences.RESET);
            buffer.Append(": ");
            buffer.Append(SourceAddress + " -> " + DestinationAddress);
            buffer.Append(" next header=" + NextHeader);
            //FIXME: figure out what to use for the lengths
#if false
            buffer.Append(" l=" + this.IPPayloadLength);
            buffer.Append(" sum=" + this.IPPayloadLength);
#endif
            buffer.Append(']');

            // append the base class output
            buffer.Append(base.ToColoredString(colored));

            return buffer.ToString();
        }

        /// <summary> Convert this IP packet to a more verbose string.</summary>
        public override System.String ToColoredVerboseString(bool colored)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Converts the packet to a color string. TODO add a method for colored to string.
        /// </summary>
        override public String Color
        {
            get
            {
                return AnsiEscapeSequences.WHITE;
            }
        }
    }
}
