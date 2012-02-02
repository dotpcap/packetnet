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
        public enum ReasonCode
        {
            None = 0,
            Unspecified = 1,
            AuthNoLongerValid = 2,
            Leaving = 3,
            Inactivity = 4,
            OutOfResources = 5,
            NeedAuthentication = 6,
            NeedAssociation = 7,
            LeavingToRoam = 8,
            AssociationInvalid = 9,
            BadPower = 10,
            BadChannels = 11,
            InvalidInformationElement = 13,
            MessageIntegrityCheckFailure = 14,
            FourwayHandshakeTimeout = 15,
            GroupKeyHandshakeTimeout = 16,
            FourwayHandshakeInvalid = 17,
            GroupCipherInvalid = 18,
            PairwiseCipherInvalid = 19,
            AuthenticationKeyManagmentProtocolInvalid = 20,
            RsnVersionUnsupported = 21,
            RsnCapabilitiesInvalid = 22,
            Ieee8021XFailure = 23,
            CipherRejected = 24,
            QosUnspecified = 32,
            QosOutOfResources = 33,
            PoorChannelConditions = 34,
            InvalidTxop = 35,
            RequestedLeaving = 36,
            RequestedNoUse = 37,
            RequestedNeedsSetup = 38,
            RequestedDueToTimeout = 39,
            CipherUnsupported = 45
        } 
    }
}
