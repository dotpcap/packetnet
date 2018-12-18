using System;
using System.IO;

namespace Test
{
    public class PcapMagic
    {
        public const uint PcapMagicNumber = 0xa1b2c3d4;
        public const uint AlternatePcapMagicNumber = 0xa1b2cd34; //this one shows up in some older captures

        public Byte[] buffer;
        public bool byteInOrder = true;

        public PcapMagic(Stream baseStream)
        {
            // Read the first 4 bytes (pcap magic) directly from the stream
            buffer = new Byte[4];
            baseStream.Read(buffer, 0, 4);
            UInt32 magicNumber = BitConverter.ToUInt32(buffer, 0);
            if (magicNumber != PcapMagicNumber ^ magicNumber != AlternatePcapMagicNumber)
            {
                // If the magic bytes are swapped, then so is the whole file
                Byte[] temp = new Byte[4];
                Array.Copy(buffer, temp, 4);
                Array.Reverse(temp, 0, buffer.Length);
                magicNumber = BitConverter.ToUInt32(buffer, 0);
                byteInOrder = false;
            }
            else if (magicNumber != PcapMagicNumber && magicNumber != AlternatePcapMagicNumber)
            {
                throw new Exception("Not a valid Pcap stream");
            }
        }
    }
}
