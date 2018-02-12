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
        /// Data data frame.
        /// </summary>
        public class DataDataFrame : DataFrame
        {
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
                    //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                    int numOfAddressFields = (this.FrameControl.ToDS && this.FrameControl.FromDS) ? 4 : 3;

                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * numOfAddressFields) +
                        MacFields.SequenceControlLength);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DataDataFrame"/> class.
            /// </summary>
            /// <param name='bas'>
            /// Bas.
            /// </param>
            public DataDataFrame (ByteArraySegment bas)
            {
                this.header = new ByteArraySegment (bas);

                this.FrameControl = new FrameControlField (this.FrameControlBytes);
                this.Duration = new DurationField (this.DurationBytes);
                this.SequenceControl = new SequenceControlField (this.SequenceControlBytes);
                this.ReadAddresses (); //must do this after reading FrameControl

                this.header.Length = this.FrameSize; 
                var availablePayloadLength = this.GetAvailablePayloadLength();
				if(availablePayloadLength > 0)
				{
				    this.payloadPacketOrData.TheByteArraySegment = this.header.EncapsulatedBytes (availablePayloadLength);
				}
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DataDataFrame"/> class.
            /// </summary>
            public DataDataFrame ()
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.SequenceControl = new SequenceControlField ();
                this.AssignDefaultAddresses ();

                this.FrameControl.SubType = FrameControlField.FrameSubTypes.Data;
            }
            
            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues ()
            {
                if ((this.header == null) || (this.header.Length > (this.header.BytesLength - this.header.Offset)) || (this.header.Length < this.FrameSize))
                {
                    this.header = new ByteArraySegment (new Byte[this.FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.WriteAddressBytes ();
            }

        } 
    }

