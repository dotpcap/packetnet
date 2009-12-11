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

namespace PacketDotNet.Utils
{
    public class ByteArrayAndOffset
    {
        //FIXME: we need to set the length in ipv4packet
        // because we don't know the encapsulated payload size
        // until we read the HeaderLength field and we can't
        // read that field until we've setup the header field with
        // a ByteArrayAndOffset, which we default to the remaining
        // bytes in the byte[]. Think about this issue to see if
        // there is a better way
        public int Length { get; internal set; }
        public byte[] Bytes { get; private set; }
        public int Offset { get; private set; }

        public ByteArrayAndOffset(byte[] Bytes, int Offset, int Length)
        {
            this.Bytes = Bytes;
            this.Offset = Offset;
            this.Length = Length;
        }

        /// <summary>
        /// Returns a contiguous byte[] from this container, if necessary, by copying
        /// the bytes from the current offset into a newly allocated byte[].
        /// NeedsCopyForActualBytes can be used to determine if the copy is necessary
        /// 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Byte"/>
        /// </returns>
        public byte[] ActualBytes()
        {
            if(NeedsCopyForActualBytes)
            {
                var newBytes = new byte[Length];
                Array.Copy(Bytes, Offset, newBytes, 0, Length);
                return newBytes;
            } else
            {
                return Bytes;
            }
        }

        /// <summary>
        /// Return true if we don't need to perform a copy to get
        /// the bytes represented by this class
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public bool NeedsCopyForActualBytes
        {
            get
            {
                return ((Offset == 0) && (Length == Bytes.Length));
            }
        }

        /// <summary>
        /// Helper method that returns the segment immediately following
        /// this instance, useful for processing where the parent
        /// wants to pass the next segment to a sub class for processing
        /// </summary>
        /// <returns>
        /// A <see cref="ByteArrayAndOffset"/>
        /// </returns>
        public ByteArrayAndOffset EncapsulatedBytes()
        {
            int startingOffset = Offset + Length; // start at the end of the current segment
            return new ByteArrayAndOffset(Bytes, startingOffset, Bytes.Length - startingOffset);
        }

        /// <summary>
        /// Format the class information as a string
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[ByteArrayAndOffset: Length={0}, Bytes={1}, Offset={2}, NeedsCopyForActualBytes={3}]", Length, Bytes, Offset, NeedsCopyForActualBytes);
        }
    }
}
