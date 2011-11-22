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
        public class ReassociationRequestFrame : ManagementFrame
        {
            private class ReassociationRequestFields
            {
                public readonly static int CapabilityInformationLength = 2;
                public readonly static int ListenIntervalLength = 2;

                public readonly static int CapabilityInformationPosition;
                public readonly static int ListenIntervalPosition;
                public readonly static int CurrentAccessPointPosition;
                public readonly static int InformationElement1Position;

                static ReassociationRequestFields()
                {
                    CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
                    CurrentAccessPointPosition = ListenIntervalPosition + ListenIntervalLength;
                    InformationElement1Position = CurrentAccessPointPosition + MacFields.AddressLength;
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
                                                          header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
                }
            }

            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }


            public UInt16 ListenInterval
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                          header.Offset + ReassociationRequestFields.ListenIntervalPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + ReassociationRequestFields.ListenIntervalPosition);
                }
            }

            /// <summary>
            /// DestinationAddress
            /// </summary>
            public PhysicalAddress CurrentAccessPointAddress
            {
                get
                {
                    return GetAddressByOffset(header.Offset + ReassociationRequestFields.CurrentAccessPointPosition);
                }

                set
                {
                    SetAddressByOffset(header.Offset + ReassociationRequestFields.CurrentAccessPointPosition, value);
                }
            }


            public InformationElementSection InformationElements { get; set; }

            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        ReassociationRequestFields.CapabilityInformationLength +
                        ReassociationRequestFields.ListenIntervalLength +
                        MacFields.AddressLength +
                        InformationElements.Length);
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public ReassociationRequestFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                SequenceControl = new SequenceControlField(SequenceControlBytes);

                CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + ReassociationRequestFields.InformationElement1Position),
                    (bas.Length - ReassociationRequestFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementSection(infoElementsSegment);

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
}
