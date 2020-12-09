/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Echo Reply Option
    /// Marked obsolete in the TCP spec Echo Reply Option has been
    /// replaced by the TSOPT (Timestamp Option)
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1072/
    /// </remarks>
    public class EchoReplyOption : TcpOption
    {
        /// <summary>
        /// Creates an Echo Reply Option
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
        public EchoReplyOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }
    }
}