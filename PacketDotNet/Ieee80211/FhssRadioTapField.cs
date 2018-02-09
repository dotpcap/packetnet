using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Fhss radio tap field
    /// </summary>
    public class FhssRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.Fhss; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 2; } }
            
        /// <summary>
        /// Hop set
        /// </summary>
        public byte ChannelHoppingSet { get; set; }

        /// <summary>
        /// Hop pattern
        /// </summary>
        public byte Pattern { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = this.ChannelHoppingSet;
            dest[offset + 1] = this.Pattern;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public FhssRadioTapField(BinaryReader br)
        {
            var u16 = br.ReadUInt16();

            this.ChannelHoppingSet = (byte)(u16 & 0xff);
            this.Pattern = (byte)((u16 >> 8) & 0xff);
        }
   
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.FhssRadioTapField"/> class.
        /// </summary>
        public FhssRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.FhssRadioTapField"/> class.
        /// </summary>
        /// <param name='ChannelHoppingSet'>
        /// Channel hopping set.
        /// </param>
        /// <param name='Pattern'>
        /// Channel hopping pattern.
        /// </param>
        public FhssRadioTapField(byte ChannelHoppingSet, byte Pattern)
        {
            this.ChannelHoppingSet = ChannelHoppingSet;
            this.Pattern = Pattern;
        }
            
        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("ChannelHoppingSet {0}, Pattern {1}",
                this.ChannelHoppingSet, this.Pattern);
        }
    }
}