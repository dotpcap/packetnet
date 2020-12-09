/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Tcp
{
    public class UnsupportedOption : TcpOption
    {
        /// <summary>
        /// Creates an Option that is not yet supported by PacketDotNet.
        /// </summary>
        /// <param name="bytes">A <see cref="T:System.Byte[]" /></param>
        /// <param name="offset">A <see cref="T:System.Int32" /></param>
        /// <param name="length">A <see cref="T:System.Int32" /></param>
        public UnsupportedOption(byte[] bytes, int offset, int length) : base(bytes, offset, length)
        { }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": Currently unsupported by PacketDotNet]";
        }
    }
}