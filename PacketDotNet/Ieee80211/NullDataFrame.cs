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
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
    {
        /// <summary>
        /// Null data frames are like normal data frames except they carry no payload. They are primarily used for control purposes
        /// such as power management or telling an Access Point to buffer packets while a station scans other channels.
        /// </summary>
        public class NullDataFrame : DataFrame
        {
            /// <summary>
            /// Length of the frame header.
            /// 
            /// This does not include the FCS, it represents only the header bytes that would
            /// would preceed any payload.
            /// </summary>
            public override Int32 FrameSize
            {
                get
                {
                    //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                    Int32 numOfAddressFields = (this.FrameControl.ToDS && this.FrameControl.FromDS) ? 4 : 3;

                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * numOfAddressFields) +
                        MacFields.SequenceControlLength);
                }
            }
   
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.NullDataFrame"/> class.
            /// </summary>
            /// <param name='bas'>
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public NullDataFrame (ByteArraySegment bas)
            {
                this.HeaderByteArraySegment = new ByteArraySegment (bas);

                this.FrameControl = new FrameControlField (this.FrameControlBytes);
                this.Duration = new DurationField (this.DurationBytes);
                this.SequenceControl = new SequenceControlField (this.SequenceControlBytes);
                this.ReadAddresses ();

                this.HeaderByteArraySegment.Length = this.FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.NullDataFrame"/> class.
            /// </summary>
            public NullDataFrame ()
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.SequenceControl = new SequenceControlField ();
                this.AssignDefaultAddresses ();

                this.FrameControl.SubType = FrameControlField.FrameSubTypes.DataNullFunctionNoData;
            }
            
            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues ()
            {
                if ((this.HeaderByteArraySegment == null) || (this.HeaderByteArraySegment.Length > (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) || (this.HeaderByteArraySegment.Length < this.FrameSize))
                {
                    this.HeaderByteArraySegment = new ByteArraySegment (new Byte[this.FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.WriteAddressBytes ();
            }

        } 
    }

