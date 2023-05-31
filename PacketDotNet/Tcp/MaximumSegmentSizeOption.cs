/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Tcp;

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