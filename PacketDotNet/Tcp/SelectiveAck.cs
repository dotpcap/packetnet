/*
This file is part of PacketDotNet

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
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// SelectiveAck (Selective Ack) Option
    /// Provides a means for a receiver to notify the sender about
    /// all the segments that have arrived successfully.
    /// Used to cut down on the number of unnecessary re-transmissions.
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc2018/
    /// http://datatracker.ietf.org/doc/rfc2883/
    /// </remarks>
    public class SelectiveAck : Option
    {
        // the length (in bytes) of a SelectiveAck block
        private const int BlockLength = 2;

        // the offset (in bytes) of the ScaleFactor Field
        private const int SACKBlocksFieldOffset = 2;

        /// <summary>
        /// Creates a SelectiveAck (Selective Ack) Option
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public SelectiveAck(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// Contains an array of selective ack blocks.
        /// </summary>
        public ushort[] Blocks
        {
            get
            {
                var numOfBlocks = (Length - SACKBlocksFieldOffset) / BlockLength;
                var blocks = new ushort[numOfBlocks];
                for (var i = 0; i < numOfBlocks; i++)
                {
                    var offset = SACKBlocksFieldOffset + (i * BlockLength);
                    blocks[i] = EndianBitConverter.Big.ToUInt16(Bytes, offset);
                }

                return blocks;
            }
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            var output = "[" + Kind + ": ";

            for (var i = 0; i < Blocks.Length; i++)
            {
                output += "Block" + i + "=" + Blocks[i] + " ";
            }

            output = output.TrimEnd();
            output += "]";

            return output;
        }
    }
}