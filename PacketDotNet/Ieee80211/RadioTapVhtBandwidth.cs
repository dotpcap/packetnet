namespace PacketDotNet.Ieee80211;


public enum RadioTapVhtBandwidth : byte
{
    /// <summary>
    /// 20MHz bandwidth
    /// </summary>
    MHz20 = 0x00,

    /// <summary>
    /// 40MHz bandwidth
    /// </summary>
    MHz40 = 0x01,

    /// <summary>
    /// 40MHz bandwidth with 20L side band. Sideband index=0
    /// </summary>
    MHz40Side20L = 2,

    /// <summary>
    /// 40MHz bandwidth with 20U side band. Sideband index=1
    /// </summary>
    MHz40Side20U = 3,

    /// <summary>
    /// 80MHz bandwidth
    /// </summary>
    MHz80 = 4,
    MHz80Side40L = 5,
    MHz80Side40U = 6,
    MHz80Side20LL = 7,
    MHz80Side20LU = 8,
    MHz80Side20UL = 9,
    MHz80Side20UU = 10,

    /// <summary>
    /// 160MHz bandwidth
    /// </summary>
    MHz160 = 11,
    MHz160Side80L = 12,
    MHz160Side80U = 13,
    MHz160Side40LL = 14,
    MHz160Side40LU = 15,
    MHz160Side40UL = 16,
    MHz160Side40UU = 17,
    MHz160Side20LLL = 18,
    MHz160Side20LLU = 19,
    MHz160Side20LUL = 20,
    MHz160Side20LUU = 21,
    MHz160Side20ULL = 22,
    MHz160Side20ULU = 23,
    MHz160Side20UUL = 24,
    MHz160Side20UUU = 25,
}
