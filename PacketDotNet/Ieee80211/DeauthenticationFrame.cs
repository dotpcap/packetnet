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
        /// Deauthentication frame.
        /// </summary>
        public class DeauthenticationFrame : ManagementFrame
        {
            private class DeauthenticationFields
            {
                public readonly static int ReasonCodeLength = 2;

                public readonly static int ReasonCodePosition;

                static DeauthenticationFields()
                {
                    ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                }
            }
   
            /// <summary>
            /// Gets the reason for deauthentication.
            /// </summary>
            /// <value>
            /// The reason.
            /// </value>
            public ReasonCode Reason { get; set;}
            
            private ReasonCode ReasonBytes
            {
                get
                {
					if(header.Length >= (DeauthenticationFields.ReasonCodePosition + DeauthenticationFields.ReasonCodeLength))
					{
						return (ReasonCode)EndianBitConverter.Little.ToUInt16 (header.Bytes,
						                                                       header.Offset + DeauthenticationFields.ReasonCodePosition);
					}
					else
					{
						return ReasonCode.Unspecified;
					}
                }
                
                set
                {
                    EndianBitConverter.Little.CopyBytes ((UInt16)value,
                        header.Bytes,
                        header.Offset + DeauthenticationFields.ReasonCodePosition);
                }
            }

            /// <summary>
            /// Gets the size of the frame.
            /// </summary>
            /// <value>
            /// The size of the frame.
            /// </value>
            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        DeauthenticationFields.ReasonCodeLength);
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public DeauthenticationFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
                Reason = ReasonBytes;

                header.Length = FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DeauthenticationFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (MAC Address of the Access Point).
            /// </param>
            public DeauthenticationFrame (PhysicalAddress SourceAddress,
                                          PhysicalAddress DestinationAddress,
                                          PhysicalAddress BssId)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.DestinationAddress = DestinationAddress;
                this.SourceAddress = SourceAddress;
                this.BssId = BssId;
                this.SequenceControl = new SequenceControlField ();
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementDeauthentication;
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
                SetAddress (0, DestinationAddress);
                SetAddress (1, SourceAddress);
                SetAddress (2, BssId);
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.ReasonBytes = this.Reason;
                
                header.Length = FrameSize;
            }

        } 
    }
}
