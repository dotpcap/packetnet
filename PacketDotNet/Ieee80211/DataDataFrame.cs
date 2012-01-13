using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class DataDataFrame : DataFrame
        {
            public override int FrameSize
            {
                get
                {
                    //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                    int numOfAddressFields = (FrameControl.ToDS && FrameControl.FromDS) ? 4 : 3;

                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * numOfAddressFields) +
                        MacFields.SequenceControlLength);
                }
            }


            public DataDataFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
                ReadAddresses(); //must do this after reading FrameControl

                header.Length = FrameSize;
                int payloadLength = header.BytesLength - (header.Offset + header.Length) - MacFields.FrameCheckSequenceLength;
                payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes (payloadLength);
            }
            
            public DataDataFrame ()
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.SequenceControl = new SequenceControlField ();
            }
            
            public override void UpdateCalculatedValues ()
            {
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                this.SequenceControlBytes = this.SequenceControl.Field;
                WriteAddressBytes();
//                var updatedFrameLength = (FrameSize + MacFields.FrameCheckSequenceLength);
//                if (header.Length < updatedFrameLength)
//                {
//                    //the backing buffer isnt big enough to accommodate the info elements so we need to resize it
//                    ByteArraySegment newFrameArray = new ByteArraySegment (new Byte[updatedFrameLength]);
//                    Array.Copy (header.Bytes, header.Offset, newFrameArray.Bytes, 0, BeaconFields.InformationElement1Position);
//                    header = newFrameArray;
//                }
//                
//                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
//                this.InformationElements.CopyTo (header, header.Offset + BeaconFields.InformationElement1Position);
//                
//                header.Length = FrameSize;
//                
//                //TODO: We should recalculate the FCS here
//                this.FrameCheckSequence = 0xFFFFFFFF;
            }


            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [DataFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
