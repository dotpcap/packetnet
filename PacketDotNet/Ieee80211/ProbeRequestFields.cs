using System;

namespace PacketDotNet.Ieee80211
{
    public class ProbeRequestFields
    {
        public static readonly Int32 InformationElement1Position;

        static ProbeRequestFields()
        {
            InformationElement1Position = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
        }
    }
}