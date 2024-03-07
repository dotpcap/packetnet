using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public  enum RadioTapVhtCoding : byte
{
    /// <summary>
    /// Coding for user 1
    /// 0: BCC
    /// 1: LDPC
    /// </summary>
    CodingForUser1 = 0x01,

    /// <summary>
    /// Coding for user 2
    /// 0: BCC
    /// 1: LDPC
    /// </summary>
    CodingForUser2 = 0x02,

    /// <summary>
    /// Coding for user 3
    /// 0: BCC
    /// 1: LDPC
    /// </summary>
    CodingForUser3 = 0x03,

    /// <summary>
    /// Coding for user 4
    /// 0: BCC
    /// 1: LDPC
    /// </summary>
    CodingForUser4 = 0x04,

    /// <summary>
    /// Unused.
    /// </summary>
    Unused = 0xf0
}
