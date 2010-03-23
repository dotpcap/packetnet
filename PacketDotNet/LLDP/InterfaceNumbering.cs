namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Interface Numbering Types
    /// </summary>
    /// <remarks>Source IETF RFC 802.1AB</remarks>
    public enum InterfaceNumbering
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>Interface Index</summary>
        ifIndex,
        /// <summary>System Port Number</summary>
        SystemPortNumber
    };
}