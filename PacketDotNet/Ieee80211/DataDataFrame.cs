/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

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
        /// <param name="byteArraySegment">
        /// byteArraySegment.
        /// </param>
        public DataDataFrame(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            ReadAddresses(); //must do this after reading FrameControl

            Header.Length = FrameSize;
            var availablePayloadLength = GetAvailablePayloadLength();
            if (availablePayloadLength > 0)
            {
                PayloadPacketOrData.Value.ByteArraySegment = Header.NextSegment(availablePayloadLength);
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
        public sealed override int FrameSize
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
                Header = new ByteArraySegment(new byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SequenceControlBytes = SequenceControl.Field;
            WriteAddressBytes();
        }
    }
}