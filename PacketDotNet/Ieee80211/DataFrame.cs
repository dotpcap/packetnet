/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

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
        /// <summary>
        /// Data frame.
        /// </summary>
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
            
            /// <summary>
            /// Assigns the default MAC address of 00-00-00-00-00-00 to all address fields.
            /// </summary>
            protected void AssignDefaultAddresses ()
            {
                PhysicalAddress zeroAddress = PhysicalAddress.Parse ("000000000000");
                
                SourceAddress = zeroAddress;
                DestinationAddress = zeroAddress;
                TransmitterAddress = zeroAddress;
                ReceiverAddress = zeroAddress;
                BssId = zeroAddress;
            }
            
            /// <summary>
            /// Reads the addresses from the backing ByteArraySegment into the the address properties.
            /// </summary>
            /// <remarks>
            /// The <see cref="PacketDotNet.Ieee80211.FrameControlField"/> ToDS and FromDS properties dictate
            /// which of the 4 possible address fields is read into which address property.
            /// </remarks>
            protected void ReadAddresses ()
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
            
            /// <summary>
            /// Writes the address properties into the backing <see cref="PacketDotNet.Utils.ByteArraySegment"/>.
            /// </summary>
            /// <remarks>
            /// The address position into which a particular address property is written is determined by the 
            /// value of <see cref="PacketDotNet.Ieee80211.FrameControlField"/> ToDS and FromDS properties.
            /// </remarks>
            protected void WriteAddressBytes ()
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
            protected UInt16 SequenceControlBytes
            {
                get
                {
					if(header.Length >= (MacFields.SequenceControlPosition + MacFields.SequenceControlLength))
					{
						return EndianBitConverter.Little.ToUInt16 (header.Bytes,
						                                           (header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
					}
					else
					{
						return 0;
					}
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
            
            /// <summary>
            /// Returns a string with a description of the addresses used in the packet.
            /// This is used as a compoent of the string returned by ToString().
            /// </summary>
            /// <returns>
            /// The address string.
            /// </returns>
            protected override String GetAddressString()
            {
                String addresses = null;
                if (FrameControl.ToDS && FrameControl.FromDS)
                {
                    addresses = String.Format("SA {0} DA {1} TA {2} RA {3}", 
                                              SourceAddress, 
                                              DestinationAddress, 
                                              TransmitterAddress, 
                                              ReceiverAddress);
                }
                else
                {
                    addresses = String.Format("SA {0} DA {1} BSSID {2}",
                                              SourceAddress, 
                                              DestinationAddress, 
                                              BssId);
                }
                return addresses;
            }
        } 
    }
}
