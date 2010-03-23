using System;
using System.Text;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A System Description TLV
    /// </summary>
    public class SystemDescription : StringTLV
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        #region Constructors

        /// <summary>
        /// Creates a System Description TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The System Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public SystemDescription(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            log.Debug("");
        }

        /// <summary>
        /// Creates a System Description TLV and sets it value
        /// </summary>
        /// <param name="description">
        /// A textual Description of the system
        /// </param>
        public SystemDescription(string description) : base(TLVTypes.SystemDescription,
                                                            description)
        {
            log.Debug("");
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Description of the system
        /// </value>
        public string Description
        {
            get { return StringValue; }
            set { StringValue = value; }
        }

        #endregion
    }
}