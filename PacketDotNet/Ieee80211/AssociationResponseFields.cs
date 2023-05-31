namespace PacketDotNet.Ieee80211;

    public struct AssociationResponseFields
    {
        public static readonly int AssociationIdLength = 2;
        public static readonly int AssociationIdPosition;
        public static readonly int CapabilityInformationLength = 2;
        public static readonly int CapabilityInformationPosition;
        public static readonly int InformationElement1Position;
        public static readonly int StatusCodeLength = 2;
        public static readonly int StatusCodePosition;

        static AssociationResponseFields()
        {
            CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            StatusCodePosition = CapabilityInformationPosition + CapabilityInformationLength;
            AssociationIdPosition = StatusCodePosition + StatusCodeLength;
            InformationElement1Position = AssociationIdPosition + AssociationIdLength;
        }
    }