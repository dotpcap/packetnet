/*
This file is part of PacketDotNet.

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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    /// <summary>
    /// ICMPv6 types, see http://en.wikipedia.org/wiki/ICMPv6 and
    /// http://www.iana.org/assignments/icmpv6-parameters
    /// </summary>
    public enum ICMPv6Types : byte
    {
#pragma warning disable 1591
        #region ICMPv6 Error Messages
        DestinationUnreachable = 1,
        PacketTooBig = 2,
        TimeExceeded = 3,
        ParameterProblem = 4,
        PrivateExperimentation1 = 100,
        PrivateExperimentation2 = 101,
        ReservedForExpansion1 = 127,
        #endregion
        #region ICMPv6 Informational Messages
        EchoRequest = 128,
        EchoReply = 129,
        RouterSolicitation = 133,
        PrivateExperimentation3 = 200,
        PrivateExperimentation4 = 201,
        ReservedForExpansion2 = 255
        #endregion
#pragma warning restore 1591
    }
}
