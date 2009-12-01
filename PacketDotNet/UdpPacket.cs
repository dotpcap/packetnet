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

namespace Packet.Net
{
    public class UdpPacket : TransportPacket
    {
        private ushort packet_length, checksum;

        public override byte[] Bytes
        {
            get { throw new NotImplementedException(); }
        }

        public ushort SourcePort
        {
            get
            {
                if (this.source_port == 0)
                {
                    this.SourcePort = BitConverter.ToUInt16(new byte[] { this.Header[1], this.Header[0] }, 0);
                }
                return this.source_port;
            }
            set
            {
                this.source_port = value;
            }
        }

        public ushort DestinationPort
        {
            get
            {
                if (this.destination_port == 0)
                {
                    this.DestinationPort = BitConverter.ToUInt16(new byte[] { this.Header[3], this.Header[2] }, 0);
                }
                return this.destination_port;
            }
            set
            {
                this.destination_port = value;
            }
        }

        public ushort PacketLength
        {
            get
            {
                if (this.packet_length == 0)
                {
                    this.PacketLength = (ushort)this.PayloadData.Length;
                }
                return this.packet_length; 
            }
            set
            {
                this.packet_length = value; 
            }
        }

        public ushort Checksum
        {
            get
            {
                if (this.checksum == 0)
                {
                    this.Checksum = BitConverter.ToUInt16(new byte[] { this.Header[7], this.Header[6] }, 0);
                }
                return this.checksum;
            }
            set
            {
                this.checksum = value;
            }
        }

        new internal static Packet Parse(byte[] bytes)
        {
            byte[] header = new byte[8];
            byte[] payload = new byte[bytes.Length - 8];

            Array.Copy(bytes, header, 8);
            Array.Copy(bytes, 8, payload, 0, payload.Length);

            UdpPacket udp_packet = new UdpPacket();
            udp_packet.SetPacketHeader(header);
            udp_packet.PayloadData = payload;

            // I didn't go any deeper because this a basic parsing

            return udp_packet;
        }
    }
}
