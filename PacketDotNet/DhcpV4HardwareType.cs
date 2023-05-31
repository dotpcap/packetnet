/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet;

    public enum DhcpV4HardwareType : byte
    {
        Reserved = 0,
        Ethernet = 1,
        ExperimentalEthernet = 2,
        AX25 = 3,
        ProteonProNETTokenRing = 4,
        Chaos = 5,
        IEEE802Networks = 6,
        ARCNET = 7,
        Hyperchannel = 8,
        Lanstar = 9,
        AutonetShortAddress = 10,
        LocalTalk = 11,
        LocalNet = 12,
        Ultralink = 13,
        SMDS = 14,
        FrameRelay = 15,
        AsynchronousTransmissionMode = 16,
        HDLC = 17,
        FibreChannel = 18,
        AsynchronousTransmissionMode1 = 19,
        SerialLine = 20,
        AsynchronousTransmissionMode2 = 21,
        MILSTD188220 = 22,
        Metricom = 23,
        IEEE1394_1995 = 24,
        MAPOS = 25,
        Twinaxial = 26,
        EUI64 = 27,
        HIPARP = 28,
        IPandARPoverISO78163 = 29,
        ARPSec = 30,
        IPsectunnel = 31,
        InfiniBand = 32,
        TIA102Project25CommonAirInterface = 33,
        WiegandInterface = 34,
        PureIP = 35,
        HWEXP1 = 36,
        HFI = 37
    }