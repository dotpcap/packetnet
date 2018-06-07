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
using System.Reflection;
using log4net;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Container class that refers to a segment of bytes in a byte[]
    /// Used to ensure high performance by allowing memory copies to
    /// be avoided
    /// </summary>
    [Serializable]
    public class ByteArraySegment
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        private Int32 _length;

        /// <value>
        /// The byte[] array
        /// </value>
        public Byte[] Bytes { get; }

        /// <value>
        /// The maximum number of bytes we should treat Bytes as having, allows
        /// for controling the number of bytes produced by EncapsulatedBytes()
        /// </value>
        public Int32 BytesLength { get; }

        /// <value>
        /// Number of bytes beyond the offset into Bytes
        /// Take care when setting this parameter as many things are based on
        /// the value of this property being correct
        /// </value>
        public Int32 Length
        {
            get => _length;
            set
            {
                // check for invalid values
                if (value < 0)
                {
                    Log.DebugFormat("Attempting to set a negative length of {0}, setting to 0.", value);
                    value = 0;
                }

                _length = value;
                Log.DebugFormat("Length: {0}", value);
            }
        }

        /// <value>
        /// Offset into Bytes
        /// </value>
        public Int32 Offset { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        public ByteArraySegment(Byte[] bytes) :
            this(bytes, 0, bytes.Length)
        { }

        /// <summary>
        /// Constructor from a byte array, offset into the byte array and
        /// a length beyond that offset of the bytes this class is referencing
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public ByteArraySegment(Byte[] bytes, Int32 offset, Int32 length)
            : this(bytes, offset, length, bytes.Length)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="bytesLength">
        /// A <see cref="System.Int32" />
        /// </param>
        public ByteArraySegment(Byte[] bytes, Int32 offset, Int32 length, Int32 bytesLength)
        {
            Log.DebugFormat("Bytes.Length {0}, Offset {1}, Length {2}, BytesLength {3}",
                            bytes.Length,
                            offset,
                            length,
                            bytesLength);

            Bytes = bytes;
            Offset = offset;
            Length = length;
            BytesLength = Math.Min(bytesLength, bytes.Length);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="original">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ByteArraySegment(ByteArraySegment original)
        {
            Bytes = original.Bytes;
            Offset = original.Offset;
            Length = original.Length;
            BytesLength = original.BytesLength;
        }

        /// <summary>
        /// Returns a contiguous byte[] from this container, if necessary, by copying
        /// the bytes from the current offset into a newly allocated byte[].
        /// NeedsCopyForActualBytes can be used to determine if the copy is necessary
        /// </summary>
        /// <returns>
        /// A <see cref="System.Byte" />
        /// </returns>
        public Byte[] ActualBytes()
        {
            Log.DebugFormat("{0}", ToString());

            if (NeedsCopyForActualBytes)
            {
                Log.Debug("needs copy");
                var newBytes = new Byte[Length];
                Array.Copy(Bytes, Offset, newBytes, 0, Length);
                return newBytes;
            }

            Log.Debug("does not need copy");
            return Bytes;
        }

        /// <summary>
        /// Return true if we need to perform a copy to get
        /// the bytes represented by this class
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean" />
        /// </returns>
        public Boolean NeedsCopyForActualBytes
        {
            get
            {
                // we need a copy unless we are at the start of the byte[]
                // and the length is the total byte[] length
                var okWithoutCopy = Offset == 0 && Length == Bytes.Length;
                var retval = !okWithoutCopy;

                Log.DebugFormat("retval {0}", retval);

                return retval;
            }
        }

        /// <summary>
        /// Helper method that returns the segment immediately following
        /// this instance, useful for processing where the parent
        /// wants to pass the next segment to a sub class for processing
        /// </summary>
        /// <returns>
        /// A <see cref="ByteArraySegment" />
        /// </returns>
        public ByteArraySegment EncapsulatedBytes()
        {
            var numberOfBytesAfterThisSegment = BytesLength - (Offset + Length);
            return EncapsulatedBytes(numberOfBytesAfterThisSegment);
        }

        /// <summary>
        /// Create the segment after the current one
        /// </summary>
        /// <param name="newSegmentLength">
        /// A <see cref="System.Int32" /> that can be used to limit the segment length
        /// of the ByteArraySegment that is to be returned. Often used to exclude trailing bytes.
        /// </param>
        /// <returns>
        /// A <see cref="ByteArraySegment" />
        /// </returns>
        public ByteArraySegment EncapsulatedBytes(Int32 newSegmentLength)
        {
            Log.DebugFormat("NewSegmentLength {0}", newSegmentLength);

            var startingOffset = Offset + Length; // start at the end of the current segment
            Log.DebugFormat("startingOffset({0}) = Offset({1}) + Length({2})",
                            startingOffset,
                            Offset,
                            Length);

            // ensure that the new segment length isn't longer than the number of bytes
            // available after the current segment
            newSegmentLength = Math.Min(newSegmentLength, BytesLength - startingOffset);

            // calculate the ByteLength property of the new ByteArraySegment
            var newByteLength = startingOffset + newSegmentLength;

            Log.DebugFormat("NewSegmentLength {0}, NewByteLength {1}, BytesLength {2}",
                            newSegmentLength,
                            newByteLength,
                            BytesLength);

            return new ByteArraySegment(Bytes, startingOffset, newSegmentLength, newByteLength);
        }

        /// <summary>
        /// Format the class information as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override String ToString()
        {
            return $"[ByteArraySegment: Length={Length}, Bytes.Length={Bytes.Length}, BytesLength={BytesLength}, Offset={Offset}, NeedsCopyForActualBytes={NeedsCopyForActualBytes}]";
        }
    }
}