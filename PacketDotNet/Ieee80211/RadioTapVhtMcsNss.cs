using System;

namespace PacketDotNet.Ieee80211;

/// <summary>
/// MCS rate index and spatial streams for one user
/// </summary>
[Flags]
public enum RadioTapVhtMcsNss : byte
{
    /// <summary>
    /// Number of Spatial Streams (1-8)
    /// </summary>
    Nss = 0x0f,

    /// <summary>
    /// MCS rate index
    /// </summary>
    Mcs = 0xf0
}
