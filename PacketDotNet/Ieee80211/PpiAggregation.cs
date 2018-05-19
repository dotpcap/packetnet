using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// The PPI Aggregation field is used to identify which physical interface a frame was collected on
    /// when multiple capture interfaces are in use.
    /// </summary>
    public class PpiAggregation : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.PpiAggregation;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length => 4;

        /// <summary>
        /// Zero-based index of the physical interface the packet was captured from.
        /// </summary>
        /// <value>
        /// The interface id.
        /// </value>
        public UInt32 InterfaceId { get; set; }

        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public override Byte[] Bytes => BitConverter.GetBytes(InterfaceId);

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiAggregation" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiAggregation(BinaryReader br)
        {
            InterfaceId = br.ReadUInt32();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiAggregation" /> class.
        /// </summary>
        /// <param name='interfaceId'>
        /// The interface id.
        /// </param>
        public PpiAggregation(UInt32 interfaceId)
        {
            InterfaceId = interfaceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiAggregation" /> class.
        /// </summary>
        public PpiAggregation()
        { }

        #endregion Constructors
    }
}