using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Rate field
    /// </summary>
    public class RateRadioTapField : RadioTapField
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        public RateRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            this.RateMbps = (0.5 * (u8 & 0x7f));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RateRadioTapField" /> class.
        /// </summary>
        public RateRadioTapField()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RateRadioTapField" /> class.
        /// </summary>
        /// <param name='rateMbps'>
        ///     Rate mbps.
        /// </param>
        public RateRadioTapField(Double rateMbps)
        {
            this.RateMbps = rateMbps;
        }

        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Rate;

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public override UInt16 Length => 1;

        /// <summary>
        ///     Rate in Mbps
        /// </summary>
        public Double RateMbps { get; set; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
        {
            dest[offset] = (Byte) (this.RateMbps / 0.5);
        }

        /// <summary>
        ///     ToString() override
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" />
        /// </returns>
        public override String ToString()
        {
            return $"RateMbps {this.RateMbps}";
        }
    }
}