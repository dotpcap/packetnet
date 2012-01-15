/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/

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
        /// <summary>
        /// Format of an 802.11 management association frame.
        /// </summary>
        public class AssociationRequestFrame : ManagementFrame
        {
            private class AssociationRequestFields
            {
                public readonly static int CapabilityInformationLength = 2;
                public readonly static int ListenIntervalLength = 2;

                public readonly static int CapabilityInformationPosition;
                public readonly static int ListenIntervalPosition;
                public readonly static int InformationElement1Position;

                static AssociationRequestFields()
                {
                    CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    ListenIntervalPosition = CapabilityInformationPosition + CapabilityInformationLength;
                    InformationElement1Position = ListenIntervalPosition + ListenIntervalLength;
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
                        header.Offset + AssociationRequestFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                        header.Bytes,
                        header.Offset + AssociationRequestFields.CapabilityInformationPosition);
                }
            }

            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }

            public UInt16 ListenInterval {get; set;}
            
            public UInt16 ListenIntervalBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                        header.Offset + AssociationRequestFields.ListenIntervalPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                        header.Bytes,
                        header.Offset + AssociationRequestFields.ListenIntervalPosition);
                }
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
                        AssociationRequestFields.CapabilityInformationLength +
                        AssociationRequestFields.ListenIntervalLength +
                        InformationElements.Length);
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public AssociationRequestFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);

                CapabilityInformation = new CapabilityInformationField (CapabilityInformationBytes);
                ListenInterval = ListenIntervalBytes;
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    (bas.Offset + AssociationRequestFields.InformationElement1Position),
                    (bas.Length - AssociationRequestFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementList (infoElementsSegment);

                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
                
                //Must do this after setting header.Length as that is used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [AssociationRequestFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
