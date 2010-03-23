using System;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Base class for several TLV types that all contain strings
    /// </summary>
    public class StringTLV : TLV
    {
        #region Constructors

        /// <summary>
        /// Creates a String TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public StringTLV(byte[] bytes, int offset) :
            base(bytes, offset)
        {}

        /// <summary>
        /// Create from a type and string value
        /// </summary>
        /// <param name="tlvType">
        /// A <see cref="TLVTypes"/>
        /// </param>
        /// <param name="StringValue">
        /// A <see cref="System.String"/>
        /// </param>
        public StringTLV(TLVTypes tlvType, string StringValue)
        {
            var bytes = new byte[TLVTypeLength.TypeLengthLength];
            var offset = 0;
            tlvData = new ByteArraySegment(bytes, offset, bytes.Length);

            Type = tlvType;
            this.StringValue = StringValue;
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Description of the port
        /// </value>
        public string StringValue
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(tlvData.Bytes,
                                                                 ValueOffset,
                                                                 Length);
            }

            set
            {
                var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                var length = TLVTypeLength.TypeLengthLength + bytes.Length;

                // is the tlv the correct size?
                if(tlvData.Length != length)
                {
                    // allocate new memory for this tlv
                    var newTLVBytes = new byte[length];
                    var offset = 0;

                    // copy header over
                    Array.Copy(tlvData.Bytes, tlvData.Offset,
                               newTLVBytes, 0,
                               TLVTypeLength.TypeLengthLength);

                    tlvData = new ByteArraySegment(newTLVBytes, offset, length);
                }

                // set the description
                Array.Copy(bytes, 0,
                           tlvData.Bytes, ValueOffset,
                           bytes.Length);
            }
        }

        /// <summary>
        /// Convert this Port Description TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[{0}: Description={0}]", Type, StringValue);
        }
        
        #endregion
    }
}
