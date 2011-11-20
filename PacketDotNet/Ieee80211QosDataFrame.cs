using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    public class Ieee80211QosDataFrame : Ieee80211DataFrame
    {
        private class Ieee80211QosDataField
        {
            public readonly static int QosControlLength = 2;

            public readonly static int QosControlPosition;

            static Ieee80211QosDataField()
            {
                QosControlPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
            }
        }

        public UInt16 QosControl
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211QosDataField.QosControlPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211QosDataField.QosControlPosition);
            }
        }

        public override int FrameSize
        {
            get
            {
                //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                int numOfAddressFields = (FrameControl.ToDS && FrameControl.FromDS) ? 4 : 3;

                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * numOfAddressFields) +
                    Ieee80211MacFields.SequenceControlLength +
                    Ieee80211QosDataField.QosControlLength);
            }
        }


        public Ieee80211QosDataFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);

            header.Length = FrameSize;
            int payloadLength = header.BytesLength - (header.Offset + header.Length) - Ieee80211MacFields.FrameCheckSequenceLength;
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes(payloadLength);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [QosDataFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }
    }
}
