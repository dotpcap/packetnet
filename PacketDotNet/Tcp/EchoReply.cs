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

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// Echo Reply Option
    ///  Marked obsolete in the TCP spec Echo Reply Option has been
    ///  replaced by the TSOPT (Timestamp Option)
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://datatracker.ietf.org/doc/rfc1072/
    /// </remarks>
    public class EchoReply : Option
    {
        #region Constructors

        /// <summary>
        /// Creates an Echo Reply Option
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
        public EchoReply(byte[] bytes, int offset, int length):
            base(bytes, offset, length)
        {
            throw new NotSupportedException("Obsolete: The Echo Option has been deprecated.");
        }

        #endregion
    }
}