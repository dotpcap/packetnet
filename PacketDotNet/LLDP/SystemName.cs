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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Text;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A System Name TLV
    /// </summary>
    public class SystemName : StringTLV
    {
        #region Constructors

        /// <summary>
        /// Creates a System Name TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Name TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemName(byte[] bytes, int offset) :
            base(bytes, offset)
        {}

        /// <summary>
        /// Creates a System Name TLV and sets it value
        /// </summary>
        /// <param name="name">
        /// A textual Name of the system
        /// </param>
        public SystemName(string name) : base(TLVTypes.SystemName, name)
        {
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Name of the system
        /// </value>
        public string Name
        {
            get { return StringValue; }
            set { StringValue = value; }
        }

        #endregion
    }
}