using System;
using SharpDotNet;
using System.Collections.Generic;
using Packet.Net.TransportLayer;

namespace Packet.Net
{
    /// <summary>
    /// Represents a Layer 4 protocol
    /// </summary>
    public abstract class TransportPacket : Packet
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

        private static Dictionary<TransportProtocols, PacketParser> parsers;

        static TransportPacket()
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
    }
}
