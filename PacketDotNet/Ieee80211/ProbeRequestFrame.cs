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
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    namespace Ieee80211
    {
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

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    (bas.Offset + ProbeRequestFields.InformationElement1Position),
                    (bas.Length - ProbeRequestFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementList (infoElementsSegment);
                
                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
                
                //Must do this after setting header.Length as that is used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }

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
                
                this.FrameControl.Type = PacketDotNet.Ieee80211.FrameControlField.FrameTypes.ManagementProbeRequest;
            }
            
            
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length < FrameSize))
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
   
            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [ProbeRequestFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
