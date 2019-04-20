namespace PacketDotNet.Ieee80211
{
    public static class DisassociationFrameFields
    {
        public static readonly int ReasonCodeLength = 2;

        public static readonly int ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
    }
}