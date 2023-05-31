/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet;

    /// <summary>
    /// ICMPv6 types, see http://en.wikipedia.org/wiki/ICMPv6 and
    /// http://www.iana.org/assignments/icmpv6-parameters
    /// </summary>
    public enum IcmpV6Type : byte
    {
        DestinationUnreachable = 1, // [RFC4443]
        PacketTooBig = 2, // [RFC4443]
        TimeExceeded = 3, // [RFC4443]
        ParameterProblem = 4, // [RFC4443]
        PrivateExperimentation1 = 100, // [RFC4443]
        PrivateExperimentation2 = 101, // [RFC4443]
        ReservedForExpansion1 = 127, // [RFC4443]
        EchoRequest = 128, // [RFC4443]
        EchoReply = 129, // [RFC4443]
        MulticastListenerQuery = 130, // [RFC2710]
        MulticastListenerReport = 131, // [RFC2710]
        MulticastListenerDone = 132, // [RFC2710]
        RouterSolicitation = 133, // [RFC4861]
        RouterAdvertisement = 134, // [RFC4861]
        NeighborSolicitation = 135, // [RFC4861]
        NeighborAdvertisement = 136, // [RFC4861]
        RedirectMessage = 137, // [RFC4861]
        RouterRenumbering = 138, //	[Matt_Crawford]
        ICMPNodeInformationQuery = 139, // [RFC4620]
        ICMPNodeInformationResponse = 140, // [RFC4620]
        InverseNeighborDiscoverySolicitationMessage = 141, // [RFC3122]
        InverseNeighborDiscoveryAdvertisementMessage = 142, // [RFC3122]
        Version2MulticastListenerReport = 143, // [RFC3810]
        HomeAgentAddressDiscoveryRequestMessage = 144, // [RFC6275]
        HomeAgentAddressDiscoveryReplyMessage = 145, // [RFC6275]
        MobilePrefixSolicitation = 146, // [RFC6275]
        MobilePrefixAdvertisement = 147, // [RFC6275]
        CertificationPathSolicitationMessage = 148, // [RFC3971]
        CertificationPathAdvertisementMessage = 149, // [RFC3971]
        ExperimentalMobilityProtocols = 150, // [RFC4065]
        MulticastRouterAdvertisement = 151, // [RFC4286]
        MulticastRouterSolicitation = 152, // [RFC4286]
        MulticastRouterTermination = 153, // [RFC4286]
        FmIPv6Messages = 154, // [RFC5568]
        RplControlMessage = 155, // [RFC6550]
        IlnpV6LocatorUpdateMessage = 156, // [RFC6743]
        DuplicateAddressRequest = 157, // [RFC6775]
        DuplicateAddressConfirmation = 158, // [RFC6775]
        MplControlMessage = 159, // [RFC7731]
        ExtendedEchoRequest = 160, // [RFC8335]
        ExtendedEchoReply = 161, // [RFC8335]
        PrivateExperimentation3 = 200, // [RFC4443]
        PrivateExperimentation4 = 201, // [RFC4443]
        ReservedForExpansion2 = 255 // [RFC4443]
    }