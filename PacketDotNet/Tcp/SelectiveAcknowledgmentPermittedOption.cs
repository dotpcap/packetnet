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
    /// SACK (Selective Ack) Permitted Option
    /// Notifies the receiver that SelectiveAcknowledgment is allowed.
    /// Must only be sent in a SYN segment
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc2018/
    /// </remarks>
    public class SelectiveAcknowledgmentPermittedOption : TcpOption
    {
        /// <summary>
        /// Creates a Sack Permitted Option
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
        public SelectiveAcknowledgmentPermittedOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }
    }
}