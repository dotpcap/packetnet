namespace PacketDotNet.Ieee80211
{
    public struct BlockAcknowledgmentFields
    {
        public static readonly int BlockAckBitmapPosition;
        public static readonly int BlockAckRequestControlLength = 2;
        public static readonly int BlockAckRequestControlPosition;
        public static readonly int BlockAckStartingSequenceControlLength = 2;
        public static readonly int BlockAckStartingSequenceControlPosition;

        static BlockAcknowledgmentFields()
        {
            BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
            BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
            BlockAckBitmapPosition = BlockAckStartingSequenceControlPosition + BlockAckStartingSequenceControlLength;
        }
    }
}