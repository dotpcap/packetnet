using System;
using System.IO;
using PacketDotNet.MiscUtil.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Tsft radio tap field
    /// </summary>
    public class TsftRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.Tsft; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 8; } }
            
        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public UInt64 TimestampUsec { get; set; }
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes(this.TimestampUsec, dest, offset);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public TsftRadioTapField(BinaryReader br)
        {
            this.TimestampUsec = br.ReadUInt64();
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.TsftRadioTapField"/> class.
        /// </summary>
        public TsftRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.TsftRadioTapField"/> class.
        /// </summary>
        /// <param name='TimestampUsec'>
        /// Value in microseconds of the Time Synchronization Function timer
        /// </param>
        public TsftRadioTapField(UInt64 TimestampUsec)
        {
            this.TimestampUsec = TimestampUsec;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TimestampUsec {0}", this.TimestampUsec);
        }
    }
}