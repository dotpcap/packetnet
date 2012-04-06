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
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

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
            private UInt16 CapabilityInformationBytes
            {
                get
                {
                    if(header.Length >= 
                       (AssociationResponseFields.CapabilityInformationPosition + AssociationResponseFields.CapabilityInformationLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                                  header.Offset + AssociationResponseFields.CapabilityInformationPosition);
                    }
                    else
                    {
                        return 0;
                    }
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
            
            private AuthenticationStatusCode StatusCodeBytes
            {
                get
                {
                    if(header.Length >= (AssociationResponseFields.StatusCodePosition + AssociationResponseFields.StatusCodeLength))
                    {
                        return (AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                                                             header.Offset + AssociationResponseFields.StatusCodePosition);
                    }
                    else
                    {
                        //This seems the most sensible value to return when it is not possible
                        //to extract a meaningful value
                        return AuthenticationStatusCode.UnspecifiedFailure;
                    }
                }
                
                set
                {
                    EndianBitConverter.Little.CopyBytes ((UInt16)value,
                        header.Bytes,
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
            
            private UInt16 AssociationIdBytes
            {
                get
                {
                    if(header.Length >= AssociationResponseFields.AssociationIdPosition + AssociationResponseFields.AssociationIdLength)
                    {
                        UInt16 associationID = EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + AssociationResponseFields.AssociationIdPosition);
                        return (UInt16)(associationID & 0xCF);
                    }
                    else
                    {
                        return 0;
                    }
                }

                set
                {
                    UInt16 associationID = (UInt16)(value & 0xCF);
                    EndianBitConverter.Little.CopyBytes(associationID,
                                                     header.Bytes,
                                                     header.Offset + AssociationResponseFields.AssociationIdPosition);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// </summary>
            public InformationElementList InformationElements { get; set; }

            /// <summary>
            /// Gets the size of the frame.
            /// </summary>
            /// <value>
            /// The size of the frame.
            /// </value>
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
                
                if(bas.Length > AssociationResponseFields.InformationElement1Position)
                {
                    //create a segment that just refers to the info element section
                    ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                        (bas.Offset + AssociationResponseFields.InformationElement1Position),
                        (bas.Length - AssociationResponseFields.InformationElement1Position));

                    InformationElements = new InformationElementList (infoElementsSegment);
                }
                else
                {
                    InformationElements = new InformationElementList();
                }
                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AssociationResponseFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (MAC Address of Access Point).
            /// </param>
            /// <param name='InformationElements'>
            /// Information elements.
            /// </param>
            public AssociationResponseFrame (PhysicalAddress SourceAddress,
                                            PhysicalAddress DestinationAddress,
                                            PhysicalAddress BssId,
                                            InformationElementList InformationElements)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.DestinationAddress = DestinationAddress;
                this.SourceAddress = SourceAddress;
                this.BssId = BssId;
                this.SequenceControl = new SequenceControlField ();
                this.CapabilityInformation = new CapabilityInformationField ();

                this.InformationElements = new InformationElementList (InformationElements);
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAssociationResponse;
            }
            
            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length > (header.BytesLength - header.Offset)) || (header.Length < FrameSize))
                {
                    header = new ByteArraySegment (new Byte[FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                SetAddress (0, DestinationAddress);
                SetAddress (1, SourceAddress);
                SetAddress (2, BssId);
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.CapabilityInformationBytes = this.CapabilityInformation.Field;
                this.StatusCodeBytes = this.StatusCode;
                this.AssociationIdBytes = this.AssociationId;
                
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (header, header.Offset + AssociationResponseFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }
                
        } 
    }
}
