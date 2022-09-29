/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// Defines the lengths and positions of the vxlan fields within
    /// a vxlan packet
    /// </summary>
    public struct VxlanFields
    {
        /// <summary>Length of the Flags in bytes.</summary>
        public static readonly int FlagsLength = 1;

        /// <summary>Position of the VNI.</summary>
        public static readonly int FlagsPosition = 0;

        /// <summary>Length of the Reserved in bytes.</summary>
        public static readonly int Reserved1Length = 3;

        /// <summary>Length of the VNI in bytes.</summary>
        public static readonly int VniLength = 3;

        /// <summary>Position of the VNI.</summary>
        public static readonly int VniPosition = 4;

        /// <summary>Length of the Reserved in bytes.</summary>
        public static readonly int Reserved2Length = 1;

        /// <summary>Length of the vxlan header in bytes.</summary>
        public static readonly int HeaderLength = FlagsLength + Reserved1Length + VniLength + Reserved2Length;

        /// <summary>IANA assigned Vxlan destination UDP port by default.</summary>
        public static readonly ushort DefaultUdpDstPortForVxlan = 4789;

        /// <summary>The Vxlan port should be configurable according to RFC 7348.</summary>
        public static ushort UdpDstPortForVxlan = DefaultUdpDstPortForVxlan;
    }
}
