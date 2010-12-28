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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 */
using System;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// The different types fields that could be found in the Options field
    /// </summary>
    /// <remarks>
    /// References:
    ///  http://en.wikipedia.org/wiki/Transmission_Control_Protocol#TCP_Timestamps
    ///  http://www.networksorcery.com/enp/default1101.htm
    /// </remarks>
    public enum OptionTypes : byte
    {
        /// <summary>End Of List</summary>
        /// <remarks>See RFC 793</remarks>
        EndOfOptionList = 0,

        /// <summary>No Operation</summary>
        /// <remarks>See RFC 793</remarks>
        NoOperation = 1,

        /// <summary>Maximum Segment Size</summary>
        /// <remarks>See RFC 793</remarks>
        MaximumSegmentSize = 2,

        /// <summary>Window Scale Factor</summary>
        /// <remarks>See RFC 1323</remarks>
        WindowScaleFactor = 3,

        /// <summary>SACK (Selective Ack) Permitted</summary>
        /// <remarks>See RFC 2018</remarks>
        SACKPermitted = 4,

        /// <summary>SACK (Selective Ack)</summary>
        /// <remarks>See RFC 2018 and RFC 2883</remarks>
        SACK = 5,

        /// <summary>Echo (obsolete)</summary>
        /// <remarks>See RFC 1072</remarks>
        Echo = 6,

        /// <summary>Echo Reply (obsolete)</summary>
        /// <remarks>See RFC 1072</remarks>
        EchoReply = 7,

        /// <summary>Timestamp</summary>
        /// <remarks>See RFC 1323</remarks>
        Timestamp = 8,

        /// <summary>Partial Order Connection Permitted (experimental)</summary>
        /// <remarks>See RFC 1693</remarks>
        POConnectionPermitted = 9,

        /// <summary>Partial Order Service Profile (experimental)</summary>
        /// <remarks>See RFC 1693</remarks>
        POServiceProfile = 10,

        /// <summary>Connection Count (experimental)</summary>
        /// <remarks>See RFC 1644</remarks>
        ConnectionCount = 11,

        /// <summary>Connection Count New (experimental)</summary>
        /// <remarks>See RFC 1644</remarks>
        ConnectionCountNew = 12,

        /// <summary>Connection Count Echo (experimental)</summary>
        /// <remarks>See RFC 1644</remarks>
        ConnectionCountEcho = 13,

        /// <summary>Alternate Checksum Request</summary>
        /// <remarks>See RFC 1146</remarks>
        AlternateChecksumRequest = 14,

        /// <summary>Alternate Checksum Data</summary>
        /// <remarks>See RFC 1146</remarks>
        AlternateChecksumData = 15,

        /// <summary>MD5 Signature</summary>
        /// <remarks>See RFC 2385</remarks>
        MD5Signature = 19,

        /// <summary>Quick-Start Response (experimental)</summary>
        /// <remarks>See RFC 4782</remarks>
        QuickStartResponse = 27,

        /// <summary>User Timeout</summary>
        /// <remarks>See RFC 5482</remarks>
        UserTimeout = 28
    }
}