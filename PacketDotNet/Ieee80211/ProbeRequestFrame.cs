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
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Probe request frames are used by stations to scan the area for existing networks.
        /// </summary>
        public class ProbeRequestFrame : ManagementFrame
        {
            private class ProbeRequestFields
            {
                public readonly static int InformationElement1Position;

                static ProbeRequestFields()
                {
                    InformationElement1Position = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                }
            }
   
            /// <summary>
            /// Length of the frame header.
            /// 
            /// This does not include the FCS, it represents only the header bytes that would
            /// would preceed any payload.
            /// </summary>
            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        InformationElements.Length);
                }
            }
   
            /// <summary>
            /// Gets or sets the information elements included in the frame.
            /// </summary>
            /// <value>
            /// The information elements.
            /// </value>
            /// <remarks>Probe request frames normally contain information elements for <see cref="InformationElement.ElementId.ServiceSetIdentity"/>, 
            /// <see cref="InformationElement.ElementId.SupportedRates"/> and <see cref="InformationElement.ElementId.ExtendedSupportedRates"/> in that order.</remarks>
            public InformationElementList InformationElements { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public ProbeRequestFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
				
				if(bas.Length > ProbeRequestFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    	(bas.Offset + ProbeRequestFields.InformationElement1Position),
                    	(bas.Length - ProbeRequestFields.InformationElement1Position));

                	InformationElements = new InformationElementList (infoElementsSegment);
				}
				else
				{
					InformationElements = new InformationElementList ();
				}
                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }
   
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ProbeRequestFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (Mac Address of the Access Point).
            /// </param>
            /// <param name='InformationElements'>
            /// Information elements.
            /// </param>
            public ProbeRequestFrame (PhysicalAddress SourceAddress,
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
                this.InformationElements = new InformationElementList (InformationElements);
                
                this.FrameControl.SubType = PacketDotNet.Ieee80211.FrameControlField.FrameSubTypes.ManagementProbeRequest;
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
                
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (header, header.Offset + ProbeRequestFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }
        } 
    }
}
