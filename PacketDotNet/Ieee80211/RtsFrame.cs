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
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// RTS Frame has a ReceiverAddress[6], TransmitterAddress[6] and a FrameCheckSequence[4],
        /// these fields follow the common FrameControl[2] and DurationId[2] fields
        /// </summary>
        public class RtsFrame : MacFrame
        {
            /// <summary>
            /// ReceiverAddress
            /// </summary>
            public PhysicalAddress ReceiverAddress {get; set;}

            /// <summary>
            /// TransmitterAddress
            /// </summary>
            public PhysicalAddress TransmitterAddress {get; set;}

            /// <summary>
            /// Length of the frame
            /// </summary>
            public override int FrameSize
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
            public RtsFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                ReceiverAddress = GetAddress (0);
                TransmitterAddress = GetAddress(1);
				
				header.Length = FrameSize;
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
                return String.Format("RA {0} TA {1}",
                                     ReceiverAddress, 
                                     TransmitterAddress);
            }
        } 
    }
}
