/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// No Operation Option
    /// Used in the TCP Options field to pad the length to the next 32 byte boundary
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc793/
    /// </remarks>
    public class NoOperationOption : TcpOption
    {
        /// <summary>
        /// The length (in bytes) of the NoOperation option
        /// </summary>
        internal const int OptionLength = 1;

        /// <summary>
        /// Creates a No Operation Option
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
        public NoOperationOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The length of the NoOperation field
        /// Returns 1 as opposed to returning the length field because
        /// the NoOperation option is only 1 byte long and doesn't
        /// contain a length field
        /// </summary>
        public override byte Length => OptionLength;
    }
}