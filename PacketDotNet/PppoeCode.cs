/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// Values for the Code field of a PPPoE packet
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    public enum PppoeCode : ushort
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
        ActiveDiscoveryTerminate = 0xa7
    }