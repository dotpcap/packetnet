using System;

namespace PacketDotNet.Ieee80211
{
    public static class DisassociationFrameFields
    {
        public static readonly Int32 ReasonCodeLength = 2;

        public static readonly Int32 ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
    }
}