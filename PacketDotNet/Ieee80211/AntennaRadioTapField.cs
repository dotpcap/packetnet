using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Antenna field
    /// </summary>
    public class AntennaRadioTapField : RadioTapField
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        public AntennaRadioTapField(BinaryReader br)
        {
            this.Antenna = br.ReadByte();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AntennaRadioTapField" /> class.
        /// </summary>
        public AntennaRadioTapField()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AntennaRadioTapField" /> class.
        /// </summary>
        /// <param name='Antenna'>
        ///     Antenna index of the Rx/Tx antenna for this packet. The first antenna is antenna 0.
        /// </param>
        public AntennaRadioTapField(Byte Antenna)
        {
            this.Antenna = Antenna;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Antenna;

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        ///     Antenna number
        /// </summary>
        public Byte Antenna { get; set; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = this.Antenna;
        }

        /// <summary>
        ///     ToString() override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public override String ToString()
        {
            return $"Antenna {this.Antenna}";
        }
    }
}