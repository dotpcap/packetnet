using System;

namespace PacketDotNet.Ieee80211
{
    public static class QosNullDataFrameFields
    {
        public static readonly Int32 QosControlLength = 2;

        public static readonly Int32 QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
    }
}