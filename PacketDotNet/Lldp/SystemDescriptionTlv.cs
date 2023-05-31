/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet.Lldp;

    /// <summary>
    /// A System Description Tlv
    /// </summary>
    public class SystemDescriptionTlv : StringTlv
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
        public SystemDescriptionTlv(byte[] bytes, int offset) :
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
        public SystemDescriptionTlv(string description) : base(TlvType.SystemDescription, description)
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