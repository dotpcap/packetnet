using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketDotNet.Ieee80211;

[Flags]
public enum RadioTapHighEfficiencyData6 : ushort
{
    /// <summary>
    /// NSTS (actual number of space-time streams, 0=unknown, 1=1, etc.
    /// </summary>
    Nsts = 0x000f,

    /// <summary>
    /// Doppler value
    /// </summary>
    DopplerValue = 0x0010,

    /// <summary>
    /// Reserved.
    /// </summary>
    Reserved = 0x00e0,

    /// <summary>
    /// TXOP Value
    /// </summary>
    TxopValue = 0x7f00,

    /// <summary>
    /// Midamble periodicity 
    /// 0=10
    /// 1=20
    /// </summary>
    MidamblePeriodicity = 0x8000,
}