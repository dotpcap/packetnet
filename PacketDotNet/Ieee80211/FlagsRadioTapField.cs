using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Radio tap flags
    /// </summary>
    public class FlagsRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType { get { return RadioTapType.Flags; } }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length { get { return 1; } }
            
        /// <summary>
        /// Flags set
        /// </summary>
        public RadioTapFlags Flags;
            
        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            dest[offset] = (byte) this.Flags;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public FlagsRadioTapField(BinaryReader br)
        {
            var u8 = br.ReadByte();
            this.Flags = (RadioTapFlags)u8;
        }
   
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.FlagsRadioTapField"/> class.
        /// </summary>
        public FlagsRadioTapField()
        {
             
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.FlagsRadioTapField"/> class.
        /// </summary>
        /// <param name='Flags'>
        /// Flags.
        /// </param>
        public FlagsRadioTapField(RadioTapFlags Flags)
        {
            this.Flags = Flags;
        }
            
        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("Flags {0}", this.Flags);
        }
    }
}