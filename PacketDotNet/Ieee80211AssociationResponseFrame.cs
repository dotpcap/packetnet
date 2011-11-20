using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Format of an 802.11 management association response frame.
    /// </summary>
    public class Ieee80211AssociationResponseFrame : Ieee80211ManagementFrame
    {
        private class Ieee80211AssociationResponseFields
        {
            public readonly static int CapabilityInformationLength = 2;
            public readonly static int StatusCodeLength = 2;
            public readonly static int AssociationIdLength = 2;

            public readonly static int CapabilityInformationPosition;
            public readonly static int StatusCodePosition;
            public readonly static int AssociationIdPosition;
            public readonly static int InformationElement1Position;

            static Ieee80211AssociationResponseFields()
            {
                CapabilityInformationPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
                StatusCodePosition = CapabilityInformationPosition + CapabilityInformationLength;
                AssociationIdPosition = StatusCodePosition + StatusCodeLength;
                InformationElement1Position = AssociationIdPosition + AssociationIdLength;
            }
        }

        /// <summary>
        /// The raw capability information bytes
        /// </summary>
        public UInt16 CapabilityInformationBytes
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211AssociationResponseFields.CapabilityInformationPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211AssociationResponseFields.CapabilityInformationPosition);
            }
        }

        /// <summary>
        /// The capability information field that describes the networks capabilities.
        /// </summary>
        public Ieee80211CapabilityInformationField CapabilityInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Value indicating the success or failure of the association.
        /// </summary>
        public Ieee80211AuthenticationStatusCode StatusCode
        {
            get
            {
                return (Ieee80211AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16(header.Bytes,
                    header.Offset + Ieee80211AssociationResponseFields.StatusCodePosition);
            }
        }

        /// <summary>
        /// The id assigned to the station by the access point to assist in management and control functions.
        /// 
        /// Although this is a 16bit field only 14 of the bits are used to represent the id. Therefore the available values
        /// for this field are inthe range 1-2,007.
        /// </summary>
        public UInt16 AssociationId
        {
            get
            {
                UInt16 associationID = EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + Ieee80211AssociationResponseFields.AssociationIdPosition);
                return (UInt16)(associationID & 0xCF);
            }

            set
            {
                UInt16 associationID = (UInt16)(value | 0xCF);
                EndianBitConverter.Little.CopyBytes(associationID,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211AssociationResponseFields.AssociationIdPosition);
            }
        }

        /// <summary>
        /// The information elements included in the frame
        /// </summary>
        public Ieee80211InformationElementSection InformationElements { get; set; }

        public override int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * 3) +
                    Ieee80211MacFields.SequenceControlLength +
                    Ieee80211AssociationResponseFields.CapabilityInformationLength +
                    Ieee80211AssociationResponseFields.StatusCodeLength +
                    Ieee80211AssociationResponseFields.AssociationIdLength +
                    InformationElements.Length);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211AssociationResponseFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);

            CapabilityInformation = new Ieee80211CapabilityInformationField(CapabilityInformationBytes);

            //create a segment that just refers to the info element section
            ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                (bas.Offset + Ieee80211AssociationResponseFields.InformationElement1Position),
                (bas.Length - Ieee80211AssociationResponseFields.InformationElement1Position - Ieee80211MacFields.FrameCheckSequenceLength));

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
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [AssociationResponseFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }
    }
}
