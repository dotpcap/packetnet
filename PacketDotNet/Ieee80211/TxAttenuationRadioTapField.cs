using System;
using System.IO;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Transmit power expressed as unitless distance from max
    /// power set at factory calibration.  0 is max power.
    /// Monotonically nondecreasing with lower power levels.
    /// </summary>
    public class TxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.TxAttenuation; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 2; } }
            
        /// <summary>
        /// Transmit power
        /// </summary>
        public int TxPower { get; set; }
   
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            UInt16 absValue = (UInt16) Math.Abs(this.TxPower);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public TxAttenuationRadioTapField(BinaryReader br)
        {
            this.TxPower = -(int)br.ReadUInt16();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.TxAttenuationRadioTapField"/> class.
        /// </summary>
        public TxAttenuationRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.TxAttenuationRadioTapField"/> class.
        /// </summary>
        /// <param name='TxPower'>
        /// Transmit power expressed as unitless distance from max power set at factory calibration. 0 is max power.
        /// </param>
        public TxAttenuationRadioTapField (int TxPower)
        {
            this.TxPower = TxPower;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TxPower {0}", this.TxPower);
        }
    }
}