using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Antenna signal in dBm
    /// </summary>
    public class DbmAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.DbmAntennaSignal; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 1; } }
            
        /// <summary>
        /// Antenna signal in dBm
        /// </summary>
        public sbyte AntennaSignalDbm { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte)this.AntennaSignalDbm;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbmAntennaSignalRadioTapField(BinaryReader br)
        {
            this.AntennaSignalDbm = br.ReadSByte();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaSignalRadioTapField"/> class.
        /// </summary>
        public DbmAntennaSignalRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaSignalRadioTapField"/> class.
        /// </summary>
        /// <param name='AntennaSignalDbm'>
        /// Antenna signal power in dB.
        /// </param>
        public DbmAntennaSignalRadioTapField (sbyte AntennaSignalDbm)
        {
            this.AntennaSignalDbm = AntennaSignalDbm;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("AntennaSignalDbm {0}", this.AntennaSignalDbm);
        }
    }
}