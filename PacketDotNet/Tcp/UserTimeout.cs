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
    /// User Timeout Option
    /// The TCP user timeout controls how long transmitted data may remain
    /// unacknowledged before a connection is forcefully closed
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc5482/
    /// </remarks>
    public class UserTimeout : Option
    {
        // the mask used to strip the Granularity field from the
        //  Values filed to expose the UserTimeout field
        private const int TimeoutMask = 0x7FFF;

        // the offset (in bytes) of the Value Fields
        private const int ValuesFieldOffset = 2;

        /// <summary>
        /// Creates a User Timeout Option
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
        public UserTimeout(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The Granularity
        /// </summary>
        public bool Granularity
        {
            get
            {
                var granularity = Values >> 15;
                return granularity != 0;
            }
        }

        /// <summary>
        /// The User Timeout
        /// </summary>
        public ushort Timeout => (ushort) (Values & TimeoutMask);

        // a convenient property to grab the value fields for further processing
        public ushort Values
        {
            get => EndianBitConverter.Big.ToUInt16(OptionData.Bytes, OptionData.Offset + ValuesFieldOffset);
            set => EndianBitConverter.Big.CopyBytes(value, OptionData.Bytes, OptionData.Offset + ValuesFieldOffset);
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": Granularity=" + (Granularity ? "minutes" : "seconds") + " Timeout=" + Timeout + "]";
        }
    }
}