using System.IO;
namespace Test
{
    public class PcapStream
    {
        private BinaryReader _buffer;
        private PcapMagic _pcapMagic;
        private PcapHeader _pcapHeader;

        public void Initialize(Stream inputStream)
        {
            _pcapMagic = new PcapMagic(inputStream);

            if (_pcapMagic.ByteInOrder)
                _buffer = new BinaryReader(inputStream);
            else
                _buffer = new ByteSwappedBinaryReader(inputStream);

            _pcapHeader = new PcapHeader(_buffer);
        }

        public PcapPacket GetPacket()
        {
            return PcapPacket.GetPacket(_buffer, _pcapHeader.LinkType);
        }
    }
}
