using System;
using System.IO;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Contains properties about the received from.
    /// </summary>
    public class RxFlagsRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.RxFlags; } }
            
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 2; } }
            
        /// <summary>
        /// Gets or sets a value indicating whether the frame failed the PLCP CRC check.
        /// </summary>
        /// <value>
        /// <c>true</c> if the PLCP CRC check failed; otherwise, <c>false</c>.
        /// </value>
        public bool PlcpCrcCheckFailed {get; set;}
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            UInt16 flags = (UInt16)((this.PlcpCrcCheckFailed) ? 0x2 : 0x0);
            EndianBitConverter.Little.CopyBytes(flags, dest, offset);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public RxFlagsRadioTapField(BinaryReader br)
        {
            UInt16 flags = br.ReadUInt16();
            this.PlcpCrcCheckFailed = ((flags & 0x2) == 0x2);
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RxFlagsRadioTapField"/> class.
        /// </summary>
        public RxFlagsRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RxFlagsRadioTapField"/> class.
        /// </summary>
        /// <param name='PlcpCrcCheckFailed'>
        /// PLCP CRC check failed.
        /// </param>
        public RxFlagsRadioTapField(bool PlcpCrcCheckFailed)
        {
            this.PlcpCrcCheckFailed = PlcpCrcCheckFailed;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("PlcpCrcCheckFailed {0}", this.PlcpCrcCheckFailed);
        }
    }
}