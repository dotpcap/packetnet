using System;

namespace Packet.Net
{
    public abstract class Packet
    {
        private byte[] header;
        private byte[] payload_data;
        private Packet parent_packet, payload_packet;

        // A EthernetPacket may be tunneled through a TCP or UDP Packet
        /// <summary>
        /// The packet that is carrying this one
        /// </summary>
        public Packet ParentPacket
        {
            get { return parent_packet; }
        }

        // I'm not sure if Layer 7 protocols can carry other protocols
        /// <summary>
        /// Set the Packet that this packet will carry
        /// </summary>
        public Packet PayloadPacket
        {
            get { return payload_packet; }
            set
            {
                if (payload_packet == value)
                    throw new InvalidOperationException("A packet cannot have itself as its payload.");

                payload_packet = value;
                payload_packet.SetParentPacket(this);
            }
        }

        public byte[] Header
        {
            get { return this.header; }
        }

        /// <summary>
        /// The encapsulated data, this may be data sent by a program, or another protocol
        /// </summary>
        // The reason that this property is here, is that an Ethernet Packet may be sent by a switch, which usually has no IP address
        // A router running OSPF won't use TCP or UDP since it's a layer 4 protocol, I don't remember if OSPF updates are broadcasts or not.
        public byte[] PayloadData
        {
            get { return payload_data; }
            set { payload_data = value; }
        }

        /// <summary>
        /// The packet and the packets its carrying as an array of bytes
        /// </summary>
        public abstract byte[] Bytes
        {
            get;
        }

        /// <summary>
        /// Turns an array of bytes into a packet
        /// </summary>
        /// <param name="data">The packets caught</param>
        /// <returns>An ethernet packet which has references to the higher protocols</returns>
        public static Packet Parse(byte[] data)
        {
            return EthernetPacket.Parse(data);
        }

        /// <summary>
        /// Sets the parent packet
        /// </summary>
        /// <param name="packet">The parent packet</param>
        internal void SetParentPacket(Packet packet)
        {
            this.parent_packet = packet;
        }

        internal void SetPacketHeader(byte[] header)
        {
            this.header = header;
        }
    }
}
