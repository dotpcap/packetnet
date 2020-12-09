/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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
        /// Constant unexpected sequence number.
        /// </summary>
        UnexpectedSequenceNumber = 14,

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
        /// Constant station doesn't support data rates.
        /// </summary>
        StationDoesntSupportDataRates = 18,

        /// <summary>
        /// Constant station doesn't support preamble.
        /// </summary>
        StationDoesntSupportPreamble = 19,

        /// <summary>
        /// Constant station doesn't support pbcc modulation.
        /// </summary>
        StationDoesntSupportPbccModulation = 20,

        /// <summary>
        /// Constant station doesn't support channel agility.
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
        /// Constant station doesn't support short time slot.
        /// </summary>
        StationDoesntSupportShortTimeSlot = 25,

        /// <summary>
        /// Constant station doesn't support dsss ofdm.
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