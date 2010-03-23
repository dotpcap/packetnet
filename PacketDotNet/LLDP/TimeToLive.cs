using System;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Time to Live TLV
    /// </summary>
    public class TimeToLive : TLV
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
        /// Number of bytes in the value portion of this tlv
        /// </summary>
        private const int ValueLength = 2;

        #region Constructors

        /// <summary>
        /// Creates a TTL TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The TTL TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public TimeToLive(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            log.Debug("");
        }

        /// <summary>
        /// Creates a TTL TLV and sets it value
        /// </summary>
        /// <param name="seconds">
        /// The length in seconds until the LLDP
        /// is refreshed
        /// </param>
        public TimeToLive(ushort seconds)
        {
            log.Debug("");

            var bytes = new byte[TLVTypeLength.TypeLengthLength + ValueLength];
            int offset = 0;
            int length = bytes.Length;
            tlvData = new ByteArraySegment(bytes, offset, length);

            Type = TLVTypes.TimeToLive;
            Seconds = seconds;
        }

        #endregion

        #region Properties

        /// <value>
        /// The number of seconds until the LLDP needs
        /// to be refreshed
        ///
        /// A value of 0 means that the LLDP source is
        /// closed and should no longer be refreshed
        /// </value>
        public ushort Seconds
        {
            get
            {
                // get the seconds
                return BigEndianBitConverter.Big.ToUInt16(tlvData.Bytes,
                                                          tlvData.Offset + TLVTypeLength.TypeLengthLength);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 tlvData.Bytes,
                                                 tlvData.Offset + TLVTypeLength.TypeLengthLength);
            }
        }

        /// <summary>
        /// Convert this TTL TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[TimeToLive: Seconds={0}]", Seconds);
        }

        #endregion
    }
}