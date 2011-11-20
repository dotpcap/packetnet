using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    public class Ieee80211BlockAcknowledgmentRequestFrame : Ieee80211MacFrame
    {
        private class Ieee80211BlockAcknowledgmentRequestField
        {
            public readonly static int BlockAckRequestControlLength = 2;
            public readonly static int BlockAckStartingSequenceControlLength = 2;

            public readonly static int BlockAckRequestControlPosition;
            public readonly static int BlockAckStartingSequenceControlPosition;

            static Ieee80211BlockAcknowledgmentRequestField()
            {
                BlockAckRequestControlPosition = Ieee80211MacFields.DurationIDPosition + Ieee80211MacFields.DurationIDLength + (2 * Ieee80211MacFields.AddressLength);
                BlockAckStartingSequenceControlPosition = BlockAckRequestControlPosition + BlockAckRequestControlLength;
            }
        }

        /// <summary>
        /// Receiver address
        /// </summary>
        public PhysicalAddress ReceiverAddress
        {
            get
            {
                return GetAddress(0);
            }

            set
            {
                SetAddress(0, value);
            }
        }

        /// <summary>
        /// Transmitter address
        /// </summary>
        public PhysicalAddress TransmitterAddress
        {
            get
            {
                return GetAddress(1);
            }

            set
            {
                SetAddress(1, value);
            }
        }

        /// <summary>
        /// Block acknowledgment control bytes are the first two bytes of the frame
        /// </summary>
        public UInt16 BlockAckRequestControlBytes
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211BlockAcknowledgmentRequestField.BlockAckRequestControlPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211BlockAcknowledgmentRequestField.BlockAckRequestControlPosition);
            }
        }

        /// <summary>
        /// Block acknowledgment control field
        /// </summary>
        public Ieee80211BlockAcknowledgmentControlField BlockAcknowledgmentControl
        {
            get;
            set;
        }

        public UInt16 BlockAckStartingSequenceControl
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                    header.Offset + Ieee80211BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                    header.Bytes,
                    header.Offset + Ieee80211BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlPosition);
            }
        }


        /// <summary>
        /// Length of the frame
        /// </summary>
        override public int FrameSize
        {
            get
            {
                return (Ieee80211MacFields.FrameControlLength +
                    Ieee80211MacFields.DurationIDLength +
                    (Ieee80211MacFields.AddressLength * 2) +
                    Ieee80211BlockAcknowledgmentRequestField.BlockAckRequestControlLength +
                    Ieee80211BlockAcknowledgmentRequestField.BlockAckStartingSequenceControlLength);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211BlockAcknowledgmentRequestFrame(ByteArraySegment bas)
        {
            header = new ByteArraySegment(bas);

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);
            Duration = new Ieee80211DurationField(DurationBytes);
            BlockAcknowledgmentControl = new Ieee80211BlockAcknowledgmentControlField(BlockAckRequestControlBytes);

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
