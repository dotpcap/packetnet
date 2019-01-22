// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// The protocol encapsulated inside of the IP packet
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum IPProtocolType : byte
    {

        /// <summary> IPv6 Hop-by-Hop options. </summary>
        HOPOPTS = 0,

        /// <summary> Internet Control Message Protocol. </summary>
        ICMP = 1,

        /// <summary> Internet Group Management Protocol.</summary>
        IGMP = 2,

        /// <summary> IPIP tunnels (older KA9Q tunnels use 94). </summary>
        IPIP = 4,

        /// <summary> Transmission Control Protocol. </summary>
        TCP = 6,

        /// <summary> Exterior Gateway Protocol. </summary>
        EGP = 8,

        /// <summary> PUP protocol. </summary>
        PUP = 12,

        /// <summary> User Datagram Protocol. </summary>
        UDP = 17,

        /// <summary> XNS IDP protocol. </summary>
        IDP = 22,

        /// <summary> SO Transport Protocol Class 4. </summary>
        TP = 29,

        /// <summary> IPv6 header. </summary>
        IPV6 = 41,

        /// <summary> IPv6 routing header. </summary>
        ROUTING = 43,

        /// <summary> IPv6 fragmentation header. </summary>
        FRAGMENT = 44,

        /// <summary> Reservation Protocol. </summary>
        RSVP = 46,

        /// <summary> General Routing Encapsulation. </summary>
        GRE = 47,

        /// <summary> encapsulating security payload. </summary>
        ESP = 50,

        /// <summary> authentication header. </summary>
        AH = 51,

        /// <summary> ICMPv6. </summary>
        ICMPV6 = 58,

        /// <summary> IPv6 no next header. </summary>
        NONE = 59,

        /// <summary> IPv6 destination options. </summary>
        DSTOPTS = 60,

        /// <summary> OSPF v2/v3 </summary>
        OSPF = 89,

        /// <summary> Multicast Transport Protocol. </summary>
        MTP = 92,

        /// <summary> Encapsulation Header. </summary>
        ENCAP = 98,

        /// <summary> Protocol Independent Multicast. </summary>
        PIM = 103,

        /// <summary> Compression Header Protocol. </summary>
        COMP = 108,

        /// <summary> Mobility Header Protocol. </summary>
        MOBILITY = 135,

        /// <summary> Host Identity Protocol. </summary>
        HOSTIDENTITY = 139,

        /// <summary> Shim6 Protocol. </summary>
        SHIM6 = 140,

        /// <summary> reserved type 253, used in IPv6 Extionson Header. </summary>
        RESERVEDTYPE253 = 253,

        /// <summary> reserved type 254, used in IPv6 Extionson Header. </summary>
        RESERVEDTYPE254 = 254,


        /// <summary> Raw IP packets. </summary>
        RAW = 255,

        /// <summary> IP protocol mask.</summary>
        MASK = 0xff
    }
}