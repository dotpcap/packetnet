/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  This file is licensed under the Apache License, Version 2.0.
 */

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet
{
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
}