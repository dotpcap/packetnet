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
        /// Contention free end frame.
        /// </summary>
        public class ContentionFreeEndFrame : MacFrame
        {
            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress {get; set;}

            /// <summary>
            /// BSS ID
            /// </summary>
            public PhysicalAddress BssId {get; set;}

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
            public ContentionFreeEndFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                ReceiverAddress = GetAddress (0);
                BssId = GetAddress (1);
                
                header.Length = FrameSize;
            }
   
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ContentionFreeEndFrame"/> class.
            /// </summary>
            /// <param name='ReceiverAddress'>
            /// Receiver address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (MAC Address of the Access Point).
            /// </param>
            public ContentionFreeEndFrame (PhysicalAddress ReceiverAddress,
                                           PhysicalAddress BssId)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.ReceiverAddress = ReceiverAddress;
                this.BssId = BssId;
                
                this.FrameControl.SubType = PacketDotNet.Ieee80211.FrameControlField.FrameSubTypes.ControlCFEnd;
            }
            
            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length > (header.BytesLength - header.Offset)) || (header.Length < FrameSize))
                {
                    header = new ByteArraySegment (new Byte[FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                SetAddress (0, ReceiverAddress);
                SetAddress (1, BssId);
               
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
                return String.Format("RA {0} BSSID {1}", ReceiverAddress, BssId);
            }
        } 
    }
}
