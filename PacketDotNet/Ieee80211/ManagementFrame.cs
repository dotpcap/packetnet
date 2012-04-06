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
using System.Net.NetworkInformation;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Format of a CTS or an ACK frame
        /// </summary>
        public abstract class ManagementFrame : MacFrame
        {
            /// <summary>
            /// DestinationAddress
            /// </summary>
            public PhysicalAddress DestinationAddress {get; set;}

            /// <summary>
            /// SourceAddress
            /// </summary>
            public PhysicalAddress SourceAddress {get; set;}

            /// <summary>
            /// BssID
            /// </summary>
            public PhysicalAddress BssId {get; set;}


            /// <summary>
            /// Frame control bytes are the first two bytes of the frame
            /// </summary>
            protected UInt16 SequenceControlBytes
            {
                get
                {
					if(header.Length >= (MacFields.SequenceControlPosition + MacFields.SequenceControlLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          (header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
					}
					else
					{
						return 0;
					}
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     (header.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
                }
            }

            /// <summary>
            /// Sequence control field
            /// </summary>
            public SequenceControlField SequenceControl {get; set;}
            
            /// <summary>
            /// Returns a string with a description of the addresses used in the packet.
            /// This is used as a compoent of the string returned by ToString().
            /// </summary>
            /// <returns>
            /// The address string.
            /// </returns>
            protected override String GetAddressString()
            {
                return String.Format("SA {0} DA {1} BSSID {2}",
                                     SourceAddress, 
                                     DestinationAddress, 
                                     BssId);
            }
        } 
    }

}
