using PacketDotNet;
using System;
using System.IO;
namespace Test
{
    public class PcapHeader
    {
        public ushort majorVersion = 0;
        public ushort minorVersion = 0;
        public uint thisZone = 0;
        public uint sigFigs = 0;
        public uint snapLen = 0;
        public LinkLayers linkType = 0;

        public byte[] buffer;

        public PcapHeader(BinaryReader readBuffer)
        {
            buffer = new byte[20];

            readBuffer.Read(buffer, 0, 20);

            this.majorVersion = BitConverter.ToUInt16(buffer, 0);
            this.minorVersion = BitConverter.ToUInt16(buffer, 2);
            this.thisZone = BitConverter.ToUInt32(buffer, 4);
            this.sigFigs = BitConverter.ToUInt32(buffer, 8);
            this.snapLen = BitConverter.ToUInt32(buffer, 12);
            this.linkType = (LinkLayers)BitConverter.ToUInt32(buffer, 16);
        }


    }
}
