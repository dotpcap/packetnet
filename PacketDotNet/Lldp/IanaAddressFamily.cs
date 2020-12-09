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

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// The IANA (Internet Assigned Numbers Authority) Address Family
    /// </summary>
    /// <remarks>Source http://www.iana.org/assignments/address-family-numbers/</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum IanaAddressFamily
    {
        /// <summary>IP version 4</summary>
        IPv4 = 1,

        /// <summary>IP version 6</summary>
        IPv6 = 2,

        /// <summary>NSAP</summary>
        Nsap = 3,

        /// <summary>HDLC</summary>
        Hdlc = 4,

        /// <summary>BBN 1822</summary>
        Bbn1822 = 5,

        /// <summary>802 (includes all 802 media plus Ethernet "canonical format")</summary>
        Eth802 = 6,

        /// <summary>E.163</summary>
        E163 = 7

        // Add more if necessary
        // See remarks for more info on where
        // to find more info
    }
}