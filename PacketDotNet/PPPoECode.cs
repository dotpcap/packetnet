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
 */

using System;
namespace PacketDotNet
{
    /// <summary>
    /// Values for the Code field of a PPPoE packet
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    public enum PPPoECode : ushort
    {
        /// <summary>
        /// The PPPoe payload must contain a PPP packet
        /// </summary>
        SessionStage = 0x0,

        /// <summary>
        /// Active Discovery Offer (PADO) packet
        /// </summary>
        ActiveDiscoveryOffer = 0x07,

        /// <summary>
        /// From RFC2516:
        /// The Host sends the PADI packet with the DESTINATION_ADDR set to the
        /// broadcast address.  The CODE field is set to 0x09 and the SESSION_ID
        /// MUST be set to 0x0000.
        ///
        /// The PADI packet MUST contain exactly one TAG of TAG_TYPE Service-
        /// Name, indicating the service the Host is requesting, and any number
        /// of other TAG types.  An entire PADI packet (including the PPPoE
        /// header) MUST NOT exceed 1484 octets so as to leave sufficient room
        /// for a relay agent to add a Relay-Session-Id TAG.
        /// </summary>
        ActiveDiscoveryInitiation = 0x9,

        /// <summary>
        /// Indicate that the PPPoe session specified by the SessionId field of
        /// the PPPoe packet has been terminated
        /// </summary>
        ActiveDiscoveryTerminate = 0xa7,
    }
}

