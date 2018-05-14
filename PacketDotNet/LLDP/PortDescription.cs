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
using System.Reflection;
using log4net;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Port Description TLV
    /// </summary>
    [Serializable]
    public class PortDescription : StringTLV
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


        #region Constructors

        /// <summary>
        /// Creates a Port Description TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public PortDescription(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a Port Description TLV and sets it value
        /// </summary>
        /// <param name="description">
        /// A textual description of the port
        /// </param>
        public PortDescription(String description) : base(TLVTypes.PortDescription, description)
        {
            Log.Debug("");
        }

        #endregion


        #region Properties

        /// <value>
        /// A textual Description of the port
        /// </value>
        public String Description
        {
            get => StringValue;
            set => StringValue = value;
        }

        #endregion
    }
}