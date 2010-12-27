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

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// No Operation Option
    ///  Used in the TCP Options field to pad the length to the next 32 byte boundary
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://datatracker.ietf.org/doc/rfc793/
    /// </remarks>
    public class NoOperation : Option
    {
        #region Constructors

        /// <summary>
        /// Creates a No Operation Option
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
        public NoOperation(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The length of the NoOperation field
        ///  Returns 1 as opposed to returning the length field because
        ///  the NoOperation option is only 1 byte long and doesn't
        ///  contain a length field
        /// </summary>
        public override byte Length
        {
            get { return NoOperation.OptionLength; }
        }

        #endregion

        #region Members

        /// <summary>
        /// The length (in bytes) of the NoOperation option
        /// </summary>
        internal const int OptionLength = 1;

        #endregion
    }
}