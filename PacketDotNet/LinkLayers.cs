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

namespace PacketDotNet;

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
    public enum LinkLayers : ushort
    {
        /// <summary>BSD loopback encapsulation.</summary>
        Null = 0,

        /// <summary>IEEE 802.3 Ethernet (10Mb, 100Mb, 1000Mb, and up).</summary>
        Ethernet = 1,

        /// <summary>Xerox experimental 3Mb Ethernet.</summary>
        ExperimentalEthernet = 2,

        /// <summary>AX.25 packet.</summary>
        Ax25 = 3,

        /// <summary>Reserved for PRONET.</summary>
        ProNet = 4,

        /// <summary>Reserved for MIT CHAOSNET.</summary>
        Chaos = 5,

        /// <summary>IEEE 802.5 Token Ring.</summary>
        Ieee8025 = 6,

        /// <summary>ARCNET Data Packets with BSD encapsulation.</summary>
        ArcNetBsd = 7,

        /// <summary>Serial Line IP.</summary>
        SerialLineIP = 8,

        /// <summary>PPP, as per RFC 1661 and RFC 1662.</summary>
        Ppp = 9,

        /// <summary>FDDI, as specified by ANSI INCITS 239-1994.</summary>
        Fddi = 10,

        /// <summary>Raw IP - do not use.</summary>
        RawLegacy = 12,

        /// <summary>BSD Serial Line IP - do not use.</summary>
        SerialLineIPBsd = 15,

        /// <summary>BSD PPP - do not use.</summary>
        PppBsd = 16,

        /// <summary>IP over ATM - do not use.</summary>
        AtmClipLegacy = 19,

        /// <summary>PPP in HDLC-like framing, as per RFC 1662.</summary>
        PppHdlc = 50,

        /// <summary>PPPoE, as per RFC 2516.</summary>
        PppEther = 51,

        /// <summary>Reserved for Symantec Firewall.</summary>
        SymantecFirewall = 99,

        /// <summary>RFC 1483 LLC/SNAP-encapsulated ATM.</summary>
        AtmRfc1483 = 100,

        /// <summary>Raw IP.</summary>
        Raw = 101,

        /// <summary>Reserved for BSD/OS SLIP BFP header.</summary>
        SerialLineIPBsdOs = 102,

        /// <summary>Reserved for BSD/OS PPP BFP header.</summary>
        PppBsdOs = 103,

        /// <summary>Cisco PPP with HDLC framing, as per section 4.3.1 of RFC 1547.</summary>
        CiscoHdlc = 104,

        /// <summary>IEEE 802.11 wireless LAN.</summary>
        Ieee80211 = 105,

        /// <summary>ATM classical IP.</summary>
        AtmClip = 106,

        /// <summary>Frame Relay LAPF frames.</summary>
        FrameRelay = 107,

        /// <summary>OpenBSD loopback encapsulation.</summary>
        Loop = 108,

        /// <summary>Reserved for OpenBSD IPSEC encapsulation.</summary>
        IPsecBsd = 109,

        /// <summary>Reserved for ATM LANE + 802.3.</summary>
        Lane8023 = 110,

        /// <summary>Reserved for NetBSD HIPPI.</summary>
        Hippi = 111,

        /// <summary>Reserved for NetBSD HDLC framing.</summary>
        Hdlc = 112,

        /// <summary>Linux "cooked" capture encapsulation.</summary>
        LinuxSll = 113,

        /// <summary>Radiotap header followed by an 802.11 header.</summary>
        Ieee80211RadioTap = 127,

        /// <summary>ARCNET Data Packets per RFC 1051 frames w/ variations.</summary>
        ArcNetLinux = 129,

        /// <summary>DOCSIS MAC frames, as described by the DOCSIS 3.1 MAC and Upper Layer Protocols Interface Specification or earlier specifications for MAC frames.</summary>
        Docsis = 143,

        /// <summary>PPP in HDLC-like encapsulation.</summary>
        PppPppd = 166,

        /// <summary>FRF.16.1 Multi-Link Frame Relay frames.</summary>
        Mfr = 182,

        /// <summary>Per-Packet Information information, as specified by the Per-Packet Information Header Specification.</summary>
        Ppi = 192,

        /// <summary>AX.25 packet, with a 1-byte KISS header containing a type indicator.</summary>
        Ax25Kiss = 202,

        /// <summary>PPP, as per RFC 1661 and RFC 1662, preceded with a one-byte pseudo-header with a zero value meaning "received by this host" and a non-zero value meaning "sent by this host".</summary>
        PppWithDir = 204,

        /// <summary>Raw IPv4; the packet begins with an IPv4 header.</summary>
        IPv4 = 228,

        /// <summary>Raw IPv6; the packet begins with an IPv6 header.</summary>
        IPv6 = 229,

        /// <summary>Protocol for communication between host and guest machines in VMware and KVM hypervisors.</summary>
        VSock = 271
    }
