/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// End-of-Options Option
    /// Marks the end of the Options list
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc793/
    /// </remarks>
    public class EndOfOptionsOption : TcpOption
    {
        /// <summary>
        /// The length (in bytes) of the EndOfOptions option
        /// </summary>
        internal const int OptionLength = 1;

        /// <summary>
        /// Creates an End Of Options Option
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
        public EndOfOptionsOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The length of the EndOfOptions field
        /// Returns 1 as opposed to returning the length field because
        /// the EndOfOptions option is only 1 byte long and doesn't
        /// contain a length field
        /// </summary>
        public override byte Length => OptionLength;
    }
}