/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Port Description Tlv
    /// </summary>
    public class PortDescriptionTlv : StringTlv
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
        /// Creates a Port Description Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public PortDescriptionTlv(byte[] bytes, int offset) :
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
        public PortDescriptionTlv(string description) : base(TlvType.PortDescription, description)
        {
            Log.Debug("");
        }

        /// <value>
        /// A textual Description of the port
        /// </value>
        public string Description
        {
            get => Value;
            set => Value = value;
        }
    }
}