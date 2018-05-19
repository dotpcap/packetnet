using System;

namespace PacketDotNet.Ieee80211
{
    public class QosDataFields
    {
        public static readonly Int32 QosControlLength = 2;

        public static readonly Int32 QosControlPosition;

        static QosDataFields()
        {
            QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
        }
    }
}