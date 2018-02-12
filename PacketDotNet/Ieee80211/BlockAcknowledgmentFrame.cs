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
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Format of the 802.11 block acknowledgment frame.
    ///     http://en.wikipedia.org/wiki/Block_acknowledgement
    /// </summary>
    public class BlockAcknowledgmentFrame : MacFrame
    {
        private Byte[] _blockAckBitmap;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public BlockAcknowledgmentFrame(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            this.FrameControl = new FrameControlField(this.FrameControlBytes);
            this.Duration = new DurationField(this.DurationBytes);
            this.ReceiverAddress = this.GetAddress(0);
            this.TransmitterAddress = this.GetAddress(1);
            this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField(this.BlockAckRequestControlBytes);
            this.BlockAckStartingSequenceControl = this.BlockAckStartingSequenceControlBytes;
            this.BlockAckBitmap = this.BlockAckBitmapBytes;
            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.BlockAcknowledgmentFrame" /> class.
        /// </summary>
        /// <param name='transmitterAddress'>
        ///     Transmitter address.
        /// </param>
        /// <param name='receiverAddress'>
        ///     Receiver address.
        /// </param>
        /// <param name='blockAckBitmap'>
        ///     The Block ack bitmap signalling the receive status of the MSDUs.
        /// </param>
        public BlockAcknowledgmentFrame(PhysicalAddress transmitterAddress,
            PhysicalAddress receiverAddress,
            Byte[] blockAckBitmap)
        {
            this.FrameControl = new FrameControlField();
            this.Duration = new DurationField();
            this.ReceiverAddress = receiverAddress;
            this.TransmitterAddress = transmitterAddress;
            this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField();
            this.BlockAckBitmap = blockAckBitmap;

            this.FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgment;
        }


        /// <summary>
        ///     Length of the frame
        /// </summary>
        public override Int32 FrameSize => (MacFields.FrameControlLength +
                                            MacFields.DurationIDLength +
                                            (MacFields.AddressLength * 2) +
                                            BlockAcknowledgmentField.BlockAckRequestControlLength +
                                            BlockAcknowledgmentField.BlockAckStartingSequenceControlLength +
                                            this.GetBitmapLength());

        /// <summary>
        ///     Gets or sets the block ack bitmap used to indicate the receive status of the MPDUs.
        /// </summary>
        /// <value>
        ///     The block ack bitmap.
        /// </value>
        /// <exception cref='ArgumentException'>
        ///     Is thrown when the bitmap is of an incorrect lenght. The bitmap must be either 8 or 64 btyes longs depending on
        ///     whether or not
        ///     it is compressed.
        /// </exception>
        public Byte[] BlockAckBitmap
        {
            get => this._blockAckBitmap;

            set
            {
                if (value.Length == 8)
                {
                    this.BlockAcknowledgmentControl.CompressedBitmap = true;
                }
                else if (value.Length == 64)
                {
                    this.BlockAcknowledgmentControl.CompressedBitmap = false;
                }
                else
                {
                    throw new ArgumentException("Invalid BlockAckBitmap size. Must be either 8 or 64 bytes long.");
                }

                this._blockAckBitmap = value;
            }
        }

        /// <summary>
        ///     Block acknowledgment control field
        /// </summary>
        public BlockAcknowledgmentControlField BlockAcknowledgmentControl { get; set; }

        /// <summary>
        ///     Gets or sets the block ack starting sequence control.
        /// </summary>
        /// <value>
        ///     The block ack starting sequence control.
        /// </value>
        public UInt16 BlockAckStartingSequenceControl { get; set; }

        /// <summary>
        ///     Receiver address
        /// </summary>
        public PhysicalAddress ReceiverAddress { get; set; }

        /// <summary>
        ///     Transmitter address
        /// </summary>
        public PhysicalAddress TransmitterAddress { get; set; }

        private Byte[] BlockAckBitmapBytes
        {
            get
            {
                Byte[] bitmap = new Byte[this.GetBitmapLength()];
                if (this.HeaderByteArraySegment.Length >=
                    (BlockAcknowledgmentField.BlockAckBitmapPosition + this.GetBitmapLength()))
                {
                    Array.Copy(this.HeaderByteArraySegment.Bytes,
                        (BlockAcknowledgmentField.BlockAckBitmapPosition),
                        bitmap,
                        0, this.GetBitmapLength());
                }

                return bitmap;
            }

            set => Array.Copy(this.BlockAckBitmap,
                0, this.HeaderByteArraySegment.Bytes,
                BlockAcknowledgmentField.BlockAckBitmapPosition, this.GetBitmapLength());
        }

        /// <summary>
        ///     Gets or sets the block ack request control bytes.
        /// </summary>
        /// <value>
        ///     The block ack request control bytes.
        /// </value>
        private UInt16 BlockAckRequestControlBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (BlockAcknowledgmentField.BlockAckRequestControlPosition +
                     BlockAcknowledgmentField.BlockAckRequestControlLength))
                {
                    return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
        }

        private UInt16 BlockAckStartingSequenceControlBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition +
                     BlockAcknowledgmentField.BlockAckStartingSequenceControlLength))
                {
                    return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset +
                        BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
        }

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
            this.SetAddress(1, this.TransmitterAddress);

            this.BlockAckRequestControlBytes = this.BlockAcknowledgmentControl.Field;
            this.BlockAckStartingSequenceControlBytes = this.BlockAckStartingSequenceControl;
            this.BlockAckBitmapBytes = this.BlockAckBitmap;

            this.HeaderByteArraySegment.Length = this.FrameSize;
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
            return String.Format("RA {0} TA {1}", this.ReceiverAddress, this.TransmitterAddress);
        }


        private Int32 GetBitmapLength()
        {
            return this.BlockAcknowledgmentControl.CompressedBitmap ? 8 : 64;
        }

        private class BlockAcknowledgmentField
        {
            public static readonly Int32 BlockAckBitmapPosition;
            public static readonly Int32 BlockAckRequestControlLength = 2;

            public static readonly Int32 BlockAckRequestControlPosition;
            public static readonly Int32 BlockAckStartingSequenceControlLength = 2;
            public static readonly Int32 BlockAckStartingSequenceControlPosition;

            static BlockAcknowledgmentField()
            {
                BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength +
                                                 (2 * MacFields.AddressLength);
                BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
                BlockAckBitmapPosition =
                    BlockAckStartingSequenceControlPosition + BlockAckStartingSequenceControlLength;
            }
        }
    }
}