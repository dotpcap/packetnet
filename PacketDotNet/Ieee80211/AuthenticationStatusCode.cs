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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// The potential results of authentication or association operations.
        /// </summary>
        public enum AuthenticationStatusCode
        {
            Success = 0,
            UnspecifiedFailure = 1,
            RequestedCapabilityUnsupportable = 10,
            UnidentifiablePriorAssociation = 11,
            NonStandardUnspecifiedDenial = 12,
            AuthenticationAlgorithmNotSupported = 13,
            UnexpectedSequenecNumber = 14,
            ResponseToChallengeFailed = 15,
            NextFrameOutsideExpectedWindow = 16,
            AccessPointResourceConstrained = 17,
            StationDoesntSupportDataRates = 18,
            StationDoesntSupportPreamble = 19,
            StationDoesntSupportPbccModulation = 20,
            StationDoesntSupportChannelAgility = 21,
            SpectrumManagementRequired = 22,
            UnacceptablePowerCapabilityValue = 23,
            UnacceptableSupportedChannelsValue = 24,
            StationDoesntSupportShortTimeSlot = 25,
            StationDoesntSupportDsssOfdm = 26,
            InvalidInformationElement = 40,
            InvalidGroupCipher = 41,
            InvalidPairwiseCipher = 42,
            InvalidAuthenticationAndKeyManagementProtocol = 43,
            UnsupportedRsnInformationElementVersion = 44,
            UnsupportedRsnIeCapabilities = 45,
            CipherSuitePolicyRejection = 46
        } 
    }
}
