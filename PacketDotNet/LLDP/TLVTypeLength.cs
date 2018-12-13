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
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Reflection;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Tlv type and length are 2 bytes
    /// See http://en.wikipedia.org/wiki/Link_Layer_Discovery_Protocol#Frame_structure
    /// </summary>
    [Serializable]
    public class TLVTypeLength
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

        /// <summary>
        /// Length in bytes of the tlv type and length fields
        /// </summary>
        public const Int32 TypeLengthLength = 2;

        private const Int32 TypeMask = 0xFE00;
        private const Int32 LengthBits = 9;
        private const Int32 LengthMask = 0x1FF;

        private const Int32 MaximumTLVLength = 511;

        private readonly ByteArraySegment _byteArraySegment;

        /// <summary>
        /// Construct a TLVTypeLength for a TLV
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public TLVTypeLength(ByteArraySegment byteArraySegment)
        {
            _byteArraySegment = byteArraySegment;
        }

        /// <value>
        /// The TLV Value's Type
        /// </value>
        public TLVTypes Type
        {
            get
            {
                // get the type
                var typeAndLength = TypeAndLength;
                // remove the length info
                return (TLVTypes) (typeAndLength >> LengthBits);
            }

            set
            {
                Log.DebugFormat("value of {0}", value);

                // shift type into the type position
                var type = (UInt16) ((UInt16) value << LengthBits);
                // save the old length
                var length = (UInt16) (LengthMask & TypeAndLength);
                // set the type
                TypeAndLength = (UInt16) (type | length);
            }
        }

        /// <value>
        /// The TLV Value's Length
        /// NOTE: Value is the length of the TLV Value only, does not include the length
        /// of the type and length fields
        /// </value>
        public Int32 Length
        {
            get
            {
                // get the length
                var typeAndLength = TypeAndLength;
                // remove the type info
                return LengthMask & typeAndLength;
            }

            // Length set is internal as the length of a tlv is automatically set based on
            // the tlvs content
            internal set
            {
                Log.DebugFormat("value {0}", value);

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Length must be a positive value");
                }

                if (value > MaximumTLVLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The maximum value for a TLV length is 511");
                }

                // save the old type
                var type = (UInt16) (TypeMask & TypeAndLength);
                // set the length
                TypeAndLength = (UInt16) (type | value);
            }
        }

        /// <value>
        /// A unsigned short representing the concatenated Type and Length
        /// </value>
        private UInt16 TypeAndLength
        {
            get => EndianBitConverter.Big.ToUInt16(_byteArraySegment.Bytes, _byteArraySegment.Offset);

            set => EndianBitConverter.Big.CopyBytes(value, _byteArraySegment.Bytes, _byteArraySegment.Offset);
        }
    }
}