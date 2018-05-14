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
    /// MD5 Signature
    /// Carries the MD5 Digest used by the BGP protocol to
    /// ensure security between two endpoints
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc2385/
    /// </remarks>
    public class MD5Signature : Option
    {
        #region Members

        // the offset (in bytes) of the MD5 Digest field
        const Int32 MD5DigestFieldOffset = 2;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a MD5 Signature Option
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
        public MD5Signature(Byte[] bytes, Int32 offset, Int32 length) :
            base(bytes, offset, length)
        { }

        #endregion


        #region Properties

        /// <summary>
        /// The MD5 Digest
        /// </summary>
        public Byte[] MD5Digest
        {
            get
            {
                var data = new Byte[Length - MD5DigestFieldOffset];
                Array.Copy(Bytes, MD5DigestFieldOffset, data, 0, data.Length);
                return data;
            }
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
            return "[" + Kind + ": MD5Digest=0x" + MD5Digest + "]";
        }

        #endregion
    }
}