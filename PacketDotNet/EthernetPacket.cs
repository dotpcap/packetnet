using System;
using System.Net.NetworkInformation;
using Packet.Net.NetworkLayer;

namespace Packet.Net
{
    public class EthernetPacket : DataLinkPacket
    {
        private PhysicalAddress source_mac, dest_mac;

        public PhysicalAddress DestinationHwAddress
        {
            get
            {
                if (this.dest_mac == null)
                {
                    byte[] mac = new byte[6];
                    Array.Copy(this.Header, mac, 6);
                    this.dest_mac = new PhysicalAddress(mac);
                }
                return dest_mac;
            }
            set
            {
                dest_mac = value;
            }
        }

        public PhysicalAddress SourceHwAddress
        {
            get
            {
                if (this.dest_mac == null)
                {
                    byte[] mac = new byte[6];
                    Array.Copy(this.Header, 6, mac, 0, 6);
                    this.source_mac = new PhysicalAddress(mac);
                }
                return source_mac;
            }
            set
            {
                source_mac = value;
            }
        }

        public override byte[] Bytes
        {
            get 
            { 
                throw new NotImplementedException();
            }
        }

        new internal static Packet Parse(byte[] bytes)
        {
            byte[] header = new byte[14];
            byte[] payload = new byte[bytes.Length - 14];

            Array.Copy(bytes, header, 14);
            Array.Copy(bytes, 14, payload, 0, payload.Length);

            ushort ether_type = BitConverter.ToUInt16(new byte[] { header[13], header[12] }, 0);

            EthernetPacket eth_packet = new EthernetPacket();
            eth_packet.SetPacketHeader(header);

            eth_packet.PayloadData = payload;

            // This could be put in a try+catch block because there may be no Network Protocol encapsulated here
            eth_packet.PayloadPacket = (NetworkingPacket)NetworkingPacket.Parse((NetworkingProtocols)ether_type, payload);

            return eth_packet;
        }
    }
}
