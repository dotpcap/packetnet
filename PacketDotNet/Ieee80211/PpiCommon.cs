using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The PPI Common field contains fields common to all 802.11 specifications.
    /// This field is loosely based on the Radio Tap header format.
    /// </summary>
    public class PpiCommon : PpiField
    {
        /// <summary>
        /// Common 802.11 flags.
        /// </summary>
        [Flags]
        public enum CommonFlags : ushort
        {
            /// <summary>
            /// Defines whether or not an FCS is included at the end of the encapsulated 802.11 frame.
            /// </summary>
            FcsIncludedInFrame = 0x1,
            /// <summary>
            /// If set the TSF-timer is in milliseconds, if not set the TSF-timer is in microseconds
            /// </summary>
            TimerSynchFunctionInUse = 0x2,
            /// <summary>
            /// Indicates that the FCS on the encapsulated 802.11 frame is invalid
            /// </summary>
            FailedFcsCheck = 0x4,
            /// <summary>
            /// Indicates that there was some type of physical error when receiving the packet.
            /// </summary>
            PhysicalError = 0x8
        }
         
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType
        {
            get { return PpiFieldType.PpiCommon;}
        }
            
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override int Length { get { return 20; } }
            
        /// <summary>
        /// Radiotap-formatted channel flags.
        /// </summary>
        public RadioTapChannelFlags ChannelFlags { get; set; }
            
        /// <summary>
        /// Radiotap-formatted channel frequency, in MHz. 0 indicates an invalid value.
        /// </summary>
        /// <value>
        /// The channel frequency.
        /// </value>
        public UInt16 ChannelFrequency { get; set; }
            
        /// <summary>
        /// The common flags.
        /// </summary>
        public CommonFlags Flags { get; set; }
   
        /// <summary>
        /// Data rate in multiples of 500 Kbps. 0 indicates an invalid value.
        /// </summary>
        /// <value>
        /// The data rate.
        /// </value>
        public double Rate { get; set; } 
   
        /// <summary>
        /// Gets or sets the TSF timer.
        /// </summary>
        /// <value>
        /// The TSF Timer value.
        /// </value>
        public UInt64 TSFTimer { get; set; }
            
        /// <summary>
        /// Gets or sets the Frequency-hopping spread spectrum (FHSS) hopset
        /// </summary>
        /// <value>
        /// The FHSS hopset.
        /// </value>
        public Byte FhssHopset { get; set; }
            
        /// <summary>
        /// Gets or sets the Frequency-hopping spread spectrum (FHSS) pattern.
        /// </summary>
        /// <value>
        /// The FHSS pattern.
        /// </value>
        public Byte FhssPattern { get; set; }
   
        /// <summary>
        /// Gets or sets the RF signal power at antenna.
        /// </summary>
        /// <value>
        /// The antenna signal power.
        /// </value>
        public SByte AntennaSignalPower { get; set; }
            
        /// <summary>
        /// Gets or sets the RF signal noise at antenna
        /// </summary>
        /// <value>
        /// The antenna signal noise.
        /// </value>
        public SByte AntennaSignalNoise
        {
            get;
            set;
        }
            
        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public override byte[] Bytes
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                    
                writer.Write(this.TSFTimer);
                writer.Write((ushort)this.Flags);
                writer.Write((ushort)(this.Rate * 2));
                writer.Write(this.ChannelFrequency);
                writer.Write((ushort)this.ChannelFlags);
                writer.Write(this.FhssHopset);
                writer.Write(this.FhssPattern);
                writer.Write(this.AntennaSignalPower);
                writer.Write(this.AntennaSignalNoise);
                    
                return ms.ToArray();
            }
        }
            
        #endregion Properties

        #region Constructors
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiCommon"/> class from the 
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiCommon (BinaryReader br)
        {
            this.TSFTimer = br.ReadUInt64 ();
            this.Flags = (CommonFlags)br.ReadUInt16 ();
            this.Rate = 0.5f * br.ReadUInt16 ();
            this.ChannelFrequency = br.ReadUInt16 ();
            this.ChannelFlags = (RadioTapChannelFlags) br.ReadUInt16 ();
            this.FhssHopset = br.ReadByte ();
            this.FhssPattern = br.ReadByte ();
            this.AntennaSignalPower = br.ReadSByte();
            this.AntennaSignalNoise = br.ReadSByte();
        }
   
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiCommon"/> class.
        /// AntennaSignalPower and AntennaSignalNoise are both set to their minimum value of -128.
        /// </summary>
        public PpiCommon ()
        {
            this.AntennaSignalPower = -128;
            this.AntennaSignalNoise = -128;
        }
            
        #endregion Constructors
    }
}