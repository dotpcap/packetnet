// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************

using System;

namespace PacketDotNet
{
    /// <summary>
    /// String representation of an IP protocol value
    /// </summary>
    public class IPProtocol
    {
        /// <summary> Fetch a protocol description.</summary>
        /// <param name="code">the code associated with the message.
        /// </param>
        /// <returns> a message describing the significance of the IP protocol.
        /// </returns>
        public static System.String getDescription(int code)
        {
            System.Int32 c = (System.Int32) code;
            if (messages.ContainsKey(c))
            {
                return (System.String) messages[c];
            } else
            {
                return "unknown";
            }
        }

        /// <summary> 'Human-readable' IP protocol descriptions.</summary>
        private static System.Collections.Hashtable messages = new System.Collections.Hashtable();

        static IPProtocol()
        {
            messages[(System.Int32) IPProtocolType.IP] = "Dummy protocol for TCP";
            messages[(System.Int32) IPProtocolType.HOPOPTS] = "IPv6 Hop-by-Hop options";
            messages[(System.Int32) IPProtocolType.ICMP] = "Internet Control Message Protocol";
            messages[(System.Int32) IPProtocolType.IGMP] = "Internet Group Management Protocol";
            messages[(System.Int32) IPProtocolType.IPIP] = "IPIP tunnels";
            messages[(System.Int32) IPProtocolType.TCP] = "Transmission Control Protocol";
            messages[(System.Int32) IPProtocolType.EGP] = "Exterior Gateway Protocol";
            messages[(System.Int32) IPProtocolType.PUP] = "PUP protocol";
            messages[(System.Int32) IPProtocolType.UDP] = "User Datagram Protocol";
            messages[(System.Int32) IPProtocolType.IDP] = "XNS IDP protocol";
            messages[(System.Int32) IPProtocolType.TP] = "SO Transport Protocol Class 4";
            messages[(System.Int32) IPProtocolType.IPV6] = "IPv6 header";
            messages[(System.Int32) IPProtocolType.ROUTING] = "IPv6 routing header";
            messages[(System.Int32) IPProtocolType.FRAGMENT] = "IPv6 fragmentation header";
            messages[(System.Int32) IPProtocolType.RSVP] = "Reservation Protocol";
            messages[(System.Int32) IPProtocolType.GRE] = "General Routing Encapsulation";
            messages[(System.Int32) IPProtocolType.ESP] = "encapsulating security payload";
            messages[(System.Int32) IPProtocolType.AH] = "authentication header";
            messages[(System.Int32) IPProtocolType.ICMPV6] = "ICMPv6";
            messages[(System.Int32) IPProtocolType.NONE] = "IPv6 no next header";
            messages[(System.Int32) IPProtocolType.DSTOPTS] = "IPv6 destination options";
            messages[(System.Int32) IPProtocolType.MTP] = "Multicast Transport Protocol";
            messages[(System.Int32) IPProtocolType.ENCAP] = "Encapsulation Header";
            messages[(System.Int32) IPProtocolType.PIM] = "Protocol Independent Multicast";
            messages[(System.Int32) IPProtocolType.COMP] = "Compression Header Protocol";
            messages[(System.Int32) IPProtocolType.RAW] = "Raw IP Packet";
//            messages[(System.Int32) IPProtocolType.INVALID] = "INVALID IP";
        }
    }
}
