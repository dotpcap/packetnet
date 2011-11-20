using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    public abstract class Ieee80211DataFrame : Ieee80211MacFrame
    {
        /// <summary>
        /// SourceAddress
        /// </summary>
        public PhysicalAddress SourceAddress
        {
            get
            {
                if (!FrameControl.FromDS)
                {
                    return GetAddress(1);
                }
                else if (!FrameControl.ToDS)
                {
                    return GetAddress(2);
                }
                else
                {
                    //WDS modes so we use the 4th address field
                    return GetAddress(3);
                }
            }

            set
            {
                if (!FrameControl.FromDS)
                {
                    SetAddress(1, value);
                }
                else if (!FrameControl.ToDS)
                {
                    SetAddress(2, value);
                }
                else
                {
                    //WDS modes so we use the 4th address field
                    SetAddress(3, value);
                }
            }
        }

        /// <summary>
        /// DestinationAddress
        /// </summary>
        public PhysicalAddress DestinationAddress
        {
            get
            {
                if (FrameControl.ToDS)
                {
                    return GetAddress(2);
                }
                else
                {
                    return GetAddress(0);
                }
            }

            set
            {
                if (FrameControl.ToDS)
                {
                    SetAddress(2, value);
                }
                else
                {
                    SetAddress(0, value);
                }
            }
        }

        /// <summary>
        /// ReceiverAddress
        /// </summary>
        public PhysicalAddress ReceiverAddress
        {
            get
            {
                if ((FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    return GetAddress(0);
                }
                else
                {
                    //WDS mode (ToDS and FromDS) is the only time 
                    //the receiver address is valid
                    return null;
                }
            }

            set
            {
                if ((FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    SetAddress(0, value);
                }
            }
        }

        /// <summary>
        /// TransmitterAddress
        /// </summary>
        public PhysicalAddress TransmitterAddress
        {
            get
            {
                if ((FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    return GetAddress(1);
                }
                else
                {
                    //WDS mode (ToDS and FromDS) is the only time 
                    //the transmitter address is valid
                    return null;
                }
            }

            set
            {
                if ((FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    SetAddress(1, value);
                }
            }
        }

        /// <summary>
        /// BssID
        /// </summary>
        public PhysicalAddress BssId
        {
            get
            {
                if ((!FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    return GetAddress(2);
                }
                else if((FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    return GetAddress(0);
                }
                else if ((!FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    return GetAddress(1);
                }
                else
                {
                    //both are true so we are in WDS mode again. BSSID is not valid in this mode
                    return null;
                }
            }

            set
            {
                if ((!FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    SetAddress(2, value);
                }
                else if ((FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    SetAddress(0, value);
                }
                else if ((!FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    SetAddress(1, value);
                }
            }
        }


        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        public UInt16 SequenceControlBytes
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      (header.Offset + Ieee80211MacFields.Address1Position + (Ieee80211MacFields.AddressLength * 3)));
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 (header.Offset + Ieee80211MacFields.Address1Position + (Ieee80211MacFields.AddressLength * 3)));
            }
        }

        /// <summary>
        /// Sequence control field
        /// </summary>
        public Ieee80211SequenceControlField SequenceControl
        {
            get;
            set;
        }


        
        
    }
}
