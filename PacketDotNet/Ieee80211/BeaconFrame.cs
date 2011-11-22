using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Format of an 802.11 management beacon frame.
        /// 
        /// Beacon frames are used to annouce the existance of a wireless network. If an
        /// access point has been configured to not broadcast its SSID then it may not transmit
        /// beacon frames.
        /// </summary>
        public class BeaconFrame : ManagementFrame
        {
            private class BeaconFields
            {
                public readonly static int TimestampLength = 8;
                public readonly static int BeaconIntervalLength = 2;
                public readonly static int CapabilityInformationLength = 2;

                public readonly static int TimestampPosition;
                public readonly static int BeaconIntervalPosition;
                public readonly static int CapabilityInformationPosition;
                public readonly static int InformationElement1Position;

                static BeaconFields()
                {
                    TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    BeaconIntervalPosition = TimestampPosition + TimestampLength;
                    CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                    InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
                }
            }

            /// <summary>
            /// The number of microseconds the networks master timekeeper has been active.
            /// 
            /// Used for synchronisation between stations in an IBSS. When it reaches the maximum value the timestamp will wrap (not very likely).
            /// </summary>
            public UInt64 Timestamp
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + BeaconFields.TimestampPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.TimestampPosition);
                }
            }

            /// <summary>
            /// The number of "time units" between beacon frames.
            /// 
            /// A time unit is 1,024 microseconds. This interval is usually set to 100 which equates to approximately 100 milliseconds or 0.1 seconds.
            /// </summary>
            public UInt16 BeaconInterval
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + BeaconFields.BeaconIntervalPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.BeaconIntervalPosition);
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
                                                          header.Offset + BeaconFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.CapabilityInformationPosition);
                }
            }

            /// <summary>
            /// Defines the capabilities of the network.
            /// </summary>
            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }


            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        BeaconFields.TimestampLength +
                        BeaconFields.BeaconIntervalLength +
                        BeaconFields.CapabilityInformationLength +
                        InformationElements.Length);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// 
            /// Most (but not all) beacons frames will contain an Information element that contains the SSID.
            /// </summary>
            public InformationElementSection InformationElements { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public BeaconFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                SequenceControl = new SequenceControlField(SequenceControlBytes);
                CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + BeaconFields.InformationElement1Position),
                    (bas.Length - BeaconFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementSection(infoElementsSegment);

                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }

            public BeaconFrame(FrameControlField frameControl,
                DurationField duration,
                PhysicalAddress source,
                PhysicalAddress destination,
                PhysicalAddress bssId,
                SequenceControlField sequenceControl,
                UInt64 timestamp,
                UInt16 beaconInterval,
                CapabilityInformationField capabilityInfo,
                List<InformationElement> infoElements)
            {
                //need to handle information elements first as they dictate the length of the frame
                InformationElementSection infoElementSection = new InformationElementSection(infoElements);
                header = new ByteArraySegment(new Byte[BeaconFields.InformationElement1Position + infoElementSection.Length]);

                FrameControlBytes = frameControl.Field;
                DurationBytes = duration.Field;
                SequenceControlBytes = sequenceControl.Field;
                SourceAddress = source;
                DestinationAddress = destination;
                BssId = bssId;
                Timestamp = timestamp;
                BeaconInterval = beaconInterval;
                CapabilityInformationBytes = capabilityInfo.Field;

                Byte[] infoElementBuffer = infoElementSection.Bytes;
                Array.Copy(infoElementBuffer, 0, header.Bytes, BeaconFields.InformationElement1Position, infoElementSection.Length);
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [BeaconFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
