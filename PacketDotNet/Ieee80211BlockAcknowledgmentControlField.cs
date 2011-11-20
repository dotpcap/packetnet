using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    public class Ieee80211BlockAcknowledgmentControlField
    {
        /// <summary>
        /// The available block acknowledgement policies.
        /// </summary>
        public enum AcknowledgementPolicy
        {
            /// <summary>
            /// The acknowledgement does not have to be sent immediately after the request
            /// </summary>
            Delayed = 0,
            /// <summary>
            /// The acknowledgement must be sent immediately after the request
            /// </summary>
            Immediate = 1,
        }

        /// <summary>
        /// The block acknowledgement policy in use
        /// </summary>
        public AcknowledgementPolicy Policy
        {
            get
            {
                return (AcknowledgementPolicy)(Field & 0x1);
            }
        }

        /// <summary>
        /// True if the acknowledgement can ack multi traffic ids
        /// </summary>
        public bool MultiTid
        {
            get
            {
                return (((Field >> 1) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// True if the frame is using a compressed acknowledgement bitmap.
        /// 
        /// Newer standards used a compressed bitmap reducing its size
        /// </summary>
        public bool CompressedBitmap
        {
            get
            {
                return (((Field >> 2) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// The traffic id being ack'd
        /// </summary>
        public byte Tid
        {
            get
            {
                return (byte)(Field >> 12);
            }
        }


        private UInt16 Field;

        public Ieee80211BlockAcknowledgmentControlField(UInt16 field)
        {
            Field = field;
        }
    }
}
