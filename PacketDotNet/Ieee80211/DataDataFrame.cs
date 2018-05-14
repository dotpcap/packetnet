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
        /// Initializes a new instance of the <see cref="DataDataFrame" /> class.
        /// </summary>
        /// <param name='bas'>
        /// Bas.
        /// </param>
        public DataDataFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            ReadAddresses(); //must do this after reading FrameControl

            Header.Length = FrameSize;
            var availablePayloadLength = GetAvailablePayloadLength();
            if (availablePayloadLength > 0)
            {
                PayloadPacketOrData.Value.ByteArraySegment = Header.EncapsulatedBytes(availablePayloadLength);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDataFrame" /> class.
        /// </summary>
        public DataDataFrame()
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            SequenceControl = new SequenceControlField();
            AssignDefaultAddresses();

            FrameControl.SubType = FrameControlField.FrameSubTypes.Data;
        }

        /// <summary>
        /// Gets the size of the frame.
        /// </summary>
        /// <value>
        /// The size of the frame.
        /// </value>
        public sealed override Int32 FrameSize
        {
            get
            {
                //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                var numOfAddressFields = FrameControl.ToDS && FrameControl.FromDS ? 4 : 3;

                return MacFields.FrameControlLength +
                       MacFields.DurationIDLength +
                       (MacFields.AddressLength * numOfAddressFields) +
                       MacFields.SequenceControlLength;
            }
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SequenceControlBytes = SequenceControl.Field;
            WriteAddressBytes();
        }
    }
}