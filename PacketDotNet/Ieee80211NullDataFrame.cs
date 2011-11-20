using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    public class Ieee80211NullDataFrame : Ieee80211DataFrame
    {
        public override int FrameSize
        {
            get
            {
                //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                int numOfAddressFields = (FrameControl.ToDS && FrameControl.FromDS) ? 4 : 3;

                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * numOfAddressFields) +
                    Ieee80211MacFields.SequenceControlLength);
            }
        }

        public Ieee80211NullDataFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            SequenceControl = new Ieee80211SequenceControlField(SequenceControlBytes);

            header.Length = FrameSize;
        }
    }
}
