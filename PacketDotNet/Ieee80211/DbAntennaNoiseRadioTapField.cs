using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Antenna noise in dB
    /// </summary>
    public class DbAntennaNoiseRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.DbAntennaNoise; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 1; } }
            
        /// <summary>
        /// Antenna noise in dB
        /// </summary>
        public byte AntennaNoisedB { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = this.AntennaNoisedB;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbAntennaNoiseRadioTapField(BinaryReader br)
        {
            this.AntennaNoisedB = br.ReadByte();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbAntennaNoiseRadioTapField"/> class.
        /// </summary>
        public DbAntennaNoiseRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbAntennaNoiseRadioTapField"/> class.
        /// </summary>
        /// <param name='AntennaNoisedB'>
        /// Antenna signal noise in dB.
        /// </param>
        public DbAntennaNoiseRadioTapField(byte AntennaNoisedB)
        {
            this.AntennaNoisedB = AntennaNoisedB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaNoisedB {0}", this.AntennaNoisedB);
        }
    }
}