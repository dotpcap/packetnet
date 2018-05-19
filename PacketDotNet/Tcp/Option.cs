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

using System;
using PacketDotNet.Utils;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// A TCP Option
    /// </summary>
    public abstract class Option
    {
        #region Constructors

        /// <summary>
        /// Creates an Option from a byte[]
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        protected Option(Byte[] bytes, Int32 offset, Int32 length)
        {
            _optionData = new ByteArraySegment(bytes, offset, length);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
        {
            return "[" + Kind + "]";
        }

        #endregion


        #region Properties

        /// <summary>
        /// The Length of the Option type
        /// </summary>
        public virtual Byte Length => Bytes[LengthFieldOffset];

        /// <summary>
        /// The Kind of option
        /// </summary>
        public OptionTypes Kind => (OptionTypes) Bytes[KindFieldOffset];

        /// <summary>
        /// Returns a TLV that contains the Option
        /// </summary>
        public Byte[] Bytes
        {
            get
            {
                var bytes = new Byte[_optionData.Length];
                Array.Copy(_optionData.Bytes, _optionData.Offset, bytes, 0, _optionData.Length);
                return bytes;
            }
        }

        #endregion


        #region Members

        // stores the data/length/offset of the option
        private readonly ByteArraySegment _optionData;

        /// <summary>The length (in bytes) of the Kind field</summary>
        internal const Int32 KindFieldLength = 1;

        /// <summary>The length (in bytes) of the Length field</summary>
        internal const Int32 LengthFieldLength = 1;

        /// <summary>The offset (in bytes) of the Kind Field</summary>
        internal const Int32 KindFieldOffset = 0;

        /// <summary>The offset (in bytes) of the Length field</summary>
        internal const Int32 LengthFieldOffset = 1;

        #endregion
    }
}