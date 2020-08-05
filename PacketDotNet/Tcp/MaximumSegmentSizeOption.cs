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
    /// Maximum Segment Size Option
    /// An extension to the DataOffset/HeaderLength field to
    /// allow sizes greater than 65,535
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc793/
    /// </remarks>
    public class MaximumSegmentSizeOption : TcpOption
    {
        // the offset (in bytes) of the Value Field
        private const int ValueFieldOffset = 2;

        /// <summary>
        /// Creates a Maximum Segment Size Option
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
        public MaximumSegmentSizeOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The Maximum Segment Size
        /// </summary>
        public ushort Value
        {
            get => EndianBitConverter.Big.ToUInt16(OptionData.Bytes, OptionData.Offset + ValueFieldOffset);
            set => EndianBitConverter.Big.CopyBytes(value, OptionData.Bytes, OptionData.Offset + ValueFieldOffset);
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": Value=" + Value + " bytes]";
        }
    }
}