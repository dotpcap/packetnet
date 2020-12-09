/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet
{
    /// <summary>
    /// Code constants for well-defined ethernet protocols.
    /// EtherType is a two-octet field in an Ethernet frame, as defined by the Ethernet II framing networking standard.
    /// It is used to indicate which protocol is encapsulated in the payload.
    /// Also contains entries taken from linux/if_ether.h and tcpdump/ethertype.h
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum EthernetType : ushort
    {
        /// <summary>
        /// No Ethernet type
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Internet Protocol, Version 4 (IPv4)
        /// </summary>
        IPv4 = 0x0800,

        /// <summary>
        /// Address Resolution Protocol (ARP)
        /// </summary>
        Arp = 0x0806,

        /// <summary>
        /// Reverse Address Resolution Protocol (RARP)
        /// </summary>
        ReverseArp = 0x8035,

        /// <summary>
        /// Wake-On-Lan (WOL)
        /// </summary>
        WakeOnLan = 0x0842,

        /// <summary>
        /// AppleTalk (Ethertalk)
        /// </summary>
        AppleTalk = 0x809B,

        /// <summary>
        /// AppleTalk Address Resolution Protocol (AARP)
        /// </summary>
        AppleTalkArp = 0x80F3,

        /// <summary>
        /// VLAN-tagged frame (IEEE 802.1Q)
        /// </summary>
        VLanTaggedFrame = 0x8100,

        /// <summary>
        /// Novell IPX (alt)
        /// </summary>
        NovellIpx = 0x8137,

        /// <summary>
        /// Novell
        /// </summary>
        Novell = 0x8138,

        /// <summary>
        /// Internet Protocol, Version 6 (IPv6)
        /// </summary>
        IPv6 = 0x86DD,

        /// <summary>
        /// MAC Control
        /// </summary>
        MacControl = 0x8808,

        /// <summary>
        /// CobraNet
        /// </summary>
        CobraNet = 0x8819,

        /// <summary>
        /// MPLS unicast
        /// </summary>
        MplsUnicast = 0x8847,

        /// <summary>
        /// MPLS multicast
        /// </summary>
        MplsMulticast = 0x8848,

        /// <summary>
        /// PPPoE Discovery Stage
        /// </summary>
        PppoeDiscoveryStage = 0x8863,

        /// <summary>
        /// PPPoE Session Stage
        /// </summary>
        PppoeSessionStage = 0x8864,

        /// <summary>
        /// EAP over LAN (IEEE 802.1X)
        /// </summary>
        EapOverLan = 0x888E,

        /// <summary>
        /// PROFINET
        /// </summary>
        Profinet = 0x8892,

        /// <summary>
        /// HyperSCSI (SCSI over Ethernet)
        /// </summary>
        HyperScsi = 0x889A,

        /// <summary>
        /// ATA over Ethernet
        /// </summary>
        AtaOverEthernet = 0x88A2,

        /// <summary>
        /// EtherCAT Protocol
        /// </summary>
        EtherCat = 0x88A4,

        /// <summary>
        /// Provider Bridging (IEEE 802.1ad)
        /// </summary>
        ProviderBridging = 0x88A8,

        /// <summary>
        /// AVB Transport Protocol (AVBTP)
        /// </summary>
        Avbtp = 0x88B5,

        /// <summary>
        /// Link Layer Discovery Protocol (LLDP)
        /// </summary>
        Lldp = 0x88CC,

        /// <summary>
        /// SERCOS III
        /// </summary>
        SercosIII = 0x88CD,

        /// <summary>
        /// Circuit Emulation Services over Ethernet (MEF-8)
        /// </summary>
        CecOverEthernet = 0x88D8,

        /// <summary>
        /// HomePlug
        /// </summary>
        HomePlug = 0x88E1,

        /// <summary>
        /// MAC security (IEEE 802.1AE)
        /// </summary>
        MacSecurity = 0x88E5,

        /// <summary>
        /// Precision Time Protocol (IEEE 1588)
        /// </summary>
        Ptp = 0x88f7,

        /// <summary>
        /// IEEE 802.1ag Connectivity Fault Management (CFM) Protocol / ITU-T Recommendation Y.1731 (OAM)
        /// </summary>
        CfmOrOam = 0x8902,

        /// <summary>
        /// Fibre Channel over Ethernet
        /// </summary>
        Fcoe = 0x8906,

        /// <summary>
        /// FCoE Initialization Protocol
        /// </summary>
        FcoeInitialization = 0x8914,

        /// <summary>
        /// Q-in-Q
        /// </summary>
        QInQ = 0x9100,

        /// <summary>
        /// Veritas Low Latency Transport (LLT)
        /// </summary>
        VeritasLlt = 0xCAFE,

        /// <summary>
        /// Ethernet loopback packet
        /// </summary>
        Loop = 0x0060,

        /// <summary>
        /// Ethernet echo packet
        /// </summary>
        Echo = 0x0200
    }
}