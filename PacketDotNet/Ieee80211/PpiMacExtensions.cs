using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The 802.11n MAC Extension field contains radio information specific to 802.11n.
    /// </summary>
    public class PpiMacExtensions : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.PpiMacExtensions;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => 12;

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

                writer.Write((UInt32) Flags);
                writer.Write(AMpduId);
                writer.Write(DelimiterCount);
                writer.Write(new Byte[3]);

                return ms.ToArray();
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiMacExtensions" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiMacExtensions(BinaryReader br)
        {
            Flags = (PpiMacExtensionFlags) br.ReadUInt32();
            AMpduId = br.ReadUInt32();
            DelimiterCount = br.ReadByte();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiMacExtensions" /> class.
        /// </summary>
        public PpiMacExtensions()
        { }

        #endregion Constructors
    }
}