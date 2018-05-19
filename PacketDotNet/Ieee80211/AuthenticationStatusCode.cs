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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The potential results of authentication or association operations.
    /// </summary>
    public enum AuthenticationStatusCode
    {
        /// <summary>
        /// Constant success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Constant unspecified failure.
        /// </summary>
        UnspecifiedFailure = 1,

        /// <summary>
        /// Constant requested capability unsupportable.
        /// </summary>
        RequestedCapabilityUnsupportable = 10,

        /// <summary>
        /// Constant unidentifiable prior association.
        /// </summary>
        UnidentifiablePriorAssociation = 11,

        /// <summary>
        /// Constant non standard unspecified denial.
        /// </summary>
        NonStandardUnspecifiedDenial = 12,

        /// <summary>
        /// Constant authentication algorithm not supported.
        /// </summary>
        AuthenticationAlgorithmNotSupported = 13,

        /// <summary>
        /// Constant unexpected sequenec number.
        /// </summary>
        UnexpectedSequenecNumber = 14,

        /// <summary>
        /// Constant response to challenge failed.
        /// </summary>
        ResponseToChallengeFailed = 15,

        /// <summary>
        /// Constant next frame outside expected window.
        /// </summary>
        NextFrameOutsideExpectedWindow = 16,

        /// <summary>
        /// Constant access point resource constrained.
        /// </summary>
        AccessPointResourceConstrained = 17,

        /// <summary>
        /// Constant station doesnt support data rates.
        /// </summary>
        StationDoesntSupportDataRates = 18,

        /// <summary>
        /// Constant station doesnt support preamble.
        /// </summary>
        StationDoesntSupportPreamble = 19,

        /// <summary>
        /// Constant station doesnt support pbcc modulation.
        /// </summary>
        StationDoesntSupportPbccModulation = 20,

        /// <summary>
        /// Constant station doesnt support channel agility.
        /// </summary>
        StationDoesntSupportChannelAgility = 21,

        /// <summary>
        /// Constant spectrum management required.
        /// </summary>
        SpectrumManagementRequired = 22,

        /// <summary>
        /// Constant unacceptable power capability value.
        /// </summary>
        UnacceptablePowerCapabilityValue = 23,

        /// <summary>
        /// Constant unacceptable supported channels value.
        /// </summary>
        UnacceptableSupportedChannelsValue = 24,

        /// <summary>
        /// Constant station doesnt support short time slot.
        /// </summary>
        StationDoesntSupportShortTimeSlot = 25,

        /// <summary>
        /// Constant station doesnt support dsss ofdm.
        /// </summary>
        StationDoesntSupportDsssOfdm = 26,

        /// <summary>
        /// Constant invalid information element.
        /// </summary>
        InvalidInformationElement = 40,

        /// <summary>
        /// Constant invalid group cipher.
        /// </summary>
        InvalidGroupCipher = 41,

        /// <summary>
        /// Constant invalid pairwise cipher.
        /// </summary>
        InvalidPairwiseCipher = 42,

        /// <summary>
        /// Constant invalid authentication and key management protocol.
        /// </summary>
        InvalidAuthenticationAndKeyManagementProtocol = 43,

        /// <summary>
        /// Constant unsupported rsn information element version.
        /// </summary>
        UnsupportedRsnInformationElementVersion = 44,

        /// <summary>
        /// Constant unsupported rsn ie capabilities.
        /// </summary>
        UnsupportedRsnIeCapabilities = 45,

        /// <summary>
        /// Constant cipher suite policy rejection.
        /// </summary>
        CipherSuitePolicyRejection = 46
    }
}