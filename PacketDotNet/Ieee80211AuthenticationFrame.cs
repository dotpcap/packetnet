using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Format of an 802.11 management authentication frame.
    /// </summary>
    public class Ieee80211AuthenticationFrame : Ieee80211ManagementFrame
    {
        private class Ieee80211AuthenticationFields
        {
            public readonly static int AuthAlgorithmNumLength = 2;
            public readonly static int AuthAlgorithmTransactionSequenceNumLength = 2;
            public readonly static int StatusCodeLength = 2;

            public readonly static int AuthAlgorithmNumPosition;
            public readonly static int AuthAlgorithmTransactionSequenceNumPosition;
            public readonly static int StatusCodePosition;
            public readonly static int InformationElement1Position;

            static Ieee80211AuthenticationFields()
            {
                AuthAlgorithmNumPosition = Ieee80211MacFields.SequenceControlPosition + Ieee80211MacFields.SequenceControlLength;
                AuthAlgorithmTransactionSequenceNumPosition = AuthAlgorithmNumPosition + AuthAlgorithmNumLength;
                StatusCodePosition = AuthAlgorithmTransactionSequenceNumPosition + AuthAlgorithmTransactionSequenceNumLength;
                InformationElement1Position = StatusCodePosition + StatusCodeLength;
            }
        }

        /// <summary>
        /// Number used for selection of authentication algorithm
        /// </summary>
        public UInt16 AuthenticationAlgorithmNumber
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                    header.Offset + Ieee80211AuthenticationFields.AuthAlgorithmNumPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211AuthenticationFields.AuthAlgorithmNumPosition);
            }
        }

        /// <summary>
        /// Sequence number to define the step of the authentication algorithm
        /// </summary>
        public UInt16 AuthenticationAlgorithmTransactionSequenceNumber
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                    header.Offset + Ieee80211AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
            }
        }

        /// <summary>
        /// Indicates the success or failure of the authentication operation
        /// </summary>
        public Ieee80211AuthenticationStatusCode StatusCode
        {
            get
            {
                return (Ieee80211AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16(header.Bytes,
                    header.Offset + Ieee80211AuthenticationFields.StatusCodePosition);
            }
        }

        /// <summary>
        /// The information elements included in the frame
        /// </summary>
        public Ieee80211InformationElementSection InformationElements { get; set; }


        public override int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * 3) +
                    Ieee80211MacFields.SequenceControlLength +
                    Ieee80211AuthenticationFields.AuthAlgorithmNumLength + 
                    Ieee80211AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength + 
                    Ieee80211AuthenticationFields.StatusCodeLength + 
                    InformationElements.Length);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211AuthenticationFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);

            //create a segment that just refers to the info element section
            ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                (bas.Offset + Ieee80211AuthenticationFields.InformationElement1Position),
                (bas.Length - Ieee80211AuthenticationFields.InformationElement1Position - Ieee80211MacFields.FrameCheckSequenceLength));

            InformationElements = new Ieee80211InformationElementSection(infoElementsSegment);

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
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [AuthenticationFrame RA {2} TA {3} BSSID {4}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 DestinationAddress.ToString(),
                                 SourceAddress.ToString(),
                                 BssId.ToString());
        }

    }
}
