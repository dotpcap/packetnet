using System;
using System.IO;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Contains properties about the received from.
    /// </summary>
    public class RxFlagsRadioTapField : RadioTapField
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        public RxFlagsRadioTapField(BinaryReader br)
        {
            UInt16 flags = br.ReadUInt16();
            this.PlcpCrcCheckFailed = ((flags & 0x2) == 0x2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RxFlagsRadioTapField" /> class.
        /// </summary>
        public RxFlagsRadioTapField()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RxFlagsRadioTapField" /> class.
        /// </summary>
        /// <param name='PlcpCrcCheckFailed'>
        ///     PLCP CRC check failed.
        /// </param>
        public RxFlagsRadioTapField(Boolean PlcpCrcCheckFailed)
        {
            this.PlcpCrcCheckFailed = PlcpCrcCheckFailed;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.RxFlags;

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public override UInt16 Length => 2;

        /// <summary>
        ///     Gets or sets a value indicating whether the frame failed the PLCP CRC check.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the PLCP CRC check failed; otherwise, <c>false</c>.
        /// </value>
        public Boolean PlcpCrcCheckFailed { get; set; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            UInt16 flags = (UInt16) ((this.PlcpCrcCheckFailed) ? 0x2 : 0x0);
            EndianBitConverter.Little.CopyBytes(flags, dest, offset);
        }

        /// <summary>
        ///     ToString() override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public override String ToString()
        {
            return $"PlcpCrcCheckFailed {this.PlcpCrcCheckFailed}";
        }
    }
}