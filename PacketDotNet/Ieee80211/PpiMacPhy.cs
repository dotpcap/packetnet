using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The 802.11n MAC + PHY Extension field contains radio information specific to 802.11n.
    /// </summary>
    public class PpiMacPhy : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.PpiMacPhy;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => 48;

        /// <summary>
        /// Gets or sets the 802.11n MAC extension flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public PpiMacExtensionFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the A-MPDU identifier.
        /// </summary>
        /// <value>
        /// the A-MPDU id.
        /// </value>
        public UInt32 AMpduId { get; set; }

        /// <summary>
        /// Gets or sets the number of zero-length pad delimiters
        /// </summary>
        /// <value>
        /// The delimiter count.
        /// </value>
        public Byte DelimiterCount { get; set; }

        /// <summary>
        /// Gets or sets the modulation coding scheme.
        /// </summary>
        /// <value>
        /// The modulation coding scheme.
        /// </value>
        public Byte ModulationCodingScheme { get; set; }

        /// <summary>
        /// Gets or sets the number of spatial streams.
        /// </summary>
        /// <value>
        /// The spatial stream count.
        /// </value>
        public Byte SpatialStreamCount { get; set; }

        /// <summary>
        /// Gets or sets the combined Received Signal Strength Indication (RSSI) value
        /// from all the active antennas and channels.
        /// </summary>
        /// <value>
        /// The combined RSSI.
        /// </value>
        public Byte RssiCombined { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 0, control channel.
        /// </summary>
        /// <value>
        /// The antenna 0 RSSI value.
        /// </value>
        public Byte RssiAntenna0Control { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 1, control channel.
        /// </summary>
        /// <value>
        /// The antenna 1 control channel RSSI value.
        /// </value>
        public Byte RssiAntenna1Control { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 2, control channel.
        /// </summary>
        /// <value>
        /// The antenna 2 control channel RSSI value.
        /// </value>
        public Byte RssiAntenna2Control { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 3, control channel.
        /// </summary>
        /// <value>
        /// The antenna 3 control channel RSSI value.
        /// </value>
        public Byte RssiAntenna3Control { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 0, extension channel
        /// </summary>
        /// <value>
        /// The antenna 0 extension channel RSSI value.
        /// </value>
        public Byte RssiAntenna0Ext { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 1, extension channel
        /// </summary>
        /// <value>
        /// The antenna 1 extension channel RSSI value.
        /// </value>
        public Byte RssiAntenna1Ext { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 2, extension channel
        /// </summary>
        /// <value>
        /// The antenna 2 extension channel RSSI value.
        /// </value>
        public Byte RssiAntenna2Ext { get; set; }

        /// <summary>
        /// Gets or sets the Received Signal Strength Indication (RSSI) value for the antenna 3, extension channel
        /// </summary>
        /// <value>
        /// The antenna 3 extension channel RSSI value.
        /// </value>
        public Byte RssiAntenna3Ext { get; set; }

        /// <summary>
        /// Gets or sets the extension channel frequency.
        /// </summary>
        /// <value>
        /// The extension channel frequency.
        /// </value>
        public UInt16 ExtensionChannelFrequency { get; set; }

        /// <summary>
        /// Gets or sets the extension channel flags.
        /// </summary>
        /// <value>
        /// The extension channel flags.
        /// </value>
        public RadioTapChannelFlags ExtensionChannelFlags { get; set; }

        /// <summary>
        /// Gets or sets the RF signal power at antenna 0.
        /// </summary>
        /// <value>
        /// The signal power.
        /// </value>
        public Byte DBmAntenna0SignalPower { get; set; }

        /// <summary>
        /// Gets or sets the RF signal noise at antenna 0.
        /// </summary>
        /// <value>
        /// The signal noise.
        /// </value>
        public Byte DBmAntenna0SignalNoise { get; set; }

        /// <summary>
        /// Gets or sets the RF signal power at antenna 1.
        /// </summary>
        /// <value>
        /// The signal power.
        /// </value>
        public Byte DBmAntenna1SignalPower { get; set; }

        /// <summary>
        /// Gets or sets the RF signal noise at antenna 1.
        /// </summary>
        /// <value>
        /// The signal noise.
        /// </value>
        public Byte DBmAntenna1SignalNoise { get; set; }

        /// <summary>
        /// Gets or sets the RF signal power at antenna 2.
        /// </summary>
        /// <value>
        /// The signal power.
        /// </value>
        public Byte DBmAntenna2SignalPower { get; set; }

        /// <summary>
        /// Gets or sets the RF signal noise at antenna 2.
        /// </summary>
        /// <value>
        /// The signal noise.
        /// </value>
        public Byte DBmAntenna2SignalNoise { get; set; }

        /// <summary>
        /// Gets or sets the RF signal power at antenna 3.
        /// </summary>
        /// <value>
        /// The signal power.
        /// </value>
        public Byte DBmAntenna3SignalPower { get; set; }

        /// <summary>
        /// Gets or sets the RF signal noise at antenna 3.
        /// </summary>
        /// <value>
        /// The signal noise.
        /// </value>
        public Byte DBmAntenna3SignalNoise { get; set; }

        /// <summary>
        /// Gets or sets the error vector magnitude for Chain 0.
        /// </summary>
        /// <value>
        /// The error vector magnitude.
        /// </value>
        public UInt32 ErrorVectorMagnitude0 { get; set; }

        /// <summary>
        /// Gets or sets the error vector magnitude for Chain 1.
        /// </summary>
        /// <value>
        /// The error vector magnitude.
        /// </value>
        public UInt32 ErrorVectorMagnitude1 { get; set; }

        /// <summary>
        /// Gets or sets the error vector magnitude for Chain 2.
        /// </summary>
        /// <value>
        /// The error vector magnitude.
        /// </value>
        public UInt32 ErrorVectorMagnitude2 { get; set; }

        /// <summary>
        /// Gets or sets the error vector magnitude for Chain 3.
        /// </summary>
        /// <value>
        /// The error vector magnitude.
        /// </value>
        public UInt32 ErrorVectorMagnitude3 { get; set; }

        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public override Byte[] Bytes
        {
            get
            {
                var ms = new MemoryStream();
                var writer = new BinaryWriter(ms);

                writer.Write(AMpduId);
                writer.Write(DelimiterCount);
                writer.Write(ModulationCodingScheme);
                writer.Write(SpatialStreamCount);
                writer.Write(RssiCombined);
                writer.Write(RssiAntenna0Control);
                writer.Write(RssiAntenna1Control);
                writer.Write(RssiAntenna2Control);
                writer.Write(RssiAntenna3Control);
                writer.Write(RssiAntenna0Ext);
                writer.Write(RssiAntenna1Ext);
                writer.Write(RssiAntenna2Ext);
                writer.Write(RssiAntenna3Ext);
                writer.Write(ExtensionChannelFrequency);
                writer.Write((UInt16) ExtensionChannelFlags);
                writer.Write(DBmAntenna0SignalPower);
                writer.Write(DBmAntenna0SignalNoise);
                writer.Write(DBmAntenna1SignalPower);
                writer.Write(DBmAntenna1SignalNoise);
                writer.Write(DBmAntenna2SignalPower);
                writer.Write(DBmAntenna2SignalNoise);
                writer.Write(DBmAntenna3SignalPower);
                writer.Write(DBmAntenna3SignalNoise);
                writer.Write(ErrorVectorMagnitude0);
                writer.Write(ErrorVectorMagnitude1);
                writer.Write(ErrorVectorMagnitude2);
                writer.Write(ErrorVectorMagnitude3);

                return ms.ToArray();
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiMacPhy" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiMacPhy(BinaryReader br)
        {
            AMpduId = br.ReadUInt32();
            DelimiterCount = br.ReadByte();
            ModulationCodingScheme = br.ReadByte();
            SpatialStreamCount = br.ReadByte();
            RssiCombined = br.ReadByte();
            RssiAntenna0Control = br.ReadByte();
            RssiAntenna1Control = br.ReadByte();
            RssiAntenna2Control = br.ReadByte();
            RssiAntenna3Control = br.ReadByte();
            RssiAntenna0Ext = br.ReadByte();
            RssiAntenna1Ext = br.ReadByte();
            RssiAntenna2Ext = br.ReadByte();
            RssiAntenna3Ext = br.ReadByte();
            ExtensionChannelFrequency = br.ReadUInt16();
            ExtensionChannelFlags = (RadioTapChannelFlags) br.ReadUInt16();
            DBmAntenna0SignalPower = br.ReadByte();
            DBmAntenna0SignalNoise = br.ReadByte();
            DBmAntenna1SignalPower = br.ReadByte();
            DBmAntenna1SignalNoise = br.ReadByte();
            DBmAntenna2SignalPower = br.ReadByte();
            DBmAntenna2SignalNoise = br.ReadByte();
            DBmAntenna3SignalPower = br.ReadByte();
            DBmAntenna3SignalNoise = br.ReadByte();
            ErrorVectorMagnitude0 = br.ReadUInt32();
            ErrorVectorMagnitude1 = br.ReadUInt32();
            ErrorVectorMagnitude2 = br.ReadUInt32();
            ErrorVectorMagnitude3 = br.ReadUInt32();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiMacPhy" /> class.
        /// </summary>
        public PpiMacPhy()
        { }

        #endregion Constructors
    }
}