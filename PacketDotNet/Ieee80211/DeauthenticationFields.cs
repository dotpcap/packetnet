using System;

namespace PacketDotNet.Ieee80211
{
    public class DeauthenticationFields
    {
        public static readonly Int32 ReasonCodeLength = 2;

        public static readonly Int32 ReasonCodePosition;

        static DeauthenticationFields()
        {
            ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
        }
    }
}