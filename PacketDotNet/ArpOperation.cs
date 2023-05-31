/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet;

    /// <summary>
    /// The possible ARP operation values
    /// </summary>
    /// <remarks>
    /// References:
    /// - http://www.networksorcery.com/enp/default1101.htm
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArpOperation : ushort
    {
        /// <summary>Request</summary>
        /// <remarks>See RFC 826, RFC 5227</remarks>
        Request = 1,

        /// <summary>Response</summary>
        /// <remarks>See RFC 826, RFC 1868, RFC 5227</remarks>
        Response = 2,

        /// <summary>Request Reverse</summary>
        /// <remarks>See RFC 903</remarks>
        RequestReverse = 3,

        /// <summary>Reply Reverse</summary>
        /// <remarks>See RFC 903</remarks>
        ReplyReverse = 4,

        /// <summary>DRARP Request</summary>
        /// <remarks>See RFC 1931</remarks>
        DrarpRequest = 5,

        /// <summary>DRARP Reply</summary>
        /// <remarks>See RFC 1931</remarks>
        DrarpReply = 6,

        /// <summary>DRARP Error</summary>
        /// <remarks>See RFC 1931</remarks>
        DrarpError = 7,

        /// <summary>InARP Request</summary>
        /// <remarks>See RFC 1293</remarks>
        InArpRequest = 8,

        /// <summary>InARP Reply</summary>
        /// <remarks>See RFC 1293</remarks>
        InArpReply = 9,

        /// <summary>ARP NAK</summary>
        /// <remarks>See RFC 1577</remarks>
        ArpNak = 10,

        /// <summary>MARS Request</summary>
        MarsRequest = 11,

        /// <summary>MARS Multi</summary>
        MarsMulti = 12,

        /// <summary>MARS MServ</summary>
        MarsMServ = 13,

        /// <summary>MARS Join</summary>
        MarsJoin = 14,

        /// <summary>MARS Leave</summary>
        MarsLeave = 15,

        /// <summary>MARS NAK</summary>
        MarsNak = 16,

        /// <summary>MARS Unserv</summary>
        MarsUnserv = 17,

        /// <summary>MARS SJoin</summary>
        MarsSJoin = 18,

        /// <summary>MARS SLeave</summary>
        MarsSLeave = 19,

        /// <summary>MARS Grouplist Request</summary>
        MarsGroupListRequest = 20,

        /// <summary>MARS Grouplist Reply</summary>
        MarsGroupListReply = 21,

        /// <summary>MARS Redirect Map</summary>
        MarsRedirectMap = 22,

        /// <summary>MARS UNARP</summary>
        /// <remarks>See RFC 2176</remarks>
        MarsUnArp = 23,

        /// <summary>OP_EXP1</summary>
        /// <remarks>See RFC 5494</remarks>
        OpExp1 = 24,

        /// <summary>OP_EXP2</summary>
        OpExp2 = 25
    }