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

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// ICMPv6 types, see http://en.wikipedia.org/wiki/ICMPv6 and
    /// http://www.iana.org/assignments/icmpv6-parameters
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ICMPv6Types : byte
    {
#pragma warning disable 1591


        #region ICMPv6 Error Messages

        DestinationUnreachable = 1, // [RFC4443]
        PacketTooBig = 2, // [RFC4443]
        TimeExceeded = 3, // [RFC4443]
        ParameterProblem = 4, // [RFC4443]
        PrivateExperimentation1 = 100, // [RFC4443]
        PrivateExperimentation2 = 101, // [RFC4443]
        ReservedForExpansion1 = 127, // [RFC4443]

        #endregion


        #region ICMPv6 Informational Messages

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
        FMIPv6Messages = 154, // [RFC5568]
        RPLControlMessage = 155, // [RFC6550]
        ILNPv6LocatorUpdateMessage = 156, // [RFC6743]
        DuplicateAddressRequest = 157, // [RFC6775]
        DuplicateAddressConfirmation = 158, // [RFC6775]
        MPLControlMessage = 159, // [RFC7731]
        ExtendedEchoRequest = 160, // [RFC8335]
        ExtendedEchoReply = 161, // [RFC8335]
        PrivateExperimentation3 = 200, // [RFC4443]
        PrivateExperimentation4 = 201, // [RFC4443]
        ReservedForExpansion2 = 255 // [RFC4443]

        #endregion


#pragma warning restore 1591
    }
}