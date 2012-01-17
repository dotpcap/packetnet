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
                ReadAddresses (); //must do this after reading FrameControl

                header.Length = FrameSize;
                int payloadLength = header.BytesLength - (header.Offset + header.Length) - MacFields.FrameCheckSequenceLength;
                payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes (payloadLength);
                
                //Must do this after setting header.Length and handling payload as they are used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }
            
            public DataDataFrame ()
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.SequenceControl = new SequenceControlField ();
                AssignDefaultAddresses ();
                
                FrameControl.Type = FrameControlField.FrameTypes.Data;
            }
            
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length < FrameSize))
                {
                    header = new ByteArraySegment (new Byte[FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                this.SequenceControlBytes = this.SequenceControl.Field;
                WriteAddressBytes ();
            }


            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString ()
            {
                return string.Format ("FrameControl {0}, FrameCheckSequence {1}, [DataFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString (),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString (),
                                     SourceAddress.ToString (),
                                     BssId.ToString ());
            }
        } 
    }
}
