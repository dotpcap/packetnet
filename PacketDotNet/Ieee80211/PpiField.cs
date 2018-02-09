using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Abstract class for all PPI fields
    /// </summary>
    public abstract class PpiField
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public abstract PpiFieldType FieldType
        {
            get;
        }
   
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public abstract int Length { get; }
            
        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public abstract byte[] Bytes { get; }
            
        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Parse a PPI indicated by type, from a given BinaryReader
        /// </summary>
        /// <param name="fieldType">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        /// <param name="fieldLength">
        /// The maximum number of bytes that the field to be parsed can encompass.
        /// </param>
        /// <returns>
        /// A <see cref="PpiField"/>
        /// </returns>
        public static PpiField Parse (int fieldType, BinaryReader br, ushort fieldLength)
        {
            var type = (PpiFieldType)fieldType;
            switch (type)
            {
                case PpiFieldType.PpiReserved0:
                    return new PpiUnknown (fieldType, br, fieldLength);
                case PpiFieldType.PpiReserved1:
                    return new PpiUnknown (fieldType, br, fieldLength);
                case PpiFieldType.PpiCommon:
                    return new PpiCommon (br);
                case PpiFieldType.PpiMacExtensions:
                    return new PpiMacExtensions (br);
                case PpiFieldType.PpiMacPhy:
                    return new PpiMacPhy (br);
                case PpiFieldType.PpiSpectrum:
                    return new PpiSpectrum (br);
                case PpiFieldType.PpiProcessInfo:
                    return new PpiProcessInfo (br);
                case PpiFieldType.PpiCaptureInfo:
                    return new PpiCaptureInfo (br);
                case PpiFieldType.PpiAggregation:
                    return new PpiAggregation (br);
                case PpiFieldType.Ppi802_3:
                    return new Ppi802_3 (br);
                default:
                    return new PpiUnknown (fieldType, br, fieldLength);
            }
        }

        #endregion Public Methods
    }
}