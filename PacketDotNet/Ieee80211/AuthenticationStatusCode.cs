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
