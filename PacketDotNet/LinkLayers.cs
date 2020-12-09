/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Link-layer type codes.
    /// <p>
    /// Taken from libpcap/bpf/net/bpf.h and pcap/net/bpf.h.
    /// </p>
    /// <p>
    /// The link-layer type is used to determine what data-structure the
    /// IP protocol bits will be encapsulated inside of.
    /// </p>
    /// <p>
    /// On a 10/100mbps network, packets are encapsulated inside of ethernet.
    /// 14-byte ethernet headers which contain MAC addresses and an ethernet type
    /// field.
    /// </p>
    /// <p>
    /// On ethernet over ppp, the link-layer type is raw, and packets
    /// are not encapsulated in any ethernet header.
    /// </p>
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LinkLayers : byte
    {
        /// <summary>no link-layer encapsulation.</summary>
        Null = 0,

        /// <summary>Ethernet (10Mb).</summary>
        Ethernet = 1,

        /// <summary>Experimental Ethernet (3Mb).</summary>
        ExperimentalEthernet3MB = 2,

        /// <summary>Amateur Radio AX.25.</summary>
        AmateurRadioAX25 = 3,

        /// <summary>Proteon ProNET Token Ring.</summary>
        ProteonProNetTokenRing = 4,

        /// <summary>Chaos.</summary>
        Chaos = 5,

        /// <summary>IEEE 802 Networks.</summary>
        Ieee802 = 6,

        /// <summary>ARCNET.</summary>
        ArcNet = 7,

        /// <summary>Serial Line IP.</summary>
        Slip = 8,

        /// <summary>Point-to-point Protocol.</summary>
        Ppp = 9,

        /// <summary>FDDI.</summary>
        Fddi = 10,

        /// <summary>Raw IP.</summary>
        RawLegacy = 12,

        /// <summary>BSD Slip.</summary>
        SlipBsd = 15,

        /// <summary>BSD PPP.</summary>
        PppBsd = 16,

        /// <summary>IP over ATM.</summary>
        AtmClip = 19,

        /// <summary>PPP over HDLC.</summary>
        PppOverHdlc = 50,

        /// <summary>PPPoE.</summary>
        Pppoe = 51,

        /// <summary>LLC/SNAP encapsulated atm.</summary>
        LlcSnapAtm = 100,

        /// <summary>Raw IP.</summary>
        Raw = 101,

        /// <summary>Cisco HDLC.</summary>
        CiscoHdlc = 104,

        /// <summary>IEEE 802.11 wireless.</summary>
        Ieee80211 = 105,

        /// <summary>OpenBSD loopback.</summary>
        Loop = 108,

        /// <summary>Linux cooked sockets.</summary>
        LinuxSll = 113,

        /// <summary>
        /// Header for 802.11 plus a number of bits of link-layer information
        /// including radio information, used by some recent BSD drivers as
        /// well as the madwifi Atheros driver for Linux.
        /// </summary>
        Ieee80211Radio = 127,

        /// <summary>
        /// Per Packet Information encapsulated packets.
        /// DLT_ requested by Gianluca Varenni &lt;gianluca.varenni@cacetech.com&gt;.
        /// See http://www.cacetech.com/documents/PPI%20Header%20format%201.0.7.pdf
        /// </summary>
        Ppi = 192
    }
}