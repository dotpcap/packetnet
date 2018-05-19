using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Contains information specific to 802.3 packets.
    /// </summary>
    public class Ppi8023 : PpiFields
    {
        /// <summary>
        /// Flags for errors detected at the time the packet was captured.
        /// </summary>
        [Flags]
        public enum ErrorFlags : uint
        {
            /// <summary>
            /// The frames FCS is invalid.
            /// </summary>
            InvalidFcs = 1,

            /// <summary>
            /// The frame has a sequence error.
            /// </summary>
            SequenceError = 2,

            /// <summary>
            /// The frame has a symbol error.
            /// </summary>
            SymbolError = 4,

            /// <summary>
            /// The frame has a data error.
            /// </summary>
            DataError = 8
        }

        /// <summary>
        /// 802.3 specific extension flags.
        /// </summary>
        [Flags]
        public enum StandardFlags : uint
        {
            /// <summary>
            /// FCS is present at the end of the packet
            /// </summary>
            FcsPresent = 1
        }


        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.Ppi802_3;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => 8;

        /// <summary>
        /// Gets or sets the standard 802.2 flags.
        /// </summary>
        /// <value>
        /// The standard flags.
        /// </value>
        public StandardFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the 802.3 error flags.
        /// </summary>
        /// <value>
        /// The error flags.
        /// </value>
        public ErrorFlags Errors { get; set; }

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
                writer.Write((UInt32) Errors);
                return ms.ToArray();
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ppi8023" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public Ppi8023(BinaryReader br)
        {
            Flags = (StandardFlags) br.ReadUInt32();
            Errors = (ErrorFlags) br.ReadUInt32();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ppi8023" /> class.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="errors">Standard Flags.</param>
        public Ppi8023(StandardFlags flags, ErrorFlags errors)
        {
            Flags = flags;
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ppi8023" /> class.
        /// </summary>
        public Ppi8023()
        { }

        #endregion Constructors
    }
}