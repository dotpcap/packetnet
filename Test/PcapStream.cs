using System.IO;
namespace Test
{
    public class PcapStream
    {
        BinaryReader buffer;
        public PcapMagic pcapMagic;
        public PcapHeader pcapHeader;

        public void Initialize(Stream inputStream)
        {
            pcapMagic = new PcapMagic(inputStream);

            if (pcapMagic.byteInOrder)
                buffer = new BinaryReader(inputStream);
            else
                buffer = new ByteSwappedBinaryReader(inputStream);

            pcapHeader = new PcapHeader(buffer);
        }

        public PcapPacket GetPacket()
        {
            return PcapPacket.GetPacket(buffer, pcapHeader.linkType);
        }

    }
}
