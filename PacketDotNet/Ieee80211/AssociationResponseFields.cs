using System;

namespace PacketDotNet.Ieee80211
{
    public class AssociationResponseFields
    {
        public static readonly Int32 AssociationIdLength = 2;
        public static readonly Int32 AssociationIdPosition;
        public static readonly Int32 CapabilityInformationLength = 2;

        public static readonly Int32 CapabilityInformationPosition;
        public static readonly Int32 InformationElement1Position;
        public static readonly Int32 StatusCodeLength = 2;
        public static readonly Int32 StatusCodePosition;

        static AssociationResponseFields()
        {
            CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            StatusCodePosition = CapabilityInformationPosition + CapabilityInformationLength;
            AssociationIdPosition = StatusCodePosition + StatusCodeLength;
            InformationElement1Position = AssociationIdPosition + AssociationIdLength;
        }
    }
}