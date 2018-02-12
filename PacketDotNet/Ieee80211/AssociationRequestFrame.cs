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
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211 {
        /// <summary>
        /// Format of an 802.11 management association frame.
        /// </summary>
        public class AssociationRequestFrame : ManagementFrame
        {
            private class AssociationRequestFields
            {
                public static readonly Int32 CapabilityInformationLength = 2;
                public static readonly Int32 ListenIntervalLength = 2;

                public static readonly Int32 CapabilityInformationPosition;
                public static readonly Int32 ListenIntervalPosition;
                public static readonly Int32 InformationElement1Position;

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
                    if(this.HeaderByteArraySegment.Length >= 
                        (AssociationRequestFields.CapabilityInformationPosition + AssociationRequestFields.CapabilityInformationLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + AssociationRequestFields.CapabilityInformationPosition);
                    }
                    else
                    {
                        return 0;
                    }
                }

                set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + AssociationRequestFields.CapabilityInformationPosition);
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
                    if(this.HeaderByteArraySegment.Length >= (AssociationRequestFields.ListenIntervalPosition + AssociationRequestFields.ListenIntervalLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + AssociationRequestFields.ListenIntervalPosition);
                    }
                    else
                    {
                        return 0;
                    }
                }

                set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + AssociationRequestFields.ListenIntervalPosition);
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
            public override Int32 FrameSize => (MacFields.FrameControlLength +
                                              MacFields.DurationIDLength +
                                              (MacFields.AddressLength * 3) +
                                              MacFields.SequenceControlLength +
                                              AssociationRequestFields.CapabilityInformationLength +
                                              AssociationRequestFields.ListenIntervalLength + this.InformationElements.Length);


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public AssociationRequestFrame (ByteArraySegment bas)
            {
                this.HeaderByteArraySegment = new ByteArraySegment (bas);

                this.FrameControl = new FrameControlField (this.FrameControlBytes);
                this.Duration = new DurationField (this.DurationBytes);
                this.DestinationAddress = this.GetAddress (0);
                this.SourceAddress = this.GetAddress (1);
                this.BssId = this.GetAddress (2);
                this.SequenceControl = new SequenceControlField (this.SequenceControlBytes);

                this.CapabilityInformation = new CapabilityInformationField (this.CapabilityInformationBytes);
                this.ListenInterval = this.ListenIntervalBytes;
				
				if(bas.Length > AssociationRequestFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    	(bas.Offset + AssociationRequestFields.InformationElement1Position),
                    	(bas.Length - AssociationRequestFields.InformationElement1Position));

				    this.InformationElements = new InformationElementList (infoElementsSegment);
				}
				else
				{
				    this.InformationElements = new InformationElementList();
				}

                //cant set length until after we have handled the information elements
                //as they vary in length
                this.HeaderByteArraySegment.Length = this.FrameSize;
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
                if ((this.HeaderByteArraySegment == null) || (this.HeaderByteArraySegment.Length > (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) || (this.HeaderByteArraySegment.Length < this.FrameSize))
                {
                    this.HeaderByteArraySegment = new ByteArraySegment (new Byte[this.FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                this.SetAddress (0, this.DestinationAddress);
                this.SetAddress (1, this.SourceAddress);
                this.SetAddress (2, this.BssId);
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.CapabilityInformationBytes = this.CapabilityInformation.Field;
                
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (this.HeaderByteArraySegment, this.HeaderByteArraySegment.Offset + AssociationRequestFields.InformationElement1Position);

                this.HeaderByteArraySegment.Length = this.FrameSize;
            }
            
        } 
    }
