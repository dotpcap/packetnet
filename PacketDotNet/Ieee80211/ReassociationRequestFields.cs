namespace PacketDotNet.Ieee80211
{
    public class ReassociationRequestFields
    {
        public static readonly int CapabilityInformationLength = 2;
        public static readonly int CapabilityInformationPosition;
        public static readonly int CurrentAccessPointPosition;
        public static readonly int InformationElement1Position;
        public static readonly int ListenIntervalLength = 2;
        public static readonly int ListenIntervalPosition;

        static ReassociationRequestFields()
        {
            CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
            CurrentAccessPointPosition = ListenIntervalPosition + ListenIntervalLength;
            InformationElement1Position = CurrentAccessPointPosition + MacFields.AddressLength;
        }
    }
}