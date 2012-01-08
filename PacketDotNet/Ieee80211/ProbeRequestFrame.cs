using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

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
            public ProbeRequestFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                SequenceControl = new SequenceControlField(SequenceControlBytes);

                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + ProbeRequestFields.InformationElement1Position),
                    (bas.Length - ProbeRequestFields.InformationElement1Position - MacFields.FrameCheckSequenceLength));

                InformationElements = new InformationElementList(infoElementsSegment);

                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }

            public ProbeRequestFrame()
            {
                int length = TcpFields.HeaderLength;
                var headerBytes = new byte[length];
                header = new ByteArraySegment(headerBytes, 0, length);
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
