namespace PacketDotNet.Ieee80211
{
    public class BlockAckRequestFields
    {
        public static readonly int BlockAckRequestControlLength = 2;

        public static readonly int BlockAckRequestControlPosition;
        public static readonly int BlockAckStartingSequenceControlLength = 2;
        public static readonly int BlockAckStartingSequenceControlPosition;

        static BlockAckRequestFields()
        {
            BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
            BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
        }
    }
}