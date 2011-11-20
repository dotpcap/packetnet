using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    public class Ieee80211ProbeResponseFrame : Ieee80211ManagementFrame
    {
        private class Ieee80211ProbeResponseFields
        {
            public readonly static int TimestampLength = 8;
            public readonly static int BeaconIntervalLength = 2;
            public readonly static int CapabilityInformationLength = 2;

            public readonly static int TimestampPosition;
            public readonly static int BeaconIntervalPosition;
            public readonly static int CapabilityInformationPosition;
            public readonly static int InformationElement1Position;

            static Ieee80211ProbeResponseFields()
            {
                TimestampPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
                BeaconIntervalPosition = TimestampPosition + TimestampLength;
                CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
            }
        }

        public UInt64 Timestamp
        {
            get
            {
                return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + Ieee80211ProbeResponseFields.TimestampPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211ProbeResponseFields.TimestampPosition);
            }
        }

        public UInt16 BeaconInterval
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + Ieee80211ProbeResponseFields.BeaconIntervalPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211ProbeResponseFields.BeaconIntervalPosition);
            }
        }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        public UInt16 CapabilityInformationBytes
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211ProbeResponseFields.CapabilityInformationPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211ProbeResponseFields.CapabilityInformationPosition);
            }
        }

        public Ieee80211CapabilityInformationField CapabilityInformation
        {
            get;
            set;
        }


        public Ieee80211InformationElementSection InformationElements { get; set; }

        public override int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * 3) +
                    Ieee80211MacFields.SequenceControlLength +
                    Ieee80211ProbeResponseFields.TimestampLength +
                    Ieee80211ProbeResponseFields.BeaconIntervalLength +
                    Ieee80211ProbeResponseFields.CapabilityInformationLength +
                    InformationElements.Length);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211ProbeResponseFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);
            CapabilityInformation = new Ieee80211CapabilityInformationField(CapabilityInformationBytes);

            //create a segment that just refers to the info element section
            ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                (bas.Offset + Ieee80211ProbeResponseFields.InformationElement1Position),
                (bas.Length - Ieee80211ProbeResponseFields.InformationElement1Position - Ieee80211MacFields.FrameCheckSequenceLength));

            InformationElements = new Ieee80211InformationElementSection(infoElementsSegment);

            //cant set length until after we have handled the information elements
            //as they vary in length
            header.Length = FrameSize;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [ProbeResponseFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }
    }
}
