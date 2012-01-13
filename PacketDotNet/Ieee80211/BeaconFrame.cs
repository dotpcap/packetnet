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
        /// Format of an 802.11 management beacon frame.
        /// 
        /// Beacon frames are used to annouce the existance of a wireless network. If an
        /// access point has been configured to not broadcast its SSID then it may not transmit
        /// beacon frames.
        /// </summary>
        public class BeaconFrame : ManagementFrame
        {

            private class BeaconFields
            {
                public readonly static int TimestampLength = 8;
                public readonly static int BeaconIntervalLength = 2;
                public readonly static int CapabilityInformationLength = 2;

                public readonly static int TimestampPosition;
                public readonly static int BeaconIntervalPosition;
                public readonly static int CapabilityInformationPosition;
                public readonly static int InformationElement1Position;
                
                

                static BeaconFields ()
                {
                    TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    BeaconIntervalPosition = TimestampPosition + TimestampLength;
                    CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                    InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
                }
            }
            

            /// <summary>
            /// The number of microseconds the networks master timekeeper has been active.
            /// 
            /// Used for synchronisation between stations in an IBSS. When it reaches the maximum value the timestamp will wrap (not very likely).
            /// </summary>
            public UInt64 Timestamp {get; set;}
            
            public UInt64 TimestampBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt64(header.Bytes, header.Offset + BeaconFields.TimestampPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.TimestampPosition);
                }
            }

            /// <summary>
            /// The number of "time units" between beacon frames.
            /// 
            /// A time unit is 1,024 microseconds. This interval is usually set to 100 which equates to approximately 100 milliseconds or 0.1 seconds.
            /// </summary>
            public UInt16 BeaconInterval {get; set;}
            
            public UInt16 BeaconIntervalBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes, header.Offset + BeaconFields.BeaconIntervalPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.BeaconIntervalPosition);
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
                                                          header.Offset + BeaconFields.CapabilityInformationPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BeaconFields.CapabilityInformationPosition);
                }
            }

            /// <summary>
            /// Defines the capabilities of the network.
            /// </summary>
            public CapabilityInformationField CapabilityInformation
            {
                get;
                set;
            }


            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        BeaconFields.TimestampLength +
                        BeaconFields.BeaconIntervalLength +
                        BeaconFields.CapabilityInformationLength +
                        InformationElements.Length);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// 
            /// Most (but not all) beacons frames will contain an Information element that contains the SSID.
            /// </summary>
            public InformationElementList InformationElements { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public BeaconFrame (ByteArraySegment bas)
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

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    (bas.Offset + BeaconFields.InformationElement1Position),
                    (bas.Length - BeaconFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementList (infoElementsSegment);
                
                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
                
            }
            
            public BeaconFrame (PhysicalAddress SourceAddress,
                PhysicalAddress BssId, 
                InformationElementList InformationElements)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.SequenceControl = new SequenceControlField ();
                this.CapabilityInformation = new CapabilityInformationField ();
                this.InformationElements = new InformationElementList (InformationElements);
                
                //we need to create a ByteArraySegment to big enough to back the beacon frame
                var frameHeaderLength = FrameSize;
                header = new ByteArraySegment (new Byte[frameHeaderLength + MacFields.FrameCheckSequenceLength]);
                header.Length = frameHeaderLength;
                
                //now that we have created the field and the backing array we can safely set the values
                this.FrameControl.Type = FrameControlField.FrameTypes.ManagementBeacon;
                this.SourceAddress = SourceAddress;
                this.DestinationAddress = PhysicalAddress.Parse ("FF-FF-FF-FF-FF-FF");
                this.BssId = BssId;
                this.BeaconInterval = 100;
            }
            
            public override void UpdateCalculatedValues ()
            {
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                SetAddress (0, DestinationAddress);
                SetAddress (1, SourceAddress);
                SetAddress (2, BssId);
                this.SequenceControlBytes = this.SequenceControl.Field;
                TimestampBytes = Timestamp;
                BeaconIntervalBytes = BeaconInterval;
                this.CapabilityInformationBytes = this.CapabilityInformation.Field;
                
                var updatedFrameLength = (FrameSize + MacFields.FrameCheckSequenceLength);
                if (header.Length < updatedFrameLength)
                {
                    //the backing buffer isnt big enough to accommodate the info elements so we need to resize it
                    ByteArraySegment newFrameArray = new ByteArraySegment (new Byte[updatedFrameLength]);
                    Array.Copy (header.Bytes, header.Offset, newFrameArray.Bytes, 0, BeaconFields.InformationElement1Position);
                    header = newFrameArray;
                }
                
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (header, header.Offset + BeaconFields.InformationElement1Position);
                
                header.Length = FrameSize;
                
                //TODO: We should recalculate the FCS here
                this.FrameCheckSequence = 0xFFFFFFFF;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [BeaconFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
