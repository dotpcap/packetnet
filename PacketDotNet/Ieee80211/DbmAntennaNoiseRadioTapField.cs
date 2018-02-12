using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Antenna noise in dBm
    /// </summary>
    public class DbmAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaNoise;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 1;

        /// <summary>
        /// Antenna noise in dBm
        /// </summary>
        public sbyte AntennaNoisedBm { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte)this.AntennaNoisedBm;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmAntennaNoiseRadioTapField(BinaryReader br)
        {
            this.AntennaNoisedBm = br.ReadSByte();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaNoiseRadioTapField"/> class.
        /// </summary>
        public DbmAntennaNoiseRadioTapField()
        {
             
        }
   
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaNoiseRadioTapField"/> class.
        /// </summary>
        /// <param name='AntennaNoisedBm'>
        /// Antenna noise in dBm.
        /// </param>
        public DbmAntennaNoiseRadioTapField (sbyte AntennaNoisedBm)
        {
            this.AntennaNoisedBm = AntennaNoisedBm;
        }
            
        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaNoisedBm {0}", this.AntennaNoisedBm);
        }
    }
}