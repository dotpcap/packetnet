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
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Maximum Segment Size Option
    ///  An extension to the DataOffset/HeaderLength field to
    ///  allow sizes greater than 65,535
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://datatracker.ietf.org/doc/rfc793/
    /// </remarks>
    public class MaximumSegmentSize : Option
    {
        #region Constructors

        /// <summary>
        /// Creates a Maximum Segment Size Option
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte[]"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public MaximumSegmentSize(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The Maximum Segment Size
        /// </summary>
        public ushort Value
        {
            get { return EndianBitConverter.Big.ToUInt16(Bytes, ValueFieldOffset); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind.ToString() + ": Value=" + Value.ToString() + " bytes]";
        }

        #endregion

         #region Members

        // the offset (in bytes) of the Value Field
        const int ValueFieldOffset = 2;

        #endregion
    }
}