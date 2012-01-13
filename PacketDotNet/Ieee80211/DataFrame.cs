using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public abstract class DataFrame : MacFrame
        {
            /// <summary>
            /// SourceAddress
            /// </summary>
            public PhysicalAddress SourceAddress { get; set; }

            /// <summary>
            /// DestinationAddress
            /// </summary>
            public PhysicalAddress DestinationAddress { get; set; }

            /// <summary>
            /// ReceiverAddress
            /// </summary>
            public PhysicalAddress ReceiverAddress { get; set; }

            /// <summary>
            /// TransmitterAddress
            /// </summary>
            public PhysicalAddress TransmitterAddress { get; set; }


            /// <summary>
            /// BssID
            /// </summary>
            public PhysicalAddress BssId { get; set; }                

            public void ReadAddresses ()
            {   
                if ((!FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    DestinationAddress = GetAddress (0);
                    SourceAddress = GetAddress (1);
                    BssId = GetAddress (2);
                }
                else if ((FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    BssId = GetAddress (0);
                    SourceAddress = GetAddress (1);
                    DestinationAddress = GetAddress (2);
                }
                else if ((!FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    DestinationAddress = GetAddress (0);
                    BssId = GetAddress (1);
                    SourceAddress = GetAddress (2);
                }
                else
                {
                    //both are true so we are in WDS mode again. BSSID is not valid in this mode
                    ReceiverAddress = GetAddress (0);
                    TransmitterAddress = GetAddress (1);
                    DestinationAddress = GetAddress (2);
                    SourceAddress = GetAddress (3);
                }
            }
            
            public void WriteAddressBytes ()
            {
                if ((!FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    SetAddress (0, DestinationAddress);
                    SetAddress (1, SourceAddress);
                    SetAddress (2, BssId);
                }
                else if ((FrameControl.ToDS) && (!FrameControl.FromDS))
                {
                    SetAddress (0, BssId);
                    SetAddress (1, SourceAddress);
                    SetAddress (2, DestinationAddress);
                }
                else if ((!FrameControl.ToDS) && (FrameControl.FromDS))
                {
                    SetAddress (0, DestinationAddress);
                    SetAddress (1, BssId);
                    SetAddress (2, SourceAddress);
                }
                else
                {
                    SetAddress (0, ReceiverAddress);
                    SetAddress (1, TransmitterAddress);
                    SetAddress (2, DestinationAddress);
                    SetAddress (3, SourceAddress);
                }
            }
            
            /// <summary>
            /// Frame control bytes are the first two bytes of the frame
            /// </summary>
            public UInt16 SequenceControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                          (header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes (value,
                                                     header.Bytes,
                                                     (header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
                }
            }

            /// <summary>
            /// Sequence control field
            /// </summary>
            public SequenceControlField SequenceControl
            {
                get;
                set;
            }
        } 
    }
}
