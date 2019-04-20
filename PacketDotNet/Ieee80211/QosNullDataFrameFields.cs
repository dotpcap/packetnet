namespace PacketDotNet.Ieee80211
{
    public static class QosNullDataFrameFields
    {
        public static readonly int QosControlLength = 2;

        public static readonly int QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
    }
}