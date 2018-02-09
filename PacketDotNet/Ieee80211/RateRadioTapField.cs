using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Rate field
    /// </summary>
    public class RateRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.Rate; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 1; } }
            
        /// <summary>
        /// Rate in Mbps
        /// </summary>
        public double RateMbps { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) (this.RateMbps / 0.5);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public RateRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            this.RateMbps = (0.5 * (u8 & 0x7f));
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RateRadioTapField"/> class.
        /// </summary>
        public RateRadioTapField ()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RateRadioTapField"/> class.
        /// </summary>
        /// <param name='RateMbps'>
        /// Rate mbps.
        /// </param>
        public RateRadioTapField(double RateMbps)
        {
            this.RateMbps = RateMbps;             
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("RateMbps {0}", this.RateMbps);
        }
    }
}