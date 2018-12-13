/*
This file is part of PacketDotNet.

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Computes the one's sum on a byte array.
    /// Based TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W. Richard
    /// Stevens. Page 236. And on http://www.cs.utk.edu/~cs594np/unp/checksum.html
    /// </summary>
    /*
    * taken from TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W.
    * Richard Stevens. Page 236
    */
    public static class ChecksumUtils
    {
        /// <summary>
        /// Computes the one's complement sum on a byte array
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns><see cref="Int32" />.</returns>
        public static Int32 OnesComplementSum(Byte[] bytes)
        {
            // Just complement the one's sum.
            return OnesComplementSum(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Computes the one's complement sum on a byte array
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="start">The start.</param>
        /// <param name="len">The length.</param>
        /// <returns><see cref="Int32" />.</returns>
        public static Int32 OnesComplementSum(Byte[] bytes, Int32 start, Int32 len)
        {
            // Just complement the one's sum.
            return ~OnesSum(bytes, start, len);
        }

        /// <summary>
        /// Computes the one's complement sum on a byte array combination.
        /// </summary>
        /// <param name="byteArraySegment">The byte array segment.</param>
        /// <param name="prefixedBytes">The prefixed bytes.</param>
        /// <returns><see cref="Int32" />.</returns>
        public static Int32 OnesComplementSum(ByteArraySegment byteArraySegment, byte[] prefixedBytes)
        {
            // Just complement the one's sum.
            return ~OnesSum(byteArraySegment, prefixedBytes);
        }

        /// <summary>
        /// Compute a ones sum of a byte array
        /// </summary>
        /// <param name="bytes">A <see cref="System.Byte" /></param>
        /// <returns>A <see cref="System.Int32" /></returns>
        public static Int32 OnesSum(Byte[] bytes)
        {
            return OnesSum(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 16 bit sum of all values
        /// http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        /// </summary>
        /// <param name="bytes">A <see cref="System.Byte" /></param>
        /// <param name="start">A <see cref="System.Int32" /></param>
        /// <param name="len">A <see cref="System.Int32" /></param>
        /// <returns>A <see cref="System.Int32" /></returns>
        public static Int32 OnesSum(Byte[] bytes, Int32 start, Int32 len)
        {
            unsafe
            {
                fixed (byte* mainFixed = bytes)
                {
                    var mainArray = mainFixed;
                    if (start > 0)
                        mainArray += start;

                    return OnesSum(mainArray, len, (byte*)IntPtr.Zero, 0);
                }
            }
        }

        /// <summary>
        /// 16 bit sum of all values
        /// http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        /// </summary>
        /// <param name="byteArraySegment">A <see cref="ByteArraySegment" />.</param>
        /// <param name="prefixedBytes">The prefixed bytes.</param>
        /// <returns>A <see cref="System.Int32" /></returns>
        public static UInt16 OnesSum(ByteArraySegment byteArraySegment, byte[] prefixedBytes)
        {
            unsafe
            {
                fixed (byte* mainFixed = byteArraySegment.Bytes)
                {
                    fixed (byte* prefixedFixed = prefixedBytes)
                    {
                        var mainArray = mainFixed;
                        if (byteArraySegment.Offset > 0)
                            mainArray += byteArraySegment.Offset;

                        return OnesSum(mainArray, byteArraySegment.Length, prefixedFixed, prefixedBytes.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 16 bit sum of all values
        /// http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        /// </summary>
        /// <param name="mainArray">The main array.</param>
        /// <param name="mainSize">Size of the main array.</param>
        /// <param name="prefixArray">The prefix array.</param>
        /// <param name="prefixSize">Size of the prefix array.</param>
        /// <returns><see cref="UInt16"/>.</returns>
        private static unsafe UInt16 OnesSum(byte* mainArray, int mainSize, byte* prefixArray, int prefixSize)
        {
            ulong sum = 0;

            sum = Sum(prefixArray, prefixSize, sum);
            sum = Sum(mainArray, mainSize, sum);

            // Swap the byte order of the sum.
            sum = SwapBytes(sum);
            
            // Folds the sum down to 16 bits.
            var uint1 = (uint)sum;
            var uint2 = (uint)(sum >> 32);
            uint1 += uint2;
            if (uint1 < uint2)
                uint1++;

            var ushort1 = (ushort)uint1;
            var ushort2 = (ushort)(uint1 >> 16);
            ushort1 += ushort2;
            if (ushort1 < ushort2)
                ushort1++;

            return ushort1;
        }

        /// <summary>
        /// Calculates the sum of the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="size">The size.</param>
        /// <param name="sum">The sum.</param>
        /// <returns><see cref="System.UInt64"/>.</returns>
        private static unsafe UInt64 Sum(byte* array, int size, ulong sum)
        {
            // Reads per 8 bytes (ulong), this is the main loop.
            ulong* prefixArrayLong = (ulong*)array;
            while (size >= 8)
            {
                var s = *prefixArrayLong++;
                sum += s;
                if (sum < s)
                    sum++;

                size -= 8;
            }

            // The remainder of the array, which is less than 8 bytes long, needs to be read now.
            array = (byte*)prefixArrayLong;

            // Reads 4 bytes (uint).
            if ((size & 4) != 0)
            {
                var s = *(uint*)array;
                sum += s;
                if (sum < s)
                    sum++;

                array += 4;
            }

            // Reads 2 bytes (ushort).
            if ((size & 2) != 0)
            {
                var s = *(ushort*)array;
                sum += s;
                if (sum < s)
                    sum++;

                array += 2;
            }

            // Reads 1 byte (byte).
            if ((size & 1) != 0)
            {
                var s = *array;
                sum += s;
                if (sum < s)
                    sum++;
            }

            return sum;
        }

        /// <summary>
        /// Swaps the order of the bytes in a <c>ulong</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see cref="System.UInt64"/>.</returns>
        private static ulong SwapBytes(ulong value)
        {
            // Swap adjacent 32-bit blocks.
            value = (value >> 32) | (value << 32);
            // Swap adjacent 16-bit blocks.
            value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
            // Swap adjacent 8-bit blocks.
            return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
        }
    }
}