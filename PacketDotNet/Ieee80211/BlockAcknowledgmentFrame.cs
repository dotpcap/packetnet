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
        /// Format of the 802.11 block acknowledgment frame.
        /// http://en.wikipedia.org/wiki/Block_acknowledgement
        /// </summary>
        public class BlockAcknowledgmentFrame : MacFrame
        {
            private class BlockAcknowledgmentField
            {
                public readonly static int BlockAckRequestControlLength = 2;
                public readonly static int BlockAckStartingSequenceControlLength = 2;

                public readonly static int BlockAckRequestControlPosition;
                public readonly static int BlockAckStartingSequenceControlPosition;
                public readonly static int BlockAckBitmapPosition;

                static BlockAcknowledgmentField()
                {
                    BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
                    BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
                    BlockAckBitmapPosition = BlockAckStartingSequenceControlPosition + BlockAckStartingSequenceControlLength;
                }
            }

            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress {get; set;}

            /// <summary>
            /// Transmitter address
            /// </summary>
            public PhysicalAddress TransmitterAddress { get; set; }

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
					if(this.header.Length >= 
					   (BlockAcknowledgmentField.BlockAckRequestControlPosition + BlockAcknowledgmentField.BlockAckRequestControlLength))
					{
						return EndianBitConverter.Little.ToUInt16(this.header.Bytes, this.header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
					}
					else
					{
						return 0;
					}
                }

                set => EndianBitConverter.Little.CopyBytes(value, this.header.Bytes, this.header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
            }

            /// <summary>
            /// Block acknowledgment control field
            /// </summary>
            public BlockAcknowledgmentControlField BlockAcknowledgmentControl
            {
                get;
                set;
            }
   
            /// <summary>
            /// Gets or sets the block ack starting sequence control.
            /// </summary>
            /// <value>
            /// The block ack starting sequence control.
            /// </value>
            public UInt16 BlockAckStartingSequenceControl {get; set;}
            
            private UInt16 BlockAckStartingSequenceControlBytes
            {
                get
                {
					if(this.header.Length >= 
					   (BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition + BlockAcknowledgmentField.BlockAckStartingSequenceControlLength))
					{
						return EndianBitConverter.Little.ToUInt16 (this.header.Bytes, this.header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
					}
					else
					{
						return 0;
					}
                }

                set => EndianBitConverter.Little.CopyBytes (value, this.header.Bytes, this.header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
            }
   
            private byte[] blockAckBitmap;
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
                get => this.blockAckBitmap;

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
                        throw new ArgumentException ("Invalid BlockAckBitmap size. Must be either 8 or 64 bytes long.");
                    }

                    this.blockAckBitmap = value;
                }
            }
            
            private Byte[] BlockAckBitmapBytes
            {
                get
                {
					Byte[] bitmap = new Byte[this.GetBitmapLength ()];
					if(this.header.Length >= (BlockAcknowledgmentField.BlockAckBitmapPosition + this.GetBitmapLength()))
					{
						Array.Copy (this.header.Bytes,
						            (BlockAcknowledgmentField.BlockAckBitmapPosition),
						            bitmap,
						            0, this.GetBitmapLength ());
					}
					return bitmap;
                }
                
                set => Array.Copy (this.BlockAckBitmap,
                    0, this.header.Bytes,
                    BlockAcknowledgmentField.BlockAckBitmapPosition, this.GetBitmapLength());
            }


            /// <summary>
            /// Length of the frame
            /// </summary>
            override public int FrameSize => (MacFields.FrameControlLength +
                                              MacFields.DurationIDLength +
                                              (MacFields.AddressLength * 2) +
                                              BlockAcknowledgmentField.BlockAckRequestControlLength +
                                              BlockAcknowledgmentField.BlockAckStartingSequenceControlLength + this.GetBitmapLength());


            private int GetBitmapLength()
            {
                return this.BlockAcknowledgmentControl.CompressedBitmap ? 8 : 64;
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public BlockAcknowledgmentFrame (ByteArraySegment bas)
            {
                this.header = new ByteArraySegment (bas);

                this.FrameControl = new FrameControlField (this.FrameControlBytes);
                this.Duration = new DurationField (this.DurationBytes);
                this.ReceiverAddress = this.GetAddress (0);
                this.TransmitterAddress = this.GetAddress (1);
                this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField (this.BlockAckRequestControlBytes);
                this.BlockAckStartingSequenceControl = this.BlockAckStartingSequenceControlBytes;
                this.BlockAckBitmap = this.BlockAckBitmapBytes;
                this.header.Length = this.FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.BlockAcknowledgmentFrame"/> class.
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
            public BlockAcknowledgmentFrame (PhysicalAddress TransmitterAddress,
                                             PhysicalAddress ReceiverAddress,
                                             Byte[] BlockAckBitmap)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.ReceiverAddress = ReceiverAddress;
                this.TransmitterAddress = TransmitterAddress;
                this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField ();
                this.BlockAckBitmap = BlockAckBitmap;

                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgment;
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
                this.SetAddress (0, this.ReceiverAddress);
                this.SetAddress (1, this.TransmitterAddress);
                
                this.BlockAckRequestControlBytes = this.BlockAcknowledgmentControl.Field;
                this.BlockAckStartingSequenceControlBytes = this.BlockAckStartingSequenceControl;
                this.BlockAckBitmapBytes = this.BlockAckBitmap;

                this.header.Length = this.FrameSize;
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
                return String.Format("RA {0} TA {1}", this.ReceiverAddress, this.TransmitterAddress);
            }
        } 
    }