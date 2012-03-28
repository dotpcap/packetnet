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
using MiscUtil.Conversion;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Block acknowledgment request frame.
        /// </summary>
        public class BlockAcknowledgmentRequestFrame : MacFrame
        {
            private class BlockAckRequestField
            {
                public readonly static int BlockAckRequestControlLength = 2;
                public readonly static int BlockAckStartingSequenceControlLength = 2;

                public readonly static int BlockAckRequestControlPosition;
                public readonly static int BlockAckStartingSequenceControlPosition;

                static BlockAckRequestField()
                {
                    BlockAckRequestControlPosition = MacFields.DurationIDPosition + MacFields.DurationIDLength + (2 * MacFields.AddressLength);
                    BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
                }
            }

            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress {get; set;}

            /// <summary>
            /// Transmitter address
            /// </summary>
            public PhysicalAddress TransmitterAddress {get; set;}

            /// <summary>
            /// Block acknowledgment control bytes are the first two bytes of the frame
            /// </summary>
            private UInt16 BlockAckRequestControlBytes
            {
                get
                {
					if(header.Length >= 
					   (BlockAckRequestField.BlockAckRequestControlPosition + 
					    BlockAckRequestField.BlockAckRequestControlLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          header.Offset + BlockAckRequestField.BlockAckRequestControlPosition);
					}
					else
					{
						return 0;
					}
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BlockAckRequestField.BlockAckRequestControlPosition);
                }
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
            /// Gets or sets the sequence number of the first MSDU for which this 
            /// block acknowledgement request frame is sent
            /// </summary>
            /// <value>
            /// The block ack starting sequence control field value
            /// </value>
            public UInt16 BlockAckStartingSequenceControl {get; set;}
                
            /// <summary>
            /// Gets or sets the block ack starting sequence control.
            /// </summary>
            /// <value>
            /// The block ack starting sequence control.
            /// </value>
            private UInt16 BlockAckStartingSequenceControlBytes
            {
                get
                {
					if(header.Length >= 
					   (BlockAckRequestField.BlockAckStartingSequenceControlPosition + 
					    BlockAckRequestField.BlockAckStartingSequenceControlLength))
					{
						return EndianBitConverter.Little.ToUInt16(header.Bytes,
						                                          header.Offset + BlockAckRequestField.BlockAckStartingSequenceControlPosition);
					}
					else
					{
						return 0;
					}
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                        header.Bytes,
                        header.Offset + BlockAckRequestField.BlockAckStartingSequenceControlPosition);
                }
            }


            /// <summary>
            /// Length of the frame
            /// </summary>
            override public int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 2) +
                        BlockAckRequestField.BlockAckRequestControlLength +
                        BlockAckRequestField.BlockAckStartingSequenceControlLength);
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public BlockAcknowledgmentRequestFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                ReceiverAddress = GetAddress (0);
                TransmitterAddress = GetAddress (1);
                BlockAcknowledgmentControl = new BlockAcknowledgmentControlField (BlockAckRequestControlBytes);
                BlockAckStartingSequenceControl = BlockAckStartingSequenceControlBytes;
                
                header.Length = FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.BlockAcknowledgmentRequestFrame"/> class.
            /// </summary>
            /// <param name='TransmitterAddress'>
            /// Transmitter address.
            /// </param>
            /// <param name='ReceiverAddress'>
            /// Receiver address.
            /// </param>
            public BlockAcknowledgmentRequestFrame (PhysicalAddress TransmitterAddress,
                                                    PhysicalAddress ReceiverAddress)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.ReceiverAddress = ReceiverAddress;
                this.TransmitterAddress = TransmitterAddress;
                this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField ();
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgmentRequest;
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
                SetAddress (1, TransmitterAddress);
                
                this.BlockAckRequestControlBytes = this.BlockAcknowledgmentControl.Field;
                this.BlockAckStartingSequenceControlBytes = this.BlockAckStartingSequenceControl;
                
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
                return String.Format("RA {0} TA {1}", ReceiverAddress, TransmitterAddress);
            }
        } 
    }
}
