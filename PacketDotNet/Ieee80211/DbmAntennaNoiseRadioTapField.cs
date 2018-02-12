using System;
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
        public override UInt16 Length => 1;

        /// <summary>
        /// Antenna noise in dBm
        /// </summary>
        public SByte AntennaNoisedBm { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte)this.AntennaNoisedBm;
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
        public DbmAntennaNoiseRadioTapField (SByte AntennaNoisedBm)
        {
            this.AntennaNoisedBm = AntennaNoisedBm;
        }
            
        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override String ToString()
        {
            return $"AntennaNoisedBm {this.AntennaNoisedBm}";
        }
    }
}