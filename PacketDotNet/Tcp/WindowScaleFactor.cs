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
    /// Window Scale Factor Option
    /// Expands the definition of the TCP window to 32 bits
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1323/
    /// </remarks>
    public class WindowScaleFactor : Option
    {
        #region Members

        // the offset (in bytes) of the ScaleFactor Field
        const Int32 ScaleFactorFieldOffset = 2;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a Window Scale Factor Option
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
        public WindowScaleFactor(Byte[] bytes, Int32 offset, Int32 length) :
            base(bytes, offset, length)
        { }

        #endregion


        #region Properties

        /// <summary>
        /// The Window Scale Factor
        /// used as a multiplier to the window value
        /// The multiplier is equal to 1 left-shifted by the ScaleFactor
        /// So a scale factor of 7 would equal 1 &lt;&lt; 7 = 128
        /// </summary>
        public Byte ScaleFactor => Bytes[ScaleFactorFieldOffset];

        #endregion


        #region Methods

        /// <summary>
        /// Returns the Option info as a string
        /// The multiplier is equal to a value of 1 left-shifted by the scale factor
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
        {
            return "[" + Kind + ": ScaleFactor=" + ScaleFactor + " (multiply by " + (1 << ScaleFactor) + ")]";
        }

        #endregion
    }
}