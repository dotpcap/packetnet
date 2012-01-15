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
        /// Format of an 802.11 management association response frame.
        /// </summary>
        public class AssociationResponseFrame : ManagementFrame
        {
            private class AssociationResponseFields
            {
                public readonly static int CapabilityInformationLength = 2;
                public readonly static int StatusCodeLength = 2;
                public readonly static int AssociationIdLength = 2;

                public readonly static int CapabilityInformationPosition;
                public readonly static int StatusCodePosition;
                public readonly static int AssociationIdPosition;
                public readonly static int InformationElement1Position;

                static AssociationResponseFields()
                {
                    CapabilityInformationPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
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
                                                          header.Offset + AssociationResponseFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + AssociationResponseFields.CapabilityInformationPosition);
                }
            }

            /// <summary>
            /// The capability information field that describes the networks capabilities.
            /// </summary>
            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }

            /// <summary>
            /// Value indicating the success or failure of the association.
            /// </summary>
            public AuthenticationStatusCode StatusCode {get; set;}
            
            public AuthenticationStatusCode StatusCodeBytes
            {
                get
                {
                    return (AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16(header.Bytes,
                        header.Offset + AssociationResponseFields.StatusCodePosition);
                }
            }

            /// <summary>
            /// The id assigned to the station by the access point to assist in management and control functions.
            /// 
            /// Although this is a 16bit field only 14 of the bits are used to represent the id. Therefore the available values
            /// for this field are inthe range 1-2,007.
            /// </summary>
            public UInt16 AssociationId {get; set;}
            
            public UInt16 AssociationIdBytes
            {
                get
                {
                    UInt16 associationID = EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + AssociationResponseFields.AssociationIdPosition);
                    return (UInt16)(associationID & 0xCF);
                }

                set
                {
                    UInt16 associationID = (UInt16)(value | 0xCF);
                    EndianBitConverter.Little.CopyBytes(associationID,
                                                     header.Bytes,
                                                     header.Offset + AssociationResponseFields.AssociationIdPosition);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// </summary>
            public InformationElementList InformationElements { get; set; }

            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        AssociationResponseFields.CapabilityInformationLength +
                        AssociationResponseFields.StatusCodeLength +
                        AssociationResponseFields.AssociationIdLength +
                        InformationElements.Length);
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public AssociationResponseFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);

                CapabilityInformation = new CapabilityInformationField (CapabilityInformationBytes);
                StatusCode = StatusCodeBytes;
                AssociationId = AssociationIdBytes;
                
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    (bas.Offset + AssociationResponseFields.InformationElement1Position),
                    (bas.Length - AssociationResponseFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

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
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [AssociationResponseFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
