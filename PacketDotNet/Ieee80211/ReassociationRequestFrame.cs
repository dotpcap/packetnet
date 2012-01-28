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

            /// <summary>
            /// Gets or sets the capability information, the type of network the mobile station wants to join
            /// </summary>
            /// <value>
            /// The capability information.
            /// </value>
            public CapabilityInformationField CapabilityInformation {get; set;}
   
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
                SequenceControl = new SequenceControlField (SequenceControlBytes);

                CapabilityInformation = new CapabilityInformationField (CapabilityInformationBytes);
                ListenInterval = ListenIntervalBytes;
                CurrentAccessPointAddress = CurrentAccessPointAddressBytes;
                
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + ReassociationRequestFields.InformationElement1Position),
                    (bas.Length - ReassociationRequestFields.InformationElement1Position));

                InformationElements = new InformationElementList(infoElementsSegment);

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
