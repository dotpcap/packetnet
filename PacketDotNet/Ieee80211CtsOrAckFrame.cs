using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    /// <summary>
    /// Format of a CTS or an ACK frame
    /// </summary>
    public class Ieee80211CtsOrAckFrame : Ieee80211MacFrame
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
        /// Length of the frame
        /// </summary>
        override public int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    Ieee80211MacFields.AddressLength);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211CtsOrAckFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);
            header.Length = FrameSize;

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [CTSOrACKFrame RA {2}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 ReceiverAddress.ToString());
        }
    }

}
