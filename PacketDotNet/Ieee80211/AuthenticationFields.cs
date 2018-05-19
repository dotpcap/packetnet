using System;

namespace PacketDotNet.Ieee80211
{
    public class AuthenticationFields
    {
        public static readonly Int32 AuthAlgorithmNumLength = 2;
        public static readonly Int32 AuthAlgorithmNumPosition;
        public static readonly Int32 AuthAlgorithmTransactionSequenceNumLength = 2;
        public static readonly Int32 AuthAlgorithmTransactionSequenceNumPosition;
        public static readonly Int32 InformationElement1Position;
        public static readonly Int32 StatusCodeLength = 2;
        public static readonly Int32 StatusCodePosition;

        static AuthenticationFields()
        {
            AuthAlgorithmNumPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            AuthAlgorithmTransactionSequenceNumPosition = AuthAlgorithmNumPosition + AuthAlgorithmNumLength;
            StatusCodePosition = AuthAlgorithmTransactionSequenceNumPosition + AuthAlgorithmTransactionSequenceNumLength;
            InformationElement1Position = StatusCodePosition + StatusCodeLength;
        }
    }
}