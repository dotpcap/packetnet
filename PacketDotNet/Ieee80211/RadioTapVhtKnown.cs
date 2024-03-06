using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapVhtKnown : ushort
{
    StbcKnown                       = 0x0001,
    TxopPsNotAllowedKnown           = 0x0002,
    GuardIntervalKnown              = 0x0004,
    ShortGiNsymDisambiguationKnown  = 0x0008,
    LdpcExtraOfdmSymbolKnown        = 0x0010,
    BeamformedKnown                 = 0x0020,
    BandwidthKnown                  = 0x0040,
    GroupIdKnown                    = 0x0080,
    PartialAidKnown                 = 0x0100
}
