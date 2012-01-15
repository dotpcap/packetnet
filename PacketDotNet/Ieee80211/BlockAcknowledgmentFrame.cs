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
        /// Format of the 802.11 block acknowledgment frame.
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
            
            public UInt16 BlockAckRequestControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                          header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + BlockAcknowledgmentField.BlockAckRequestControlPosition);
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
            
            public UInt16 BlockAckStartingSequenceControlBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                        header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes (value,
                        header.Bytes,
                        header.Offset + BlockAcknowledgmentField.BlockAckStartingSequenceControlPosition);
                }
            }
   
            public Byte[] BlockAckBitmap {get; set;}
            
            public Byte[] BlockAckBitmapBytes
            {
                get
                {
                    Byte[] bitmap = new Byte[GetBitmapLength()];
                    Array.Copy(header.Bytes,
                        (BlockAcknowledgmentField.BlockAckBitmapPosition),
                        bitmap,
                        0,
                        GetBitmapLength());
                    return bitmap;
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
                        BlockAcknowledgmentField.BlockAckRequestControlLength +
                        BlockAcknowledgmentField.BlockAckStartingSequenceControlLength +
                        GetBitmapLength());
                }
            }


            private int GetBitmapLength()
            {
                return BlockAcknowledgmentControl.CompressedBitmap ? 8 : 64;
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public BlockAcknowledgmentFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                ReceiverAddress = GetAddress (0);
                TransmitterAddress = GetAddress (1);
                BlockAcknowledgmentControl = new BlockAcknowledgmentControlField (BlockAckRequestControlBytes);
                BlockAckStartingSequenceControl = BlockAckStartingSequenceControlBytes;
                BlockAckBitmap = BlockAckBitmapBytes;
                header.Length = FrameSize;
                
                //Must do this after setting header.Length as that is used in calculating the posistion of the FCS
                FrameCheckSequence = FrameCheckSequenceBytes;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("FrameControl {0}, FrameCheckSequence {1}, [BlockAcknowledgmentFrame RA {2} TA {3}]",
                                     FrameControl.ToString(),
                                     FrameCheckSequence,
                                     ReceiverAddress.ToString(),
                                     TransmitterAddress.ToString());
            }
        } 
    }
}
