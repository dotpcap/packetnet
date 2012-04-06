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
        /// Probe response frames are sent by Access Points in response to probe requests by stations.
        /// An access point may respond to a probe request if it hosts a network with parameters compatible with those
        /// requested by the station.
        /// </summary>
        public class ProbeResponseFrame : ManagementFrame
        {
            private class ProbeResponseFields
            {
                public readonly static int TimestampLength = 8;
                public readonly static int BeaconIntervalLength = 2;
                public readonly static int CapabilityInformationLength = 2;

                public readonly static int TimestampPosition;
                public readonly static int BeaconIntervalPosition;
                public readonly static int CapabilityInformationPosition;
                public readonly static int InformationElement1Position;

                static ProbeResponseFields()
                {
                    TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    BeaconIntervalPosition = TimestampPosition + TimestampLength;
                    CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                    InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
                }
            }
   
            /// <summary>
            /// Gets or sets the timestamp. The timestamp is used by a station to ensure that it
            /// is using the most up to date parameters for the network.
            /// </summary>
            /// <value>
            /// The timestamp.
            /// </value>
            public UInt64 Timestamp {get;set;}
                
            private UInt64 TimestampBytes
            {
                get
                {
					if(header.Length >= (ProbeResponseFields.TimestampPosition + ProbeResponseFields.TimestampLength))
					{
						return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + ProbeResponseFields.TimestampPosition);
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
                                                     header.Offset + ProbeResponseFields.TimestampPosition);
                }
            }

            /// <summary>
            /// Gets or sets the beacon interval. This is the minimum time between beacon frames from the access point.
            /// </summary>
            /// <value>
            /// The beacon interval.
            /// </value>
            public UInt16 BeaconInterval { get; set; }
            
            private UInt16 BeaconIntervalBytes
            {
                get
                {
					if(header.Length >= (ProbeResponseFields.BeaconIntervalPosition + ProbeResponseFields.BeaconIntervalLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + ProbeResponseFields.BeaconIntervalPosition);
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
                                                     header.Offset + ProbeResponseFields.BeaconIntervalPosition);
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
					   (ProbeResponseFields.CapabilityInformationPosition + ProbeResponseFields.CapabilityInformationLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          header.Offset + ProbeResponseFields.CapabilityInformationPosition);
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
                                                     header.Offset + ProbeResponseFields.CapabilityInformationPosition);
                }
            }
   
            /// <summary>
            /// Get or set the capability information field that defines the capabilities of the network.
            /// </summary>
            public CapabilityInformationField CapabilityInformation {get; set;}

            /// <summary>
            /// Gets or sets the information elements included in the frame.
            /// </summary>
            /// <value>
            /// The information elements.
            /// </value>
            public InformationElementList InformationElements { get; set; }

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
                        ProbeResponseFields.TimestampLength +
                        ProbeResponseFields.BeaconIntervalLength +
                        ProbeResponseFields.CapabilityInformationLength +
                        InformationElements.Length);
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public ProbeResponseFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
                Timestamp = TimestampBytes;
                BeaconInterval = BeaconIntervalBytes;
                CapabilityInformation = new CapabilityInformationField (CapabilityInformationBytes);
				
				if(bas.Length > ProbeResponseFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    	(bas.Offset + ProbeResponseFields.InformationElement1Position),
                    	(bas.Length - ProbeResponseFields.InformationElement1Position));

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
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ProbeResponseFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (Mac address of the access point).
            /// </param>
            /// <param name='InformationElements'>
            /// Information elements.
            /// </param>
            public ProbeResponseFrame (PhysicalAddress SourceAddress,
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
                
                this.FrameControl.SubType = PacketDotNet.Ieee80211.FrameControlField.FrameSubTypes.ManagementProbeResponse;
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
                this.InformationElements.CopyTo (header, header.Offset + ProbeResponseFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }

        } 
    }
}
