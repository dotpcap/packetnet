using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Db antenna signal
    /// </summary>
    public class DbAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbAntennaSignal;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        /// Signal strength in dB
        /// </summary>
        public Byte SignalStrengthdB { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = this.SignalStrengthdB;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public DbAntennaSignalRadioTapField(BinaryReader br)
        {
            this.SignalStrengthdB = br.ReadByte();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbAntennaSignalRadioTapField"/> class.
        /// </summary>
        public DbAntennaSignalRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbAntennaSignalRadioTapField"/> class.
        /// </summary>
        /// <param name='SignalStrengthdB'>
        /// Signal strength in dB
        /// </param>
        public DbAntennaSignalRadioTapField (Byte SignalStrengthdB)
        {
            this.SignalStrengthdB = SignalStrengthdB;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override String ToString()
        {
            return $"SignalStrengthdB {this.SignalStrengthdB}";
        }
    }
}