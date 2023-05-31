namespace PacketDotNet.Ieee80211;

    public struct QosNullDataFrameFields
    {
        public static readonly int QosControlLength = 2;
        public static readonly int QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
    }