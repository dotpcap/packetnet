using System;
using System.IO;
using PacketDotNet.Utils.Conversion;

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
        public override RadioTapType FieldType => RadioTapType.TxAttenuation;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 2;

        /// <summary>
        /// Transmit power
        /// </summary>
        public Int32 TxPower { get; set; }
   
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
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
            this.TxPower = -(Int32)br.ReadUInt16();
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
        public TxAttenuationRadioTapField (Int32 TxPower)
        {
            this.TxPower = TxPower;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override String ToString()
        {
            return $"TxPower {this.TxPower}";
        }
    }
}