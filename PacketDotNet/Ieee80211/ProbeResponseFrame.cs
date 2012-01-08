using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class ProbeResponseFrame : ManagementFrame
        {
            private class ProbeResponseFields
            {
                public readonly static int TimestampLength = 8;
                public readonly static int BeaconIntervalLength = 2;
                public readonly static int CapabilityInformationLength = 2;

                public readonly static int TimestampPosition;
                public readonly static int BeaconIntervalPosition;
                public readonly static int CapabilityInformationPosition;
                public readonly static int InformationElement1Position;

                static ProbeResponseFields()
                {
                    TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    BeaconIntervalPosition = TimestampPosition + TimestampLength;
                    CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                    InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
                }
            }

            public UInt64 Timestamp
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + ProbeResponseFields.TimestampPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + ProbeResponseFields.TimestampPosition);
                }
            }

            public UInt16 BeaconInterval
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + ProbeResponseFields.BeaconIntervalPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + ProbeResponseFields.BeaconIntervalPosition);
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
                                                          header.Offset + ProbeResponseFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + ProbeResponseFields.CapabilityInformationPosition);
                }
            }

            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }


            public InformationElementList InformationElements { get; set; }

            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        ProbeResponseFields.TimestampLength +
                        ProbeResponseFields.BeaconIntervalLength +
                        ProbeResponseFields.CapabilityInformationLength +
                        InformationElements.Length);
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public ProbeResponseFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                SequenceControl = new SequenceControlField(SequenceControlBytes);
                CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + ProbeResponseFields.InformationElement1Position),
                    (bas.Length - ProbeResponseFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementList(infoElementsSegment);

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
}
