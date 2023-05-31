using System.Runtime.CompilerServices;

namespace PacketDotNet.Utils.Converters;

    /// <summary>
    /// Implementation of EndianBitConverter which converts to/from little-endian
    /// byte arrays.
    /// </summary>
    public sealed class LittleEndianBitConverter : EndianBitConverter
    {
        /// <summary>
        /// Indicates the byte order ("endianness") in which data is converted using this class.
        /// </summary>
        public override Endianness Endianness => Endianness.LittleEndian;

        /// <summary>
        /// Copies the specified number of bytes from value to buffer, starting at index.
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <param name="bytes">The number of bytes to copy</param>
        /// <param name="buffer">The buffer to copy the bytes into</param>
        /// <param name="index">The index to start at</param>
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            for (var i = 0; i < bytes; i++)
            {
                buffer[i + index] = unchecked((byte) (value & 0xff));
                value >>= 8;
            }
        }

        /// <inheritdoc />
        public override void CopyBytes(char value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(short value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(int value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(long value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(ushort value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(uint value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override void CopyBytes(ulong value, byte[] buffer, int index)
        {
            Unsafe.WriteUnaligned(ref buffer[index], value);
        }

        /// <inheritdoc />
        public override char ToChar(byte[] buffer, int startIndex)
        {
            return (char) Unsafe.As<byte, ushort>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override ushort ToUInt16(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, ushort>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override uint ToUInt32(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, uint>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override ulong ToUInt64(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, ulong>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override short ToInt16(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, short>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override int ToInt32(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, int>(ref buffer[startIndex]);
        }

        /// <inheritdoc />
        public override long ToInt64(byte[] buffer, int startIndex)
        {
            return Unsafe.As<byte, long>(ref buffer[startIndex]);
        }
    }