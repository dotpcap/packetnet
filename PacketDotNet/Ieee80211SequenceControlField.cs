using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    public class Ieee80211SequenceControlField
    {
        private UInt16 Field;


        public short SequenceNumber
        {
            get
            {
                return (short)(Field >> 4);
            }

            set
            {
                //Use the & mask to make sure we only overwrite the sequence number part of the field
                Field |= (UInt16)(value << 4);
            }
        }

        public byte FragmentNumber
        {
            get
            {
                return (byte)(Field & 0x000F);
            }

            set
            {
                //move the fragment number back into the correct position
                Field |= (UInt16)(value & 0x0F);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="UInt16"/>
        /// </param>
        public Ieee80211SequenceControlField(UInt16 field)
        {
            this.Field = field;
        }
    }
}
