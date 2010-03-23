using System;
using System.Text;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A System Name TLV
    /// </summary>
    public class SystemName : StringTLV
    {
        #region Constructors

        /// <summary>
        /// Creates a System Name TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Name TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemName(byte[] bytes, int offset) :
            base(bytes, offset)
        {}

        /// <summary>
        /// Creates a System Name TLV and sets it value
        /// </summary>
        /// <param name="name">
        /// A textual Name of the system
        /// </param>
        public SystemName(string name) : base(TLVTypes.SystemName, name)
        {
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Name of the system
        /// </value>
        public string Name
        {
            get { return StringValue; }
            set { StringValue = value; }
        }
        
        #endregion
    }
}