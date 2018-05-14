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
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// A Time Stamp Option
    /// Used for RTTM (Round Trip Time Measurement)
    /// and PAWS (Protect Against Wrapped Sequences)
    /// Opsoletes the Echo and EchoReply option fields
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1323/
    /// </remarks>
    public class TimeStamp : Option
    {
        #region Constructors

        /// <summary>
        /// Creates a Timestamp Option
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
        public TimeStamp(Byte[] bytes, Int32 offset, Int32 length) :
            base(bytes, offset, length)
        { }

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
            return "[" + Kind + ": Value=" + Value + " EchoReply=" + EchoReply + "]";
        }

        #endregion


        #region Properties

        /// <summary>
        /// The Timestamp value
        /// </summary>
        public UInt32 Value => EndianBitConverter.Big.ToUInt32(Bytes, ValueFieldOffset);

        /// <summary>
        /// The Echo Reply
        /// </summary>
        public UInt32 EchoReply => EndianBitConverter.Big.ToUInt32(Bytes, EchoReplyFieldOffset);

        #endregion


        #region Members

        // the offset (in bytes) of the Value Field
        const Int32 ValueFieldOffset = 2;

        // the offset (in bytes) of the Echo Reply Field
        const Int32 EchoReplyFieldOffset = 6;

        #endregion
    }
}