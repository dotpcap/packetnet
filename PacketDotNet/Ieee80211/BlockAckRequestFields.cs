using System;

namespace PacketDotNet.Ieee80211
{
    public class BlockAckRequestFields
    {
        public static readonly Int32 BlockAckRequestControlLength = 2;

        public static readonly Int32 BlockAckRequestControlPosition;
        public static readonly Int32 BlockAckStartingSequenceControlLength = 2;
        public static readonly Int32 BlockAckStartingSequenceControlPosition;

        static BlockAckRequestFields()
        {
            BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
            BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
        }
    }
}