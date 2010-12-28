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
    /// User Timeout Option
    /// The TCP user timeout controls how long transmitted data may remain
    ///  unacknowledged before a connection is forcefully closed
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://datatracker.ietf.org/doc/rfc5482/
    /// </remarks>
    public class UserTimeout: Option
    {
        #region Constructors

        /// <summary>
        /// Creates a User Timeout Option
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
        public UserTimeout(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The Granularity
        /// </summary>
        public bool Granularity
        {
            get
            {
                int granularity = ((int)Values >> 15);
                return (granularity != 0);
            }
        }

        /// <summary>
        /// The User Timeout
        /// </summary>
        public ushort Timeout
        {
            get { return (ushort)((int)Values & TimeoutMask); }
        }

        // a convenient property to grab the value fields for further processing
        private ushort Values
        {
            get { return EndianBitConverter.Big.ToUInt16(Bytes, ValuesFieldOffset); }
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
            return "[" + Kind.ToString() + ": Granularity=" + (Granularity ? "minutes" : "seconds") + " Timeout=" + Timeout + "]";
        }

        #endregion

         #region Members

        // the offset (in bytes) of the Value Fields
        const int ValuesFieldOffset = 2;

        // the mask used to strip the Granularity field from the
        //  Values filed to expose the UserTimeout field
        const int TimeoutMask = 0x7FFF;

        #endregion
    }
}