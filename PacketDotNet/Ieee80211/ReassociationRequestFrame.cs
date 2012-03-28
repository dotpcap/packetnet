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
        /// Reassociation request frame.
        ///
        /// Sent when a wireless client is going from one access point to another
        /// http://en.wikipedia.org/wiki/IEEE_802.11#Frames
        /// </summary>
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
            private UInt16 CapabilityInformationBytes
            {
                get
                {
					if(header.Length >= 
					   (ReassociationRequestFields.CapabilityInformationPosition + ReassociationRequestFields.CapabilityInformationLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
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
                                                     header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
                }
            }

            /// <summary>
            /// Gets or sets the capability information, the type of network the mobile station wants to join
            /// </summary>
            /// <value>
            /// The capability information.
            /// </value>
            public CapabilityInformationField CapabilityInformation {get; set;}
   
            /// <summary>
            /// Gets or sets the listen interval. This is the number of beacon interval time periods that the access
            /// point must retain buffered packets for.
            /// </summary>
            /// <value>
            /// The listen interval.
            /// </value>
            public UInt16 ListenInterval {get; set;}

            /// <summary>
            /// Gets or sets the listen interval, the length of buffered frame retention
            /// </summary>
            /// <value>
            /// The listen interval.
            /// </value>
            private UInt16 ListenIntervalBytes
            {
                get
                {
					if(header.Length >= 
					   (ReassociationRequestFields.ListenIntervalPosition + ReassociationRequestFields.ListenIntervalLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          header.Offset + ReassociationRequestFields.ListenIntervalPosition);
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
                                                     header.Offset + ReassociationRequestFields.ListenIntervalPosition);
                }
            }

            /// <summary>
            /// DestinationAddress
            /// </summary>
            public PhysicalAddress CurrentAccessPointAddress { get; set; }
            
            private PhysicalAddress CurrentAccessPointAddressBytes
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
            public ReassociationRequestFrame (ByteArraySegment bas)
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
                CurrentAccessPointAddress = CurrentAccessPointAddressBytes;
                
				if(bas.Length > ReassociationRequestFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    	(bas.Offset + ReassociationRequestFields.InformationElement1Position),
                    	(bas.Length - ReassociationRequestFields.InformationElement1Position));

                	InformationElements = new InformationElementList(infoElementsSegment);
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
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ReassociationRequestFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// BssId.
            /// </param>
            /// <param name='InformationElements'>
            /// Information elements.
            /// </param>
            public ReassociationRequestFrame (PhysicalAddress SourceAddress,
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
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementReassociationRequest;
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
                this.InformationElements.CopyTo (header, header.Offset + ReassociationRequestFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }
            
        } 
    }
}
