using System;
using PacketDotNet;

namespace Test
{
    public class SharpPcapRawPacketToPacket
    {
        public static Packet RawPacketToPacket(SharpPcap.Packets.RawPacket rawPacket)
        {
            Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                          new PosixTimeval(rawPacket.Timeval.Seconds,
                                                           rawPacket.Timeval.MicroSeconds),
                                          rawPacket.Data);

            return p;
        }
    }
}
