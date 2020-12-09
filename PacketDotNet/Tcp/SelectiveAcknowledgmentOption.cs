/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// SelectiveAcknowledgment (Selective Ack) Option
    /// Provides a means for a receiver to notify the sender about
    /// all the segments that have arrived successfully.
    /// Used to cut down on the number of unnecessary re-transmissions.
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc2018/
    /// http://datatracker.ietf.org/doc/rfc2883/
    /// </remarks>
    public class SelectiveAcknowledgmentOption : TcpOption
    {
        // the length (in bytes) of a SelectiveAcknowledgment block
        private const int BlockLength = 2;

        // the offset (in bytes) of the ScaleFactor Field
        private const int SACKBlocksFieldOffset = 2;

        /// <summary>
        /// Creates a SelectiveAcknowledgment (Selective Ack) Option
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
        public SelectiveAcknowledgmentOption(byte[] bytes, int offset, int length) :
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