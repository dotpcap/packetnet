namespace PacketDotNet.LLDP
{
    /// <summary>
    /// The Port ID TLV subtypes
    /// </summary>
    public enum PortSubTypes
    {
        /// <summary>An Interface Alias identifier</summary>
        /// <remarks>See IETF RFC 2863</remarks>
        InterfaceAlias = 1,
        /// <summary>A Port Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        PortComponent = 2,
        /// <summary>A MAC (Media Access Control) Address identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        MACAddress = 3,
        /// <summary>A Network Address (IP Address) Identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        NetworkAddress = 4,
        /// <summary>An Interface Name identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        InterfaceName = 5,
        /// <summary>An Agent Circiut ID identifier</summary>
        /// <remarks>See IETF RFC 3046</remarks>
        AgentCircuitID = 6,
        /// <summary>A Locally Assigned identifier</summary>
        /// <remarks>See IETF RFC 3046</remarks>
        LocallyAssigned = 7
    };
}