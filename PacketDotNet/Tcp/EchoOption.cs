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
    /// An Echo Option
    /// throws an exception because Echo Options
    /// are obsolete as per their spec
    /// </summary>
    public class EchoOption : TcpOption
    {
        /// <summary>
        /// Creates an Echo Option
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
        public EchoOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }
    }
}