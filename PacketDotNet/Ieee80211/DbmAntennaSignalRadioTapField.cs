using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Antenna signal in dBm
    /// </summary>
    public class DbmAntennaSignalRadioTapField : RadioTapField
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        public DbmAntennaSignalRadioTapField(BinaryReader br)
        {
            this.AntennaSignalDbm = br.ReadSByte();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaSignalRadioTapField" /> class.
        /// </summary>
        public DbmAntennaSignalRadioTapField()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DbmAntennaSignalRadioTapField" /> class.
        /// </summary>
        /// <param name='antennaSignalDbm'>
        ///     Antenna signal power in dB.
        /// </param>
        public DbmAntennaSignalRadioTapField(SByte antennaSignalDbm)
        {
            this.AntennaSignalDbm = antennaSignalDbm;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.DbmAntennaSignal;

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        ///     Antenna signal in dBm
        /// </summary>
        public SByte AntennaSignalDbm { get; set; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) this.AntennaSignalDbm;
        }

        /// <summary>
        ///     ToString() override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public override String ToString()
        {
            return $"AntennaSignalDbm {this.AntennaSignalDbm}";
        }
    }
}