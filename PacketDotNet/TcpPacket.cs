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
﻿using System;
using System.Collections.Generic;

namespace Packet.Net
{
    /// <summary>
    /// Represents a Layer 4 protocol
    /// </summary>
    public abstract class TcpPacket : Packet
    {
        protected ushort source_port, destination_port;

        public abstract ushort SourcePort
        {
            get;
            set;
        }

        public abstract ushort DestinationPort
        {
            get;
            set;
        }

        public abstract ushort Checksum
        {
            get;
            set;
        }

#if false
        private static Dictionary<TransportProtocols, PacketParser> parsers;

        static TcpPacket()
        {
            TransportPacket.parsers = new Dictionary<TransportProtocols, PacketParser>();
            TransportPacket.parsers.Add(TransportProtocols.Udp, new PacketParser(UdpPacket.Parse));
        }

        public static Packet Parse(TransportProtocols transport_protocol, byte[] data)
        {
            if (TransportPacket.parsers.ContainsKey(transport_protocol))
            {
                PacketParser parser = TransportPacket.parsers[transport_protocol];

                return parser(data);
            }
            else
            {
                throw new NotSupportedException("The specified trasnport protocol is not supported.");
            }
        }
#endif
    }
}
