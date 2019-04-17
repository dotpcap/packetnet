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

#if DEBUG
using log4net;
#endif

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
        /// Gets the byte array.
        /// </value>
        public Byte[] Bytes { get; }

        /// <value>
        /// Gets the maximum number of bytes we should treat <see cref="Bytes"/> as having.
        /// This allows for controlling the number of bytes produced by <see cref="EncapsulatedBytes()"/>.
        /// </value>
        public Int32 BytesLength { get; }

        /// <value>
        /// Gets or sets the number of bytes beyond the offset into <see cref="Bytes"/>. 
        /// </value>
        /// <remarks>Take care when setting this parameter as many things are based on the value of this property being correct.</remarks>
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
        /// Gets the offset into <see cref="Bytes"/>.
        /// </value>
        public Int32 Offset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArraySegment"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public ByteArraySegment(Byte[] bytes) :
            this(bytes, 0, bytes.Length)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArraySegment"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset into the byte array.</param>
        /// <param name="length">The length beyond the offset.</param>
        public ByteArraySegment(Byte[] bytes, Int32 offset, Int32 length)
            : this(bytes, offset, length, bytes.Length)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArraySegment"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset into the byte array.</param>
        /// <param name="length">The length beyond the offset.</param>
        /// <param name="bytesLength">Length of the bytes.</param>
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
        /// Initializes a new instance of the <see cref="ByteArraySegment"/> class.
        /// </summary>
        /// <param name="byteArraySegment">The original byte array segment.</param>
        public ByteArraySegment(ByteArraySegment byteArraySegment)
        {
            Bytes = byteArraySegment.Bytes;
            Offset = byteArraySegment.Offset;
            Length = byteArraySegment.Length;
            BytesLength = byteArraySegment.BytesLength;
        }

        /// <summary>
        /// Returns a contiguous byte array from this instance, if necessary, by copying the bytes from the current offset into a newly allocated byte array.
        /// <see cref="NeedsCopyForActualBytes" /> can be used to determine if the copy is necessary.
        /// </summary>
        /// <returns>A <see cref="System.Byte" /></returns>
        public Byte[] ActualBytes()
        {
            Log.DebugFormat("{0}", ToString());

            if (NeedsCopyForActualBytes)
            {
                Log.Debug("needs copy");
                var bytes = new Byte[Length];
                Array.Copy(Bytes, Offset, bytes, 0, Length);
                return bytes;
            }

            Log.Debug("does not need copy");
            return Bytes;
        }

        /// <summary>
        /// Gets a value indicating whether we need to perform a copy to get the bytes represented by this instance.
        /// </summary>
        public Boolean NeedsCopyForActualBytes
        {
            get
            {
                // we need a copy unless we are at the start of the byte[]
                // and the length is the total byte[] length
                var okWithoutCopy = Offset == 0 && Length == Bytes.Length;
                var result = !okWithoutCopy;

                Log.DebugFormat("result {0}", result);

                return result;
            }
        }

        /// <summary>
        /// Returns the segment after this instance.
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
        /// Returns the segment after this instance.
        /// </summary>
        /// <param name="segmentLength">A <see cref="System.Int32" /> that can be used to limit the length of the segment that is to be returned.</param>
        /// <returns>A <see cref="ByteArraySegment" /></returns>
        public ByteArraySegment EncapsulatedBytes(Int32 segmentLength)
        {
            Log.DebugFormat("SegmentLength {0}", segmentLength);

            var startingOffset = Offset + Length; // start at the end of the current segment
            Log.DebugFormat("StartingOffset({0}) = Offset({1}) + Length({2})", startingOffset, Offset, Length);

            // ensure that the new segment length isn't longer than the number of bytes
            // available after the current segment
            segmentLength = Math.Min(segmentLength, BytesLength - startingOffset);

            // calculate the ByteLength property of the new ByteArraySegment
            var bytesLength = startingOffset + segmentLength;

            Log.DebugFormat("SegmentLength {0}, BytesLength {1}", segmentLength, bytesLength);

            return new ByteArraySegment(Bytes, startingOffset, segmentLength, bytesLength);
        }

        /// <inheritdoc />
        public override String ToString()
        {
            return $"[ByteArraySegment: Length={Length}, Bytes.Length={Bytes.Length}, BytesLength={BytesLength}, Offset={Offset}, NeedsCopyForActualBytes={NeedsCopyForActualBytes}]";
        }
    }
}