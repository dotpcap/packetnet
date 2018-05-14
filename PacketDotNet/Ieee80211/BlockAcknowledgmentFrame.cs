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
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Format of the 802.11 block acknowledgment frame.
        /// http://en.wikipedia.org/wiki/Block_acknowledgement
        /// </summary>
        public class BlockAcknowledgmentFrame : MacFrame
        {
            private Byte[] blockAckBitmap;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment" />
            /// </param>
            public BlockAcknowledgmentFrame(ByteArraySegment bas)
            {
                Header = new ByteArraySegment(bas);

                FrameControl = new FrameControlField(FrameControlBytes);
                Duration = new DurationField(DurationBytes);
                ReceiverAddress = GetAddress(0);
                TransmitterAddress = GetAddress(1);
                BlockAcknowledgmentControl = new BlockAcknowledgmentControlField(BlockAckRequestControlBytes);
                BlockAckStartingSequenceControl = BlockAckStartingSequenceControlBytes;
                BlockAckBitmap = BlockAckBitmapBytes;
                Header.Length = FrameSize;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.BlockAcknowledgmentFrame" /> class.
            /// </summary>
            /// <param name='TransmitterAddress'>
            /// Transmitter address.
            /// </param>
            /// <param name='ReceiverAddress'>
            /// Receiver address.
            /// </param>
            /// <param name='BlockAckBitmap'>
            /// The Block ack bitmap signalling the receive status of the MSDUs.
            /// </param>
            public BlockAcknowledgmentFrame
            (
                PhysicalAddress TransmitterAddress,
                PhysicalAddress ReceiverAddress,
                Byte[] BlockAckBitmap)
            {
                FrameControl = new FrameControlField();
                Duration = new DurationField();
                this.ReceiverAddress = ReceiverAddress;
                this.TransmitterAddress = TransmitterAddress;
                BlockAcknowledgmentControl = new BlockAcknowledgmentControlField();
                this.BlockAckBitmap = BlockAckBitmap;

                FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgment;
            }

            /// <summary>
            /// Gets or sets the block ack bitmap used to indicate the receive status of the MPDUs.
            /// </summary>
            /// <value>
            /// The block ack bitmap.
            /// </value>
            /// <exception cref='ArgumentException'>
            /// Is thrown when the bitmap is of an incorrect lenght. The bitmap must be either 8 or 64 btyes longs depending on whether or not
            /// it is compressed.
            /// </exception>
            public Byte[] BlockAckBitmap
            {
                get => blockAckBitmap;

                set
                {
                    if (value.Length == 8)
                    {
                        BlockAcknowledgmentControl.CompressedBitmap = true;
                    }
                    else if (value.Length == 64)
                    {
                        BlockAcknowledgmentControl.CompressedBitmap = false;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid BlockAckBitmap size. Must be either 8 or 64 bytes long.");
                    }

                    blockAckBitmap = value;
                }
            }

            /// <summary>
            /// Block acknowledgment control field
            /// </summary>
            public BlockAcknowledgmentControlField BlockAcknowledgmentControl { get; set; }

            /// <summary>
            /// Gets or sets the block ack starting sequence control.
            /// </summary>
            /// <value>
            /// The block ack starting sequence control.
            /// </value>
            public UInt16 BlockAckStartingSequenceControl { get; set; }


            /// <summary>
            /// Length of the frame
            /// </summary>
            public override Int32 FrameSize => (MacFields.FrameControlLength +
                                                MacFields.DurationIDLength +
                                                (MacFields.AddressLength * 2) +
                                                BlockAcknowledgmentField.BlockAckRequestControlLength +
                                                BlockAcknowledgmentField.BlockAckStartingSequenceControlLength +
                                                GetBitmapLength());

            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress { get; set; }

            /// <summary>
            /// Transmitter address
            /// </summary>
            public PhysicalAddress TransmitterAddress { get; set; }

            private Byte[] BlockAckBitmapBytes
            {
                get
                {
                    Byte[] bitmap = new Byte[GetBitmapLength()];
                    if (Header.Length >= (BlockAcknowledgmentField.BlockAckBitmapPosition + GetBitmapLength()))
                    {
                        Array.Copy(Header.Bytes,
                                   (BlockAcknowledgmentField.BlockAckBitmapPosition),
                                   bitmap,
                                   0,
                                   GetBitmapLength());
                    }

                    return bitmap;
                }

                set => Array.Copy(BlockAckBitmap,
                                  0,
                                  Header.Bytes,
                                  BlockAcknowledgmentField.BlockAckBitmapPosition,
                                  GetBitmapLength());
            }

            /// <summary>
            /// Gets or sets the block ack request control bytes.
            /// </summary>
            /// <value>
            /// The block ack request control bytes.
            /// </value>
            private UInt16 BlockAckRequestControlBytes
            {
                get
                {
                    if (Header.Length >=
                        (BlockAcknowledgmentField.BlockAckRequestControlPosition + BlockAcknowledgmentField.BlockAckRequestControlLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                                  Header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
                    }

                    return 0;
                }

                set => EndianBitConverter.Little.CopyBytes(value,
                                                           Header.Bytes,
                                                           Header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
            }

            private UInt16 BlockAckStartingSequenceControlBytes
            {
                get
                {
                    if (Header.Length >=
                        (BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition + BlockAcknowledgmentField.BlockAckStartingSequenceControlLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                                  Header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
                    }

                    return 0;
                }

                set => EndianBitConverter.Little.CopyBytes(value,
                                                           Header.Bytes,
                                                           Header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
            }


            private Int32 GetBitmapLength()
            {
                return BlockAcknowledgmentControl.CompressedBitmap ? 8 : 64;
            }

            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues()
            {
                if ((Header == null) || (Header.Length > (Header.BytesLength - Header.Offset)) || (Header.Length < FrameSize))
                {
                    Header = new ByteArraySegment(new Byte[FrameSize]);
                }

                FrameControlBytes = FrameControl.Field;
                DurationBytes = Duration.Field;
                SetAddress(0, ReceiverAddress);
                SetAddress(1, TransmitterAddress);

                BlockAckRequestControlBytes = BlockAcknowledgmentControl.Field;
                BlockAckStartingSequenceControlBytes = BlockAckStartingSequenceControl;
                BlockAckBitmapBytes = BlockAckBitmap;

                Header.Length = FrameSize;
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
                return String.Format("RA {0} TA {1}", ReceiverAddress, TransmitterAddress);
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
                    BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
                    BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
                    BlockAckBitmapPosition = BlockAckStartingSequenceControlPosition + BlockAckStartingSequenceControlLength;
                }
            }
        }
    }
}