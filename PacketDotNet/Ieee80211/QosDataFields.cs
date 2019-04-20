namespace PacketDotNet.Ieee80211
{
    public class QosDataFields
    {
        public static readonly int QosControlLength = 2;

        public static readonly int QosControlPosition;

        static QosDataFields()
        {
            QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
        }
    }
}