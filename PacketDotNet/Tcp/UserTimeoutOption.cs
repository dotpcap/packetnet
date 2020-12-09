/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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
    public class UserTimeoutOption : TcpOption
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
        public UserTimeoutOption(byte[] bytes, int offset, int length) :
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