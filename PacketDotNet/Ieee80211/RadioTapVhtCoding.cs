using System;

namespace PacketDotNet.Ieee80211;

[Flags]
public  enum RadioTapVhtCoding : byte
{
    CodingForUser1 = 0x01,
    CodingForUser2 = 0x02,
    CodingForUser3 = 0x03,
    CodingForUser4 = 0x04,

    Unused = 0xf0
}
