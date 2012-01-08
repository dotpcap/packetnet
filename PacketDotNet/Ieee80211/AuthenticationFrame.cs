using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Format of an 802.11 management authentication frame.
        /// </summary>
        public class AuthenticationFrame : ManagementFrame
        {
            private class AuthenticationFields
            {
                public readonly static int AuthAlgorithmNumLength = 2;
                public readonly static int AuthAlgorithmTransactionSequenceNumLength = 2;
                public readonly static int StatusCodeLength = 2;

                public readonly static int AuthAlgorithmNumPosition;
                public readonly static int AuthAlgorithmTransactionSequenceNumPosition;
                public readonly static int StatusCodePosition;
                public readonly static int InformationElement1Position;

                static AuthenticationFields()
                {
                    AuthAlgorithmNumPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
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
                        header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
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
                        header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                }
            }

            /// <summary>
            /// Indicates the success or failure of the authentication operation
            /// </summary>
            public AuthenticationStatusCode StatusCode
            {
                get
                {
                    return (AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16(header.Bytes,
                        header.Offset + AuthenticationFields.StatusCodePosition);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// </summary>
            public InformationElementList InformationElements { get; set; }


            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        AuthenticationFields.AuthAlgorithmNumLength +
                        AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength +
                        AuthenticationFields.StatusCodeLength +
                        InformationElements.Length);
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public AuthenticationFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                SequenceControl = new SequenceControlField(SequenceControlBytes);

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + AuthenticationFields.InformationElement1Position),
                    (bas.Length - AuthenticationFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

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
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [AuthenticationFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString(),
                                     SourceAddress.ToString(),
                                     BssId.ToString());
            }

        } 
    }
}
