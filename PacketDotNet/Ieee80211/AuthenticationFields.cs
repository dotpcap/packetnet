using System;

namespace PacketDotNet.Ieee80211
{
    public class AuthenticationFields
    {
        public static readonly int AuthAlgorithmNumLength = 2;
        public static readonly int AuthAlgorithmNumPosition;
        public static readonly int AuthAlgorithmTransactionSequenceNumLength = 2;
        public static readonly int AuthAlgorithmTransactionSequenceNumPosition;
        public static readonly int InformationElement1Position;
        public static readonly int StatusCodeLength = 2;
        public static readonly int StatusCodePosition;

        static AuthenticationFields()
        {
            AuthAlgorithmNumPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            AuthAlgorithmTransactionSequenceNumPosition = AuthAlgorithmNumPosition + AuthAlgorithmNumLength;
            StatusCodePosition = AuthAlgorithmTransactionSequenceNumPosition + AuthAlgorithmTransactionSequenceNumLength;
            InformationElement1Position = StatusCodePosition + StatusCodeLength;
        }
    }
}