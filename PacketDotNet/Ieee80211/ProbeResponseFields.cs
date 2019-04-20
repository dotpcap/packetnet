using System;

namespace PacketDotNet.Ieee80211
{
    public class ProbeResponseFields
    {
        public static readonly int BeaconIntervalLength = 2;
        public static readonly int BeaconIntervalPosition;
        public static readonly int CapabilityInformationLength = 2;
        public static readonly int CapabilityInformationPosition;
        public static readonly int InformationElement1Position;
        public static readonly int TimestampLength = 8;

        public static readonly int TimestampPosition;

        static ProbeResponseFields()
        {
            TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            BeaconIntervalPosition = TimestampPosition + TimestampLength;
            CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
            InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
        }
    }
}