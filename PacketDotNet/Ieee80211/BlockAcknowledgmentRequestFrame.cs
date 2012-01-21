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
            private class BlockAcknowledgmentRequestField
            {
                public readonly static int BlockAckRequestControlLength = 2;
                public readonly static int BlockAckStartingSequenceControlLength = 2;

                public readonly static int BlockAckRequestControlPosition;
                public readonly static int BlockAckStartingSequenceControlPosition;

                static BlockAcknowledgmentRequestField()
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
            public UInt16 BlockAckRequestControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                          header.Offset + BlockAcknowledgmentRequestField.BlockAckRequestControlPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BlockAcknowledgmentRequestField.BlockAckRequestControlPosition);
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
            
            public UInt16 BlockAckStartingSequenceControl {get; set;}
                
            /// <summary>
            /// Gets or sets the block ack starting sequence control.
            /// </summary>
            /// <value>
            /// The block ack starting sequence control.
            /// </value>
            public UInt16 BlockAckStartingSequenceControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                        header.Offset + BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                        header.Bytes,
                        header.Offset + BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlPosition);
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
                        BlockAcknowledgmentRequestField.BlockAckRequestControlLength +
                        BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlLength);
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
                
                //Must do this after setting header.Length as that is used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }
            
            public BlockAcknowledgmentRequestFrame (PhysicalAddress TransmitterAddress,
                                                    PhysicalAddress ReceiverAddress)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.ReceiverAddress = ReceiverAddress;
                this.TransmitterAddress = TransmitterAddress;
                this.BlockAcknowledgmentControl = new BlockAcknowledgmentControlField ();
                
                this.FrameControl.Type = FrameControlField.FrameTypes.ControlBlockAcknowledgmentRequest;
            }
            
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length < FrameSize))
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
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [BlockAcknowledgmentRequestFrame RA {2} TA {3}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     ReceiverAddress.ToString(),
                                     TransmitterAddress.ToString());
            }
        } 
    }
}
