using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The PpiUnknown field class can be used to represent any field types not
    /// currently supported by PacketDotNet. Any unsupported field types encountered during
    /// parsing will be stored as PpiUnknown fields.
    /// </summary>
    public class PpiUnknown : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType { get; }

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => Bytes.Length;

        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public override Byte[] Bytes => UnknownBytes;

        /// <summary>
        /// Gets or sets the field data.
        /// </summary>
        /// <value>
        /// The fields values bytes.
        /// </value>
        public Byte[] UnknownBytes { get; set; }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiUnknown" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='typeNumber'>
        /// The PPI field type number
        /// </param>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        /// <param name='length'>
        /// The number of bytes the unknown field contains.
        /// </param>
        public PpiUnknown(Int32 typeNumber, BinaryReader br, Int32 length)
        {
            FieldType = (PpiFieldType) typeNumber;
            UnknownBytes = br.ReadBytes(length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiUnknown" /> class.
        /// </summary>
        /// <param name='typeNumber'>
        /// The PPI field type number.
        /// </param>
        public PpiUnknown(Int32 typeNumber)
        {
            FieldType = (PpiFieldType) typeNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiUnknown" /> class.
        /// </summary>
        /// <param name='typeNumber'>
        /// The PPI field type number.
        /// </param>
        /// <param name='unknownBytes'>
        /// The field data.
        /// </param>
        public PpiUnknown(Int32 typeNumber, Byte[] unknownBytes)
        {
            FieldType = (PpiFieldType) typeNumber;
            UnknownBytes = unknownBytes;
        }

        #endregion Constructors
    }
}