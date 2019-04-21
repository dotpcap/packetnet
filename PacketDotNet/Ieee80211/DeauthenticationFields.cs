namespace PacketDotNet.Ieee80211
{
    public struct DeauthenticationFields
    {
        public static readonly int ReasonCodeLength = 2;

        public static readonly int ReasonCodePosition;

        static DeauthenticationFields()
        {
            ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
        }
    }
}