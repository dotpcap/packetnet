using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The PPI Spectrum field is intended to be compatible with the sweep records
    /// returned by the Wi-Spy spectrum analyzer.
    /// </summary>
    public class PpiSpectrum : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.PpiSpectrum;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => 20 + SamplesData.Length;

        /// <summary>
        /// Gets or sets the starting frequency in kHz.
        /// </summary>
        /// <value>
        /// The starting frequency.
        /// </value>
        public UInt32 StartingFrequency { get; set; }

        /// <summary>
        /// Gets or sets the resolution of each sample in Hz.
        /// </summary>
        /// <value>
        /// The resolution in Hz.
        /// </value>
        public UInt32 Resolution { get; set; }

        /// <summary>
        /// Gets or sets the amplitude offset (in 0.001 dBm)
        /// </summary>
        /// <value>
        /// The amplitude offset.
        /// </value>
        public UInt32 AmplitudeOffset { get; set; }

        /// <summary>
        /// Gets or sets the amplitude resolution (in .001 dBm)
        /// </summary>
        /// <value>
        /// The amplitude resolution.
        /// </value>
        public UInt32 AmplitudeResolution { get; set; }

        /// <summary>
        /// Gets or sets the maximum raw RSSI value reported by the device.
        /// </summary>
        /// <value>
        /// The maximum rssi.
        /// </value>
        public UInt16 MaximumRssi { get; set; }

        /// <summary>
        /// Gets or sets the data samples.
        /// </summary>
        /// <value>
        /// The data samples.
        /// </value>
        public Byte[] SamplesData { get; set; }

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

                writer.Write(StartingFrequency);
                writer.Write(Resolution);
                writer.Write(AmplitudeOffset);
                writer.Write(AmplitudeResolution);
                writer.Write(MaximumRssi);
                writer.Write((UInt16) SamplesData.Length);
                writer.Write(SamplesData);

                return ms.ToArray();
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiSpectrum" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiSpectrum(BinaryReader br)
        {
            StartingFrequency = br.ReadUInt32();
            Resolution = br.ReadUInt32();
            AmplitudeOffset = br.ReadUInt32();
            AmplitudeResolution = br.ReadUInt32();
            MaximumRssi = br.ReadUInt16();
            var samplesLength = br.ReadUInt16();
            SamplesData = br.ReadBytes(samplesLength);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiSpectrum" /> class.
        /// </summary>
        public PpiSpectrum()
        { }

        #endregion Constructors
    }
}