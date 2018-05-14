using System;

namespace PacketDotNet.Ieee80211
{
    public class ReassociationRequestFields
    {
        public static readonly Int32 CapabilityInformationLength = 2;
        public static readonly Int32 CapabilityInformationPosition;
        public static readonly Int32 CurrentAccessPointPosition;
        public static readonly Int32 InformationElement1Position;
        public static readonly Int32 ListenIntervalLength = 2;
        public static readonly Int32 ListenIntervalPosition;

        static ReassociationRequestFields()
        {
            CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
            CurrentAccessPointPosition = ListenIntervalPosition + ListenIntervalLength;
            InformationElement1Position = CurrentAccessPointPosition + MacFields.AddressLength;
        }
    }
}