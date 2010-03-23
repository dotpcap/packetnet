namespace PacketDotNet.LLDP
{
    /// <summary>
    /// The Chassis ID TLV subtypes
    /// </summary>
    public enum ChassisSubTypes
    {
        /// <summary>A Chassis Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        ChassisComponent = 1,
        /// <summary>An Interface Alias identifier</summary>
        /// <remarks>See IETF RFC 2863</remarks>
        InterfaceAlias = 2,
        /// <summary>A Port Component identifier</summary>
        /// <remarks>See IETF RFC 2737</remarks>
        PortComponent = 3,
        /// <summary>A MAC (Media Access Control) Address identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        MACAddress = 4,
        /// <summary>A Network Address (IP Address) Identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        NetworkAddress = 5,
        /// <summary>An Interface Name identifier</summary>
        /// <remarks>See IEEE Std 802</remarks>
        InterfaceName = 6,
        /// <summary>A Locally Assigned identifier</summary>
        LocallyAssigned = 7
    };
}