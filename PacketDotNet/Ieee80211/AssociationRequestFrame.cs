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
            private UInt16 CapabilityInformationBytes
            {
                get
                {
                    if(header.Length >= 
                        (AssociationRequestFields.CapabilityInformationPosition + AssociationRequestFields.CapabilityInformationLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                                  header.Offset + AssociationRequestFields.CapabilityInformationPosition);
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
                        header.Offset + AssociationRequestFields.CapabilityInformationPosition);
                }
            }

            /// <summary>
            /// Gets or sets the capability information.
            /// </summary>
            /// <value>
            /// The capability information.
            /// </value>
            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the listen interval.
            /// </summary>
            /// <value>
            /// The listen interval.
            /// </value>
            public UInt16 ListenInterval {get; set;}
            
            private UInt16 ListenIntervalBytes
            {
                get
                {
                    if(header.Length >= (AssociationRequestFields.ListenIntervalPosition + AssociationRequestFields.ListenIntervalLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                                  header.Offset + AssociationRequestFields.ListenIntervalPosition);
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
                        header.Offset + AssociationRequestFields.ListenIntervalPosition);
                }
            }
   
            /// <summary>
            /// Gets or sets the information elements.
            /// </summary>
            /// <value>
            /// The information elements.
            /// </value>
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
				
				if(bas.Length > AssociationRequestFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    	(bas.Offset + AssociationRequestFields.InformationElement1Position),
                    	(bas.Length - AssociationRequestFields.InformationElement1Position));
				
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
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AssociationRequestFrame"/> class.
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
            public AssociationRequestFrame (PhysicalAddress SourceAddress,
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
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAssociationRequest;
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
                
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (header, header.Offset + AssociationRequestFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }
            
        } 
    }
}
