using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class ContentionFreeEndFrame : MacFrame
        {
            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress
            {
                get
                {
                    return GetAddress(0);
                }

                set
                {
                    SetAddress(0, value);
                }
            }

            /// <summary>
            /// BSS ID
            /// </summary>
            public PhysicalAddress BssId
            {
                get
                {
                    return GetAddress(1);
                }

                set
                {
                    SetAddress(1, value);
                }
            }

            /// <summary>
            /// Length of the frame
            /// </summary>
            override public int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 2));
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public ContentionFreeEndFrame(ByteArraySegment bas)
            {
                header = new ByteArraySegment(bas);
                header.Length = FrameSize;

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [CF-End RA {2} BSSID {3}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     ReceiverAddress.ToString(),
                                     BssId.ToString());
            }
        } 
    }
}
