using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public  enum RadioTapVhtFlags : byte
{
    /// <summary>
    /// Space-time block coding. 
    /// Set to 0 if no spatial streams of any user has STBC.
    /// Set to 1 if all spatial streams of all users have STBC.
    /// </summary>
    Stbc = 0x01,

    /// <summary>
    /// Valid only for AP transmitters.
    /// Set to 0 if STAs may doze during TXOP.
    /// Set to 1 if STAs may not doze during TXOP or transmitter is non-AP.
    /// </summary>
    TxopPsNotAllowed = 0x02,

    /// <summary>
    /// Set to 0 for long GI.
    /// Set to 1 for short GI.
    /// </summary>
    GuardInterval = 0x04,

    /// <summary>
    /// Valid only if short GI is used.
    /// Set to 0 if NSYM mod 10 != 9 or short GI not used.
    /// Set to 1 if NSYM mod 10 = 9.
    /// </summary>
    ShortGiNsymDisambiguation = 0x08,

    /// <summary>
    /// Set to 1 if one or more users are using LDPC and the encoding process resulted in extra OFDM symbol(s).
    /// Set to 0 otherwise.
    /// </summary>
    LdpcExtraOfdmSymbol = 0x10,

    /// <summary>
    /// Valid only for SU PPDUs
    /// </summary>
    Beamformed = 0x20
}
