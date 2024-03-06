using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapVhtMcsNss : byte
{
    /// <summary>
    /// Number of Spatial Streams
    /// </summary>
    Nss = 0x0f,

    /// <summary>
    /// MCS rate index
    /// </summary>
    Mcs = 0xf0
}
