using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    public class Ieee80211ReassociationRequestFrame : Ieee80211ManagementFrame
    {
        private class Ieee80211ReassociationRequestFields
        {
            public readonly static int CapabilityInformationLength = 2;
            public readonly static int ListenIntervalLength = 2;

            public readonly static int CapabilityInformationPosition;
            public readonly static int ListenIntervalPosition;
            public readonly static int CurrentAccessPointPosition;
            public readonly static int InformationElement1Position;

            static Ieee80211ReassociationRequestFields()
            {
                CapabilityInformationPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
                ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
                CurrentAccessPointPosition = ListenIntervalPosition + ListenIntervalLength;
                InformationElement1Position = CurrentAccessPointPosition + Ieee80211MacFields.AddressLength;
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
                                                      header.Offset + Ieee80211ReassociationRequestFields.CapabilityInformationPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211ReassociationRequestFields.CapabilityInformationPosition);
            }
        }

        public Ieee80211CapabilityInformationField CapabilityInformation
        {
            get;
            set;
        }


        public UInt16 ListenInterval
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211ReassociationRequestFields.ListenIntervalPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211ReassociationRequestFields.ListenIntervalPosition);
            }
        }

        /// <summary>
        /// DestinationAddress
        /// </summary>
        public PhysicalAddress CurrentAccessPointAddress
        {
            get
            {
                return GetAddressByOffset(header.Offset + Ieee80211ReassociationRequestFields.CurrentAccessPointPosition);
            }

            set
            {
                SetAddressByOffset(header.Offset + Ieee80211ReassociationRequestFields.CurrentAccessPointPosition, value);
            }
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
                    Ieee80211ReassociationRequestFields.CapabilityInformationLength + 
                    Ieee80211ReassociationRequestFields.ListenIntervalLength +
                    Ieee80211MacFields.AddressLength +
                    InformationElements.Length);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211ReassociationRequestFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);

            CapabilityInformation = new Ieee80211CapabilityInformationField(CapabilityInformationBytes);

            //create a segment that just refers to the info element section
            ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                (bas.Offset + Ieee80211ReassociationRequestFields.InformationElement1Position),
                (bas.Length - Ieee80211ReassociationRequestFields.InformationElement1Position - Ieee80211MacFields.FrameCheckSequenceLength));

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
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [ReassociationRequestFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }
    }
}
