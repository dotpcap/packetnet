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

#if DEBUG
using System.Reflection;
using log4net;
#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A System Description Tlv
    /// </summary>
    [Serializable]
    public class SystemDescription : String
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Creates a System Description Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemDescription(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a System Description TLV and sets it value
        /// </summary>
        /// <param name="description">
        /// A textual Description of the system
        /// </param>
        public SystemDescription(string description) : base(TlvType.SystemDescription, description)
        {
            Log.Debug("");
        }

        /// <value>
        /// A textual Description of the system
        /// </value>
        public string Description
        {
            get => Value;
            set => Value = value;
        }
    }
}