using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    public enum Ieee80211ReasonCode
    {
        NONE = 0,
        UNSPECIFIED = 1,
        AUTH_NO_LONGER_VALID = 2,
        LEAVING = 3,
        INACTIVITY = 4,
        OUT_OF_RESOURCES = 5,
        NEED_AUTH = 6,
        NEED_ASSOC = 7,
        LEAVING_TO_ROAM = 8,
        REASSOC_INVALID = 9,
        BAD_POWER = 10,
        BAD_CHANNELS = 11,
        INVALID_IE = 13,
        MIC_FAILURE = 14,
        FOURWAY_TIMEOUT = 15,
        GROUPKEY_TIMEOUT = 16,
        FOURWAY_INVALID = 17,
        GROUP_CIPHER_INVALID = 18,
        PAIR_CIPHER_INVALID = 19,
        AKMP_INVALID = 20,
        RSN_VERSION_INVALID = 21,
        RSN_CAPAB_INVALID = 22,
        IEE8021X_FAILURE = 23,
        CIPHER_REJECTED = 24,
        QOS_UNSPECIFIED = 32,
        QOS_OUT_OF_RESOURCES = 33,
        LINK_IS_HORRIBLE = 34,
        INVALID_TXOP = 35,
        REQUESTED_LEAVING = 36,
        REQUESTED_NO_USE = 37,
        REQUESTED_NEED_SETUP = 38,
        REQUESTED_TIMEOUT = 39,
        CIPHER_UNSUPPORTED = 45
    }
}
