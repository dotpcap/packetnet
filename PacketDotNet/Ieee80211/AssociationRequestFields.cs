using System;

namespace PacketDotNet.Ieee80211
{
    public class AssociationRequestFields
    {
        public static readonly Int32 CapabilityInformationLength = 2;

        public static readonly Int32 CapabilityInformationPosition;
        public static readonly Int32 InformationElement1Position;
        public static readonly Int32 ListenIntervalLength = 2;
        public static readonly Int32 ListenIntervalPosition;

        static AssociationRequestFields()
        {
            CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
            InformationElement1Position = ListenIntervalPosition + ListenIntervalLength;
        }
    }
}