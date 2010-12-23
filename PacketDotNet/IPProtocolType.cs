// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************

using System;

namespace PacketDotNet
{
    /// <summary>
    /// The protocol encapsulated inside of the IP packet
    /// </summary>
    public enum IPProtocolType : byte
    {
        /// <summary> Dummy protocol for TCP. </summary>
        IP = 0,
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
        /// <summary> Multicast Transport Protocol. </summary>
        MTP = 92,
        /// <summary> Encapsulation Header. </summary>
        ENCAP = 98,
        /// <summary> Protocol Independent Multicast. </summary>
        PIM = 103,
        /// <summary> Compression Header Protocol. </summary>
        COMP = 108,
        /// <summary> Raw IP packets. </summary>
        RAW = 255,
        /// <summary> IP protocol mask.</summary>
        MASK = 0xff
    }
}
