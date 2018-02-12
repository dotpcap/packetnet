using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Fhss radio tap field
    /// </summary>
    public class FhssRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Fhss;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override UInt16 Length => 2;

        /// <summary>
        /// Hop set
        /// </summary>
        public Byte ChannelHoppingSet { get; set; }

        /// <summary>
        /// Hop pattern
        /// </summary>
        public Byte Pattern { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(Byte[] dest, Int32 offset)
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

            this.ChannelHoppingSet = (Byte)(u16 & 0xff);
            this.Pattern = (Byte)((u16 >> 8) & 0xff);
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
        public FhssRadioTapField(Byte ChannelHoppingSet, Byte Pattern)
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
        public override String ToString()
        {
            return $"ChannelHoppingSet {this.ChannelHoppingSet}, Pattern {this.Pattern}";
        }
    }
}