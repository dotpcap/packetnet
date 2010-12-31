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
    /// A System Description TLV
    /// </summary>
    public class SystemDescription : StringTLV
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        #region Constructors

        /// <summary>
        /// Creates a System Description TLV
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
            log.Debug("");
        }

        /// <summary>
        /// Creates a System Description TLV and sets it value
        /// </summary>
        /// <param name="description">
        /// A textual Description of the system
        /// </param>
        public SystemDescription(string description) : base(TLVTypes.SystemDescription,
                                                            description)
        {
            log.Debug("");
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Description of the system
        /// </value>
        public string Description
        {
            get { return StringValue; }
            set { StringValue = value; }
        }

        #endregion
    }
}