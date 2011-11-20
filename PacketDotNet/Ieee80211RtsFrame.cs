using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// RTS Frame has a ReceiverAddress[6], TransmitterAddress[6] and a FrameCheckSequence[4],
    /// these fields follow the common FrameControl[2] and DurationId[2] fields
    /// </summary>
    public class Ieee80211RtsFrame : Ieee80211MacFrame
    {
        /// <summary>
        /// ReceiverAddress
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
        /// TransmitterAddress
        /// </summary>
        public PhysicalAddress TransmitterAddress
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
        public override int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength + 
                    Ieee80211MacFields.DurationIDLength + 
                    (Ieee80211MacFields.AddressLength * 2));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">
        /// A <see cref="Ieee80211MacFrame"/>
        /// </param>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211RtsFrame(ByteArraySegment bas)
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
            return string.Format("FrameControl {0}, FrameCheckSequence {1}, [RTSFrame RA {2} TA {3}]",
                                 FrameControl.ToString(),
                                 FrameCheckSequence,
                                 ReceiverAddress.ToString(),
                                 TransmitterAddress.ToString());
        }
    }
}
