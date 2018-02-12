using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Transmit power expressed as dBm (decibels from a 1 milliwatt
    /// reference). This is the absolute power level measured at
    /// the antenna port.
    /// </summary>
    public class DbmTxPowerRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmTxPower;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Tx power in dBm
        /// </summary>
        public sbyte TxPowerdBm { get; set; }
   
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte)this.TxPowerdBm;
        }
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmTxPowerRadioTapField(BinaryReader br)
        {
            this.TxPowerdBm = br.ReadSByte();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmTxPowerRadioTapField"/> class.
        /// </summary>
        public DbmTxPowerRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmTxPowerRadioTapField"/> class.
        /// </summary>
        /// <param name='TxPowerdBm'>
        /// Transmit power expressed as dBm (decibels from a 1 milliwatt reference).
        /// </param>
        public DbmTxPowerRadioTapField(sbyte TxPowerdBm)
        {
            this.TxPowerdBm = TxPowerdBm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TxPowerdBm {0}", this.TxPowerdBm);
        }
    }
}