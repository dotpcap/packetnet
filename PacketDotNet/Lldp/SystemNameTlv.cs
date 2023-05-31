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

namespace PacketDotNet.Lldp;

    /// <summary>
    /// A System Name Tlv
    /// </summary>
    public class SystemNameTlv : StringTlv
    {
        /// <summary>
        /// Creates a System Name Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Name TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemNameTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        { }

        /// <summary>
        /// Creates a System Name TLV and sets it value
        /// </summary>
        /// <param name="name">
        /// A textual Name of the system
        /// </param>
        public SystemNameTlv(string name) : base(TlvType.SystemName, name)
        { }

        /// <value>
        /// A textual Name of the system
        /// </value>
        public string Name
        {
            get => Value;
            set => Value = value;
        }
    }