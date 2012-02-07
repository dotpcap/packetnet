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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Specifies the reasons why a station may have been disassociated or deauthenticated by an access point.
        /// </summary>
        public enum ReasonCode
        {
            /// <summary>
            /// No reason was given.
            /// </summary>
            None = 0,
            /// <summary>
            /// The reason was not specified.
            /// </summary>
            Unspecified = 1,
            /// <summary>
            /// The previous authentication was no longer valid.
            /// </summary>
            AuthNoLongerValid = 2,
            /// <summary>
            /// The station is leaving (or has left) the IBSS or ESS.
            /// </summary>
            Leaving = 3,
            /// <summary>
            /// The station has been disassociated due to inactivity.
            /// </summary>
            Inactivity = 4,
            /// <summary>
            /// The access point is unable to handle anymore associated stations.
            /// </summary>
            OutOfResources = 5,
            /// <summary>
            /// The station needs to be authenticated.
            /// </summary>
            NeedAuthentication = 6,
            /// <summary>
            /// The station needs to be associated.
            /// </summary>
            NeedAssociation = 7,
            /// <summary>
            /// The station is leaving the BSS.
            /// </summary>
            LeavingToRoam = 8,
            /// <summary>
            /// Association is invalid because the station is not authenticated.
            /// </summary>
            AssociationInvalid = 9,
            /// <summary>
            /// The Power Capability information is unacceptable.
            /// </summary>
            BadPower = 10,
            /// <summary>
            /// The Supported Channels information is unacceptable.
            /// </summary>
            BadChannels = 11,
            /// <summary>
            /// An invalid information element has been provided.
            /// </summary>
            InvalidInformationElement = 13,
            /// <summary>
            /// The message integrity check failed.
            /// </summary>
            MessageIntegrityCheckFailure = 14,
            /// <summary>
            /// The 4way handshake has timed out.
            /// </summary>
            FourwayHandshakeTimeout = 15,
            /// <summary>
            /// The Group Key handshake has timed out.
            /// </summary>
            GroupKeyHandshakeTimeout = 16,
            /// <summary>
            /// An information element in the 4way handshake differs from in previous management frames.
            /// </summary>
            FourwayHandshakeInvalid = 17,
            /// <summary>
            /// The group cipher is invalid.
            /// </summary>
            GroupCipherInvalid = 18,
            /// <summary>
            /// The pairwise cipher is invalid.
            /// </summary>
            PairwiseCipherInvalid = 19,
            /// <summary>
            /// The Authentication Key Managment Protocol is invalid.
            /// </summary>
            AuthenticationKeyManagmentProtocolInvalid = 20,
            /// <summary>
            /// The provided RSN information element version is unsupported.
            /// </summary>
            RsnVersionUnsupported = 21,
            /// <summary>
            /// The provided RSN information element capabilities are invalid.
            /// </summary>
            RsnCapabilitiesInvalid = 22,
            /// <summary>
            /// There has been an IEEE 802.1X authentication failure.
            /// </summary>
            Ieee8021XFailure = 23,
            /// <summary>
            /// The cipher suite has been rejected due to the security policy.
            /// </summary>
            CipherRejected = 24,
            /// <summary>
            /// The station has been disassociated due to an unspecified QoS related reason.
            /// </summary>
            QosUnspecified = 32,
            /// <summary>
            /// The access point lacks sufficient bandwidth for the station.
            /// </summary>
            QosOutOfResources = 33,
            /// <summary>
            /// An excessive number of frames have failed to be acknowledged due to poor channel conditions.
            /// </summary>
            PoorChannelConditions = 34,
            /// <summary>
            /// The station is transmitting outside the limits of its TXOPs.
            /// </summary>
            InvalidTxop = 35,
            /// <summary>
            /// Disassociation was requested by the station as it is leaving the BSS.
            /// </summary>
            RequestedLeaving = 36,
            /// <summary>
            /// Disassociation was requested by the station as it is no longer wants to use the mechanism.
            /// </summary>
            RequestedNoUse = 37,
            /// <summary>
            /// Disassociation was requested by the station as it requires setup to use the mechanism.
            /// </summary>
            RequestedNeedsSetup = 38,
            /// <summary>
            /// Disassociation was requested by the station due to a timeout.
            /// </summary>
            RequestedDueToTimeout = 39,
            /// <summary>
            /// The station does not support the cipher suite.
            /// </summary>
            CipherUnsupported = 45
        } 
    }
}
