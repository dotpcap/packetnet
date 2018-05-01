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
using System.IO;

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
        public static Int32 OnesComplementSum(Byte[] bytes)
        {
            //just complement the one's sum
            return OnesComplementSum(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// Computes the one's complement sum on a byte array
        /// </summary>
        public static Int32 OnesComplementSum(Byte[] bytes, Int32 start, Int32 len)
        {
            //just complement the one's sum
            return ~OnesSum(bytes, start, len) & 0xFFFF;
        }

        /// <summary>
        /// Computes the one's complement sum on a byte array combination.
        /// </summary>
        public static Int32 OnesComplementSum(ByteArraySegment byteArraySegment, byte[] prefixedBytes)
        {
            //just complement the one's sum
            return ~OnesSum(byteArraySegment, prefixedBytes) & 0xFFFF;
        }

        /// <summary>
        /// Compute a ones sum of a byte array
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static Int32 OnesSum(Byte[] bytes)
        {
            return OnesSum(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 16 bit sum of all values
        /// http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="start">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="len">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static Int32 OnesSum(Byte[] bytes, Int32 start, Int32 len)
        {
            MemoryStream memStream = new MemoryStream(bytes, start, len);
            BinaryReader br = new BinaryReader(memStream);
            Int32 sum = 0;

            while (memStream.Position < memStream.Length - 1)
            {
                var val = (UInt16)System.Net.IPAddress.NetworkToHostOrder(br.ReadInt16());
                sum += val;
            }

            // if we have a remaining byte we should add it
            if (memStream.Position < len)
            {
                sum += br.ReadByte();
            }


            // fold the sum into 16 bits
            while (sum >> 16 != 0)
            {
                sum = (sum & 0xffff) + (sum >> 16);
            }

            return sum;
        }

        /// <summary>
        /// 16 bit sum of all values
        /// http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        /// </summary>
        /// <param name="byteArraySegment">A <see cref="ByteArraySegment" />.</param>
        /// <param name="prefixedBytes">The prefixed bytes.</param>
        /// <param name="padOddBytesToLeft">if set to <c>true</c> pads the last byte to the left if the last byte has an odd index.</param>
        /// <returns>A <see cref="System.Int32" /></returns>
        public static int OnesSum(ByteArraySegment byteArraySegment, byte[] prefixedBytes, bool padOddBytesToLeft = true)
        {
            var sum = 0;
            for (int i = 0; i < prefixedBytes.Length; i += 2)
            {
                sum += prefixedBytes[i] << 8 | prefixedBytes[i + 1];
            }


            var byteArraySegmentStartOffset = 0;
            if (prefixedBytes.Length % 2 == 1 && byteArraySegment.Length > 0)
            {
                sum += prefixedBytes[prefixedBytes.Length - 1] << 8 | byteArraySegment.Bytes[byteArraySegment.Offset];
                byteArraySegmentStartOffset++;
            }


            var byteArraySegmentStart = byteArraySegmentStartOffset + byteArraySegment.Offset;
            var byteArraySegmentEnd = byteArraySegment.Length + byteArraySegment.Offset - 1;
            for (int i = byteArraySegmentStart; i < byteArraySegmentEnd; i += 2)
            {
                sum += byteArraySegment.Bytes[i] << 8 |
                       byteArraySegment.Bytes[i + 1];
            }


            if (byteArraySegment.Length % 2 == 1 && byteArraySegmentStartOffset == 0)
            {
                if (padOddBytesToLeft)
                    sum += byteArraySegment.Bytes[byteArraySegment.Offset + byteArraySegment.Length - 1] << 8;
                else
                    sum += byteArraySegment.Bytes[byteArraySegment.Offset + byteArraySegment.Length - 1];
            }


            while (sum >> 16 != 0)
            {
                sum = (sum & 0xffff) + (sum >> 16);
            }

            return sum;
        }
    }
}