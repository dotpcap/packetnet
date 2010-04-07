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
    /// <summary>
    /// Container class that refers to a segment of bytes in a byte[]
    /// </summary>
    public class ByteArraySegment
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        private int length;

        /// <value>
        /// Number of bytes beyond the offset into Bytes
        /// </value>
        public int Length
        {
            get { return length; }
            internal set
            {
                // check for invalid values
                if(value < 0)
                    throw new System.InvalidOperationException("attempting to set a negative length of " + value);

                length = value;
                log.DebugFormat("Length: {0}", value);
            }
        }

        /// <value>
        /// The byte[] array
        /// </value>
        public byte[] Bytes { get; private set; }

        /// <value>
        /// Offset into Bytes
        /// </value>
        public int Offset { get; private set; }

        /// <summary>
        /// Constructor from a byte array, offset into the byte array and
        /// a length beyond that offset of the bytes this class is referencing
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public ByteArraySegment(byte[] Bytes, int Offset, int Length)
        {
            log.DebugFormat("Bytes.Length {0}, Offset {1}, Length {2}",
                            Bytes.Length,
                            Offset,
                            Length);

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
            log.DebugFormat("{0}", ToString());

            if(NeedsCopyForActualBytes)
            {
                log.Debug("needs copy");
                var newBytes = new byte[Length];
                Array.Copy(Bytes, Offset, newBytes, 0, Length);
                return newBytes;
            } else
            {
                log.Debug("does not need copy");
                return Bytes;
            }
        }

        /// <summary>
        /// Return true if we need to perform a copy to get
        /// the bytes represented by this class
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public bool NeedsCopyForActualBytes
        {
            get
            {
                // we need a copy unless we are at the start of the byte[]
                // and the length is the total byte[] length
                var okWithoutCopy = ((Offset == 0) && (Length == Bytes.Length));
                var retval = !okWithoutCopy;

                log.DebugFormat("retval {0}", retval);

                return retval;
            }
        }

        /// <summary>
        /// Helper method that returns the segment immediately following
        /// this instance, useful for processing where the parent
        /// wants to pass the next segment to a sub class for processing
        /// </summary>
        /// <returns>
        /// A <see cref="ByteArraySegment"/>
        /// </returns>
        public ByteArraySegment EncapsulatedBytes()
        {
            int startingOffset = Offset + Length; // start at the end of the current segment
            var newLength = Bytes.Length - startingOffset;
            log.DebugFormat("Offset {0}, Length {1}, startingOffset {2}, newLength {3}",
                            Offset, Length, startingOffset, newLength);
            return new ByteArraySegment(Bytes, startingOffset, newLength);
        }

        /// <summary>
        /// Format the class information as a string
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[ByteArraySegment: Length={0}, Bytes.Length={1}, Offset={2}, NeedsCopyForActualBytes={3}]",
                                 Length, Bytes.Length, Offset, NeedsCopyForActualBytes);
        }
    }
}
