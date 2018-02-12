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
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Format of an ACK frame
    /// </summary>
    public class AckFrame : MacFrame
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public AckFrame(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            this.FrameControl = new FrameControlField(this.FrameControlBytes);
            this.Duration = new DurationField(this.DurationBytes);
            this.ReceiverAddress = this.GetAddress(0);

            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AckFrame" /> class.
        /// </summary>
        /// <param name='receiverAddress'>
        ///     Receiver address.
        /// </param>
        public AckFrame(PhysicalAddress receiverAddress)
        {
            this.FrameControl = new FrameControlField();
            this.Duration = new DurationField();
            this.ReceiverAddress = receiverAddress;

            this.FrameControl.SubType = FrameControlField.FrameSubTypes.ControlACK;
        }

        /// <summary>
        ///     Length of the frame
        /// </summary>
        public override Int32 FrameSize => (MacFields.FrameControlLength +
                                            MacFields.DurationIDLength +
                                            MacFields.AddressLength);

        /// <summary>
        ///     Receiver address
        /// </summary>
        public PhysicalAddress ReceiverAddress { get; set; }

        /// <summary>
        ///     Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if ((this.HeaderByteArraySegment == null) ||
                (this.HeaderByteArraySegment.Length >
                 (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) ||
                (this.HeaderByteArraySegment.Length < this.FrameSize))
            {
                this.HeaderByteArraySegment = new ByteArraySegment(new Byte[this.FrameSize]);
            }

            this.FrameControlBytes = this.FrameControl.Field;
            this.DurationBytes = this.Duration.Field;
            this.SetAddress(0, this.ReceiverAddress);
        }

        /// <summary>
        ///     Returns a string with a description of the addresses used in the packet.
        ///     This is used as a compoent of the string returned by ToString().
        /// </summary>
        /// <returns>
        ///     The address string.
        /// </returns>
        protected override String GetAddressString()
        {
            return $"RA {this.ReceiverAddress}";
        }
    }
}