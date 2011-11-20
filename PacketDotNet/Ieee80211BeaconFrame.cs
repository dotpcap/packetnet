using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Format of an 802.11 management beacon frame.
    /// 
    /// Beacon frames are used to annouce the existance of a wireless network. If an
    /// access point has been configured to not broadcast its SSID then it may not transmit
    /// beacon frames.
    /// </summary>
    public class Ieee80211BeaconFrame : Ieee80211ManagementFrame
    {
        private class Ieee80211BeaconFields
        {
            public readonly static int TimestampLength = 8;
            public readonly static int BeaconIntervalLength = 2;
            public readonly static int CapabilityInformationLength = 2;

            public readonly static int TimestampPosition;
            public readonly static int BeaconIntervalPosition;
            public readonly static int CapabilityInformationPosition;
            public readonly static int InformationElement1Position;

            static Ieee80211BeaconFields()
            {
                TimestampPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
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
                return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + Ieee80211BeaconFields.TimestampPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211BeaconFields.TimestampPosition);
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
                return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + Ieee80211BeaconFields.BeaconIntervalPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211BeaconFields.BeaconIntervalPosition);
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
                                                      header.Offset + Ieee80211BeaconFields.CapabilityInformationPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211BeaconFields.CapabilityInformationPosition);
            }
        }

        /// <summary>
        /// Defines the capabilities of the network.
        /// </summary>
        public Ieee80211CapabilityInformationField CapabilityInformation
        {
            get;
            set;
        }


        public override int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * 3) +
                    Ieee80211MacFields.SequenceControlLength +
                    Ieee80211BeaconFields.TimestampLength +
                    Ieee80211BeaconFields.BeaconIntervalLength +
                    Ieee80211BeaconFields.CapabilityInformationLength + 
                    InformationElements.Length);
            }
        }

        /// <summary>
        /// The information elements included in the frame
        /// 
        /// Most (but not all) beacons frames will contain an Information element that contains the SSID.
        /// </summary>
        public Ieee80211InformationElementSection InformationElements { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211BeaconFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);
            CapabilityInformation = new Ieee80211CapabilityInformationField(CapabilityInformationBytes);

            //create a segment that just refers to the info element section
            ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                (bas.Offset + Ieee80211BeaconFields.InformationElement1Position),
                (bas.Length - Ieee80211BeaconFields.InformationElement1Position - Ieee80211MacFields.FrameCheckSequenceLength));

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
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [BeaconFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }
    }
}
