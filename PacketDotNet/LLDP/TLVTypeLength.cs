using System;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Tlv type and length are 2 bytes
    /// See http://en.wikipedia.org/wiki/Link_Layer_Discovery_Protocol#Frame_structure
    /// </summary>
    public class TLVTypeLength
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

        /// <summary>
        /// Length in bytes of the tlv type and length fields
        /// </summary>
        public const int TypeLengthLength = 2;

        private const int TypeBits = 7;
        private const int TypeMask = 0xFE00;
        private const int LengthBits = 9;
        private const int LengthMask = 0x1FF;

        private const int MaximumTLVLength = 511;

        private ByteArraySegment byteArraySegment;

        /// <summary>
        /// Construct a TLVTypeLength for a TLV
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public TLVTypeLength (ByteArraySegment byteArraySegment)
        {
            this.byteArraySegment = byteArraySegment;
        }

        /// <value>
        /// The TLV Value's Type
        /// </value>
        public TLVTypes Type
        {
            get
            {
                // get the type
                ushort typeAndLength = TypeAndLength;
                // remove the length info
                return (TLVTypes)(typeAndLength >> LengthBits);
            }

            set
            {
                log.DebugFormat("value of {0}", value);

                // shift type into the type position
                var type = (ushort)((ushort)value << LengthBits);
                // save the old length
                ushort length = (ushort)(LengthMask & TypeAndLength);
                // set the type
                TypeAndLength = (ushort)(type | length);
            }
        }

        /// <value>
        /// The TLV Value's Length
        /// NOTE: Value is the length of the TLV Value only, does not include the length
        ///       of the type and length fields
        /// </value>
        public int Length
        {
            get
            {
                // get the length
                ushort typeAndLength = TypeAndLength;
                // remove the type info
                return LengthMask & typeAndLength;
            }

            // Length set is internal as the length of a tlv is automatically set based on
            // the tlvs content
            internal set
            {
                log.DebugFormat("value {0}", value);

                if(value < 0) { throw new System.ArgumentOutOfRangeException("Length", "Length must be a positive value"); }
                if(value > MaximumTLVLength) { throw new ArgumentOutOfRangeException("Length", "The maximum value for a TLV length is 511"); }

                // save the old type
                ushort type = (ushort)(TypeMask & TypeAndLength);
                // set the length
                TypeAndLength = (ushort)(type | value);
            }
        }

        /// <value>
        /// A unsigned short representing the concatenated Type and Length
        /// </value>
        private ushort TypeAndLength
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(byteArraySegment.Bytes, byteArraySegment.Offset);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value, byteArraySegment.Bytes, byteArraySegment.Offset);
            }
        }
    }
}
