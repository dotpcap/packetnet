namespace PacketDotNet.LLDP
{
    /// <summary>
    /// An End Of LLDPDU TLV
    /// </summary>
    public class EndOfLLDPDU : TLV
    {
        #region Constructors

        /// <summary>
        /// Parses bytes into an End Of LLDPDU TLV
        /// </summary>
        /// <param name="bytes">
        /// TLV bytes
        /// </param>
        /// <param name="offset">
        /// The End Of LLDPDU TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public EndOfLLDPDU(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Creates an End Of LLDPDU TLV
        /// </summary>
        public EndOfLLDPDU()
        {
            var bytes = new byte[TLVTypeLength.TypeLengthLength];
            var offset = 0;
            var length = bytes.Length;
            tlvData = new PacketDotNet.Utils.ByteArraySegment(bytes, offset, length);

            Type = 0;
            Length = 0;
        }

        /// <summary>
        /// Convert this TTL TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[EndOfLLDPDU]");
        }
        
        #endregion
    }
}