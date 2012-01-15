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

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class SequenceControlField
        {
            public UInt16 Field { get; set; }

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


            public SequenceControlField()
            {

            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field">
            /// A <see cref="UInt16"/>
            /// </param>
            public SequenceControlField(UInt16 field)
            {
                this.Field = field;
            }
        } 
    }
}
