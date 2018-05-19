using System;

namespace PacketDotNet.Ieee80211
{
    public class BeaconFields
    {
        public static readonly Int32 BeaconIntervalLength = 2;
        public static readonly Int32 BeaconIntervalPosition;
        public static readonly Int32 CapabilityInformationLength = 2;
        public static readonly Int32 CapabilityInformationPosition;
        public static readonly Int32 InformationElement1Position;
        public static readonly Int32 TimestampLength = 8;
        public static readonly Int32 TimestampPosition;

        static BeaconFields()
        {
            TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            BeaconIntervalPosition = TimestampPosition + TimestampLength;
            CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
            InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
        }
    }
}