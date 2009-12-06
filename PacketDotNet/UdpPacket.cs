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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
﻿using System;
using Packet.Net.Utils;

namespace Packet.Net
{
    public class UdpPacket : TransportPacket
    {
#if false
        /// <summary> Fetch the port number on the source host.</summary>
        virtual public int SourcePort
        {
            get
            {
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + UDPFields_Fields.UDP_SP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

        }

        /// <summary> Fetch the port number on the target host.</summary>
        virtual public int DestinationPort
        {
            get
            {
                
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + UDPFields_Fields.UDP_DP_POS, UDPFields_Fields.UDP_PORT_LEN);
            }
        }
        /// <summary> Fetch the total length of the UDP packet, including header and
        /// data payload, in bytes.
        /// </summary>
        virtual public int UDPLength
        {
            get
            {
                // should produce the same value as header.length + data.length
                return Length;
            }

            set
            {
                ArrayHelper.insertLong(Bytes, value, _ipPayloadOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
            }

        }

        /// <summary> Fetch the header checksum.</summary>
        virtual public int UDPChecksum
        {
            get
            {
                return GetTransportLayerChecksum(_ipPayloadOffset + UDPFields_Fields.UDP_CSUM_POS);
            }

            set
            {
                SetTransportLayerChecksum(value, UDPFields_Fields.UDP_CSUM_POS);
            }

        }

        /// <summary> Fetch the UDP header a byte array.</summary>
        virtual public byte[] UDPHeader
        {
            get
            {
                if (_udpHeaderBytes == null)
                {
                    _udpHeaderBytes = PacketEncoding.extractHeader(_ipPayloadOffset, UDPFields_Fields.UDP_HEADER_LEN, Bytes);
                }
                return _udpHeaderBytes;
            }

        }

        /// <summary> Fetch the UDP header length in bytes.</summary>
        virtual public int UDPHeaderLength
        {
            get
            {
                return UDPFields_Fields.UDP_HEADER_LEN;
            }
        }

        /// <summary> Fetches the length of the payload data.</summary>
        virtual public int PayloadDataLength
        {
            get
            {
                return (IPPayloadLength - UDPHeaderLength);
            }
        }

        /// <summary> Fetch the UDP data as a byte array.</summary>
        virtual public byte[] UDPData
        {
            get
            {
                if (_udpDataBytes == null)
                {
                    _udpDataBytes = new byte[PayloadDataLength];
                    Array.Copy(Bytes, _ipPayloadOffset + UDPHeaderLength, _udpDataBytes, 0, PayloadDataLength);
                }
                return _udpDataBytes;
            }
            set
            {
                SetData(value);
            }

        }
#endif

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.LIGHT_GREEN;
            }
        }

#if false
        /// <summary> Fetch the total length of the UDP packet, including header and
        /// data payload, in bytes.
        /// </summary>
        public int Length
        {
            get
            {
                // should produce the same value as header.length + data.length
                return ArrayHelper.extractInteger(Bytes, _ipPayloadOffset + UDPFields_Fields.UDP_LEN_POS, UDPFields_Fields.UDP_LEN_LEN);
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
                UDPChecksum=value;
            }
        }

        /// <summary> Computes the UDP checksum, optionally updating the UDP checksum header.
        /// 
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
            if (IPVersion != IPVersions.IPv4)
                throw new System.NotImplementedException("IPVersion of " + IPVersion + " is unrecognized");

            // copy the udp section with data
            byte[] udp = IPData;
            // reset the checksum field (checksum is calculated when this field is
            // zeroed)
            ArrayHelper.insertLong(udp, 0, UDPFields_Fields.UDP_CSUM_POS, UDPFields_Fields.UDP_CSUM_LEN);
            //pseudo ip header should be attached to the udp+data
            udp = AttachPseudoIPHeader(udp);
            // compute the one's complement sum of the udp header
            int cs = ChecksumUtils.OnesComplementSum(udp);
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

        private byte[] _udpHeaderBytes = null;

        private byte[] _udpDataBytes = null;
#endif

#if false        
        /// <summary> Fetch the UDP data as a byte array.</summary>
        public override byte[] Data
        {
            get
            {
                return UDPData;
            }
        }
#endif

        /// <summary> Convert this UDP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

#if false
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
                buffer.Append(AnsiEscapeSequences.RESET);
            buffer.Append(": ");
            buffer.Append(SourceAddress);
            buffer.Append('.');
            buffer.Append(IPPort.getName(SourcePort));
            buffer.Append(" -> ");
            buffer.Append(DestinationAddress);
            buffer.Append('.');
            buffer.Append(IPPort.getName(DestinationPort));
            buffer.Append(" l=" + UDPFields_Fields.UDP_HEADER_LEN + "," + (Length - UDPFields_Fields.UDP_HEADER_LEN));
            buffer.Append(']');

            return buffer.ToString();
        }
#endif

        new internal static Packet Parse(byte[] bytes)
        {
            byte[] header = new byte[8];
            byte[] payload = new byte[bytes.Length - 8];

            Array.Copy(bytes, header, 8);
            Array.Copy(bytes, 8, payload, 0, payload.Length);

            UdpPacket udp_packet = new UdpPacket();
            throw new System.NotImplementedException();
//            udp_packet.Header = header;
//            udp_packet.PayloadData = payload;

            // I didn't go any deeper because this a basic parsing

            return udp_packet;
        }
    }
}
