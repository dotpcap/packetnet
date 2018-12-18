using System;
using System.IO;

namespace Test
{
    /// <summary>
    /// ByteSwapperBinaryReader will swap the byte order as numbers are read (big to little endian or little to big depending on 
    /// the order on the wire).
    /// This is a wrapper around another stream.
    /// </summary>
    public class ByteSwappedBinaryReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteSwappedBinaryReader"/> class.
        /// </summary>
        /// <param name="input">The stream that needs swapping</param>
        public ByteSwappedBinaryReader(System.IO.Stream input) : base(input)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteSwappedBinaryReader"/> class.
        /// Honours the encoding of the 'input' stream
        /// </summary>
        /// <param name="input">The stream that needs swapping</param>
        /// <param name="encoding">The encoding of the 'input' stream.</param>
        public ByteSwappedBinaryReader(System.IO.Stream input, System.Text.Encoding encoding) : base(input, encoding)
        {
        }

        /// <summary>
        /// Read an unsigned 64-bit (8 byte) integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 64-bit unsigned integer</returns>
        public override System.UInt64 ReadUInt64()
        {
            return Swap8(base.ReadUInt64());
        }

        /// <summary>
        /// Read an 64-bit (8 byte) signed integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 64-bit signed integer</returns>
        public override System.Int64 ReadInt64()
        {
            return (Int64)ReadUInt64();
        }

        /// <summary>
        /// Read an unsigned 32-bit (4 byte) integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 32-bit unsigned integer</returns>
        public override System.UInt32 ReadUInt32()
        {
            return Swap4(base.ReadUInt32());
        }

        /// <summary>
        /// Read an unsigned 32-bit (4 byte) integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 32-bit signed integer</returns>
        public override System.Int32 ReadInt32()
        {
            return (Int32)ReadUInt32();
        }

        /// <summary>
        /// Read an unsigned 16-bit (2 byte) integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 16-bit unsigned integer</returns>
        public override System.UInt16 ReadUInt16()
        {
            return Swap2(base.ReadUInt16());
        }

        /// <summary>
        /// Read an unsigned 16-bit (2 byte) integer off the input stream and byte swap the order and return it.
        /// </summary>
        /// <returns>A byte-swapped 16-bit signed integer</returns>
        public override System.Int16 ReadInt16()
        {
            return (Int16)ReadUInt16();
        }

        /// <summary>
        /// Takes in a 64-bit (8 byte) unsigned integer and swaps the byte order.
        /// </summary>
        /// <param name="myUInt64">64-bit unsigned integer to be swapped.</param>
        /// <returns>A byte swapped 64-bit unsigned integer</returns>
        public static UInt64 Swap8(UInt64 myUInt64)
        {
            UInt64 tempUInt64 = (
                ((myUInt64 & 0xff00000000000000) >> 56) |
                ((myUInt64 & 0x00ff000000000000) >> 40) |
                ((myUInt64 & 0x0000ff0000000000) >> 24) |
                ((myUInt64 & 0x000000ff00000000) >> 8) |
                ((myUInt64 & 0x00000000ff000000) << 8) |
                ((myUInt64 & 0x0000000000ff0000) << 24) |
                ((myUInt64 & 0x000000000000ff00) << 40) |
                ((myUInt64 & 0x00000000000000ff) << 56)
                );
            return tempUInt64;
        }

        /// <summary>
        /// Takes in a 32-bit (4 byte) unsigned integer and swaps the byte order.
        /// </summary>
        /// <param name="myUInt32">32-bit unsigned integer to be swapped.</param>
        /// <returns>A byte swapped 32-bit unsigned integer</returns>
        public static UInt32 Swap4(UInt32 myUInt32)
        {
            UInt32 tempUInt32 = (
                ((myUInt32 & 0xff000000) >> 24) |
                ((myUInt32 & 0x00ff0000) >> 8) |
                ((myUInt32 & 0x0000ff00) << 8) |
                ((myUInt32 & 0x000000ff) << 24));
            return tempUInt32;
        }

        /// <summary>
        /// Takes in a 16-bit (2 byte) unsigned integer and swaps the byte order.
        /// </summary>
        /// <param name="myUInt16">16-bit unsigned integer to be swapped.</param>
        /// <returns>A byte swapped 16-bit unsigned integer</returns>
        public static UInt16 Swap2(UInt16 myUInt16)
        {
            UInt16 tempUInt16 = (UInt16)(
                ((myUInt16 & 0x000000ff) << 8) |
                ((myUInt16 & 0x0000ff00) >> 8));
            return tempUInt16;
        }
    }

}
