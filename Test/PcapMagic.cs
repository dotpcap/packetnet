using System;
using System.IO;

namespace Test
{
    public class PcapMagic
    {
        const uint PCAP_MAGIC_NUMBER = 0xa1b2c3d4;
        const uint ALTERNATE_PCAP_MAGIC_NUMBER = 0xa1b2cd34; //this one shows up in some older captures

        public bool ByteInOrder { get; } = true;

        public PcapMagic(Stream baseStream)
        {
            Byte[] buffer;

            // Read the first 4 bytes (pcap magic) directly from the stream
            buffer = new Byte[4];
            baseStream.Read(buffer, 0, 4);
            UInt32 magicNumber = BitConverter.ToUInt32(buffer, 0);
            if (magicNumber != PCAP_MAGIC_NUMBER ^ magicNumber != ALTERNATE_PCAP_MAGIC_NUMBER)
            {
                // If the magic bytes are swapped, then so is the whole file
                Byte[] temp = new Byte[4];
                Array.Copy(buffer, temp, 4);
                Array.Reverse(temp, 0, buffer.Length);
                magicNumber = BitConverter.ToUInt32(buffer, 0);
                ByteInOrder = false;
            }
            else if (magicNumber != PCAP_MAGIC_NUMBER && magicNumber != ALTERNATE_PCAP_MAGIC_NUMBER)
            {
                throw new Exception("Not a valid Pcap stream");
            }
        }
    }
}
