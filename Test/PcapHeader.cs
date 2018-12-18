using PacketDotNet;
using System;
using System.IO;
namespace Test
{
    public class PcapHeader
    {
        public ushort MajorVersion { get; } = 0;
        public ushort MinorVersion { get; } = 0;
        public uint ThisZone { get; } = 0;
        public uint SigFigs { get; } = 0;
        public uint SnapLen { get; } = 0;
        public LinkLayers LinkType { get; } = 0;

        public PcapHeader(BinaryReader readBuffer)
        {
            byte[] Buffer = new byte[20];
            readBuffer.Read(Buffer, 0, 20);

            MajorVersion = BitConverter.ToUInt16(Buffer, 0);
            MinorVersion = BitConverter.ToUInt16(Buffer, 2);
            ThisZone = BitConverter.ToUInt32(Buffer, 4);
            SigFigs = BitConverter.ToUInt32(Buffer, 8);
            SnapLen = BitConverter.ToUInt32(Buffer, 12);
            LinkType = (LinkLayers)BitConverter.ToUInt32(Buffer, 16);
        }
    }
}
