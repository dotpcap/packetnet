using System;
using System.IO;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Transmit power expressed as decibel distance from max power
    ///     set at factory calibration.  0 is max power.  Monotonically
    ///     nondecreasing with lower power levels.
    /// </summary>
    public class DbTxAttenuationRadioTapField : RadioTapField
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        public DbTxAttenuationRadioTapField(BinaryReader br)
        {
            this.TxPowerdB = -br.ReadUInt16();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbTxAttenuationRadioTapField" /> class.
        /// </summary>
        public DbTxAttenuationRadioTapField()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbTxAttenuationRadioTapField" /> class.
        /// </summary>
        /// <param name='TxPowerdB'>
        ///     Transmit power expressed as decibel distance from max power set at factory calibration. 0 is max power.
        /// </param>
        public DbTxAttenuationRadioTapField(Int32 TxPowerdB)
        {
            this.TxPowerdB = TxPowerdB;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbTxAttenuation;

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public override UInt16 Length => 2;

        /// <summary>
        ///     Transmit power
        /// </summary>
        public Int32 TxPowerdB { get; set; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            UInt16 absValue = (UInt16) Math.Abs(this.TxPowerdB);
            EndianBitConverter.Little.CopyBytes(absValue, dest, offset);
        }

        /// <summary>
        ///     ToString() override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public override String ToString()
        {
            return $"TxPowerdB {this.TxPowerdB}";
        }
    }
}